using System.Net;
using ELibrary.Entities;
using ELibrary.Models;
using ELibrary.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ELibrary.Endpoint
{
    public static class AuthorEndpoint
    {
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/api/authors")
                .WithTags("Authors");

            group.MapGet("/", GetAuthors)
                .RequireAuthorization("All");

            group.MapGet("/{authorId:guid}", GetAuthor)
                .RequireAuthorization("All");

            group.MapPost("/", CreateAuthor)
                .RequireAuthorization("All");

            group.MapPut("/{authorId:guid}", UpdateAuthor)
                .RequireAuthorization("All");

            group.MapDelete("/{authorId:guid}", DeleteAuthor)
                .RequireAuthorization("All");
        }

        private static async Task<Results<Ok<WebResponse<PaginatedList<AuthorResponse>>>, NotFound>> GetAuthors(
            [FromServices] IUnitOfWork unitOfWork,
            [FromQuery] string? name,
            [FromQuery] string? email,
            [FromQuery] int? bookCount,
            [FromQuery(Name = "page")] int pageNumber = 1,
            [FromQuery(Name = "size")] int pageSize = 15)
        {
            if (pageNumber < 1)
            {
                return TypedResults.NotFound();
            }

            var authors = await unitOfWork.AuthorRepository.GetPagedAuthors(
                name,
                email,
                bookCount,
                pageNumber,
                pageSize);

            if (authors.PageIndex != 1 && pageNumber > authors.TotalPages)
            {
                return TypedResults.NotFound();
            }

            var response = authors.Select(a => ToAuthorResponse(a))
                .ToList();

            return TypedResults.Ok(new WebResponse<PaginatedList<AuthorResponse>>
            {
                Code = HttpStatusCode.OK,
                Status = "OK",
                Data = new PaginatedList<AuthorResponse>(
                    response,
                    authors.TotalCount,
                    authors.PageIndex,
                    pageSize),
                Meta = new MetaResponse
                {
                    CurrentPage = authors.PageIndex,
                    PerPage = authors.PageSize,
                    Total = authors.TotalCount,
                    TotalPage = authors.TotalPages,
                    HasPreviousPage = authors.HasPreviousPage(),
                    HasNextPage = authors.HasNextPage()
                }
            });
        }

        private static async Task<Results<Ok<WebResponse<AuthorResponse>>, NotFound>> GetAuthor(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid authorId)
        {
            var author = await unitOfWork.AuthorRepository.GetById(authorId);
            if (author == null)
            {
                return TypedResults.NotFound();
            }

            var response = ToAuthorResponse(author);

            return TypedResults.Ok(new WebResponse<AuthorResponse>
            {
                Code = HttpStatusCode.OK,
                Status = "OK",
                Data = response
            });
        }

        private static async Task<Results<Created<WebResponse<AuthorResponse>>, ValidationProblem>> CreateAuthor(
            [FromServices] IValidator<CreateAuthorRequest> validator,
            [FromServices] IUnitOfWork unitOfWork,
            [FromBody] CreateAuthorRequest request)
        {
            var result = await validator.ValidateAsync(request);

            if (result.IsValid)
            {
                var author = new Author
                {
                    Name = request.Name,
                    Email = request.Email
                };
                unitOfWork.AuthorRepository.Add(author);

                await unitOfWork.SaveChangesAsync();

                var response = ToAuthorResponse(author);

                return TypedResults.Created($"/api/authors/{author.Id}", new WebResponse<AuthorResponse>
                {
                    Code = HttpStatusCode.Created,
                    Status = "Created",
                    Data = response
                });
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static async Task<Results<Ok<WebResponse<AuthorResponse>>, NotFound, ValidationProblem>> UpdateAuthor(
            [FromServices] IValidator<UpdateAuthorRequest> validator,
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid authorId,
            [FromBody] UpdateAuthorRequest request)
        {
            request.Id = authorId;

            var result = await validator.ValidateAsync(request);

            if (result.IsValid)
            {
                var author = await unitOfWork.AuthorRepository.GetById(request.Id);
                if (author == null)
                {
                    return TypedResults.NotFound();
                }

                author.Name = request.Name;
                author.Email = request.Email;
                author.UpdatedAt = DateTime.UtcNow;

                await unitOfWork.SaveChangesAsync();

                var response = ToAuthorResponse(author);

                return TypedResults.Ok(new WebResponse<AuthorResponse>
                {
                    Code = HttpStatusCode.OK,
                    Status = "OK",
                    Data = response
                });
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static async Task<Results<Ok<WebResponse<object>>, NotFound>> DeleteAuthor(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid authorId)
        {
            var author = await unitOfWork.AuthorRepository.GetById(authorId);
            if (author == null)
            {
                return TypedResults.NotFound();
            }

            unitOfWork.AuthorRepository.Remove(author);

            await unitOfWork.SaveChangesAsync();

            return TypedResults.Ok(new WebResponse<object>
            {
                Code = HttpStatusCode.OK,
                Status = "OK",
                Data = null
            });
        }

        private static AuthorResponse ToAuthorResponse(Author author)
        {
            return new AuthorResponse
            {
                Id = author.Id,
                Name = author.Name,
                Email = author.Email,
                BookCount = author.BooksAuthors?.Count(),
                CreatedAt = author.CreatedAt,
                UpdatedAt = author.UpdatedAt
            };
        }
    }
}