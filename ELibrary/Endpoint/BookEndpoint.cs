﻿using ELibrary.Entities;
using ELibrary.Enums;
using ELibrary.Models;
using ELibrary.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ELibrary.Endpoint
{
    public static class BookEndpoint
    {
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/api/books")
                .WithTags("Book");

            group.MapGet("/", GetBooks)
                .RequireAuthorization("All");

            group.MapGet("/{bookId:guid}", GetBook)
                .RequireAuthorization("All");

            group.MapPost("/", CreateBook)
                .DisableAntiforgery()
                .RequireAuthorization("All");

            group.MapPut("/{bookId:guid}", UpdateBook)
                .DisableAntiforgery()
                .RequireAuthorization("All");

            group.MapDelete("/{bookId:guid}", DeleteBook)
                .RequireAuthorization("All");
        }

        private static async Task<Results<Ok<PaginatedList<BookResponse>>, NotFound>> GetBooks(
            [FromServices] IUnitOfWork unitOfWork,
            [FromQuery] string? title,
            [FromQuery] CategoryEnum? category,
            [FromQuery] string? publisher,
            [FromQuery] int? quantity,
            [FromQuery] string? authors,
            [FromQuery(Name = "page")] int pageNumber = 1,
            [FromQuery(Name = "size")] int pageSize = 15)
        {
            if (pageNumber < 1)
            {
                return TypedResults.NotFound();
            }

            var books = await unitOfWork.BookRepository.GetPagedBooks(
                title,
                category,
                publisher,
                quantity,
                authors,
                pageNumber,
                pageSize);

            if (books.PageIndex != 1 && pageNumber > books.TotalPages)
            {
                return TypedResults.NotFound();
            }

            var response = books.Select(b => ToBookResponse(b))
                .ToList();

            return TypedResults.Ok(new PaginatedList<BookResponse>(
                response,
                books.TotalCount,
                books.PageIndex,
                pageSize));
        }

        private static async Task<Results<Ok<BookResponse>, NotFound>> GetBook(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid bookId)
        {
            var book = await unitOfWork.BookRepository.GetById(bookId);
            if (book == null)
            {
                return TypedResults.NotFound();
            }

            var response = ToBookResponse(book);

            return TypedResults.Ok(response);
        }

        private static async Task<Results<Created<BookResponse>, ValidationProblem>> CreateBook(
            [FromServices] IValidator<CreateBookRequest> validator,
            [FromServices] IUnitOfWork unitOfWork,
            [FromForm] CreateBookRequest request)
        {
            var result = await validator.ValidateAsync(request);

            if (result.IsValid)
            {
                await unitOfWork.BeginTransactionAsync();

                try
                {
                    var book = new Book
                    {
                        Title = request.Title,
                        Category = request.Category,
                        Publisher = request.Publisher,
                        Quantity = request.Quantity,
                    };

                    if (request.Image != null)
                    {
                        var filename = Path.GetRandomFileName() + Path.GetExtension(request.Image.FileName);

                        await using var stream = new FileStream(Path.Combine("wwwroot", filename), FileMode.Create);
                        await request.Image.CopyToAsync(stream);

                        book.ImageUrl = filename;
                    }

                    unitOfWork.BookRepository.Add(book);

                    await unitOfWork.SaveChangesAsync();

                    foreach (var authorId in request.AuthorIds)
                    {
                        var bookAuthor = new BookAuthor
                        {
                            BookId = book.Id,
                            AuthorId = authorId
                        };
                        unitOfWork.BookAuthorRepository.Add(bookAuthor);

                        await unitOfWork.SaveChangesAsync();
                    }

                    await unitOfWork.CommitAsync();

                    var response = ToBookResponse(book);

                    return TypedResults.Created($"/api/books/{book.Id}", response);
                }
                catch (Exception e)
                {
                    await unitOfWork.RollbackAsync();

                    throw new Exception(e.Message);
                }
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static async Task<Results<Ok<BookResponse>, NotFound, ValidationProblem>> UpdateBook(
            [FromServices] IValidator<UpdateBookRequest> validator,
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid bookId,
            [FromForm] UpdateBookRequest request)
        {
            request.Id = bookId;

            var result = await validator.ValidateAsync(request);

            if (result.IsValid)
            {
                await unitOfWork.BeginTransactionAsync();

                try
                {
                    var book = await unitOfWork.BookRepository.GetById(request.Id);
                    if (book == null)
                    {
                        return TypedResults.NotFound();
                    }

                    unitOfWork.BookAuthorRepository.RemoveRange(book.BooksAuthors);

                    await unitOfWork.SaveChangesAsync();

                    book.Title = request.Title;
                    book.Category = request.Category;
                    book.Publisher = request.Publisher;
                    book.Quantity = request.Quantity;
                    book.UpdatedAt = DateTime.UtcNow;

                    if (request.Image != null)
                    {
                        if (book.ImageUrl != null && File.Exists(Path.Combine("wwwroot", book.ImageUrl)))
                        {
                            File.Delete(Path.Combine("wwwroot", book.ImageUrl));
                        }

                        var filename = Path.GetRandomFileName() + Path.GetExtension(request.Image.FileName);

                        await using var stream = new FileStream(Path.Combine("wwwroot", filename), FileMode.Create);
                        await request.Image.CopyToAsync(stream);

                        book.ImageUrl = filename;
                    }

                    await unitOfWork.SaveChangesAsync();

                    foreach (var authorId in request.AuthorIds)
                    {
                        var bookAuthor = new BookAuthor
                        {
                            BookId = book.Id,
                            AuthorId = authorId
                        };
                        unitOfWork.BookAuthorRepository.Add(bookAuthor);

                        await unitOfWork.SaveChangesAsync();
                    }

                    await unitOfWork.CommitAsync();

                    var response = ToBookResponse(book);

                    return TypedResults.Ok(response);
                }
                catch (Exception e)
                {
                    await unitOfWork.RollbackAsync();

                    throw new Exception(e.Message);
                }
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static async Task<Results<NoContent, NotFound>> DeleteBook(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid bookId)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();
                
                var book = await unitOfWork.BookRepository.GetById(bookId);
                if (book == null)
                {
                    return TypedResults.NotFound();
                }
                
                unitOfWork.BookAuthorRepository.RemoveRange(book.BooksAuthors);

                await unitOfWork.SaveChangesAsync();

                if (book.ImageUrl != null && File.Exists(Path.Combine("wwwroot", book.ImageUrl)))
                {
                    File.Delete(Path.Combine("wwwroot", book.ImageUrl));
                }
                
                unitOfWork.BookRepository.Remove(book);

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return TypedResults.NoContent();
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackAsync();
                
                throw new Exception(e.Message);
            }
        }

        private static BookResponse ToBookResponse(Book book)
        {
            return new BookResponse
            {
                Id = book.Id,
                Title = book.Title,
                Category = book.Category,
                Publisher = book.Publisher,
                Quantity = book.Quantity,
                Authors = book.BooksAuthors?.Select(ba => ba.Author?.Name).ToList(),
                ImageUrl = book.ImageUrl,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt
            };
        }
    }
}