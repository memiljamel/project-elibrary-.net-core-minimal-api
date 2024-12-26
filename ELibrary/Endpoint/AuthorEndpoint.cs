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
                .DisableAntiforgery()
                .RequireAuthorization("All");

            group.MapPut("/{authorId:guid}", UpdateAuthor)
                .DisableAntiforgery()
                .RequireAuthorization("All");

            group.MapDelete("/{authorId:guid}", DeleteAuthor)
                .RequireAuthorization("All");
        }

        private static async Task<Results<Ok<PaginatedList<AuthorResponse>>, NotFound>> GetAuthors(
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

            return TypedResults.Ok(new PaginatedList<AuthorResponse>(
                response,
                authors.TotalCount,
                authors.PageIndex,
                pageSize));
        }

        private static async Task<Results<Ok<AuthorResponse>, NotFound>> GetAuthor(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid authorId)
        {
            var author = await unitOfWork.AuthorRepository.GetById(authorId);
            if (author == null)
            {
                return TypedResults.NotFound();
            }

            var response = ToAuthorResponse(author);

            return TypedResults.Ok(response);
        }

        private static async Task<Results<Created<AuthorResponse>, ValidationProblem>> CreateAuthor(
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

                return TypedResults.Created($"/api/authors/{author.Id}", response);
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static async Task<Results<Ok<AuthorResponse>, NotFound, ValidationProblem>> UpdateAuthor(
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

                return TypedResults.Ok(response);
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static async Task<Results<NoContent, NotFound>> DeleteAuthor(
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

            return TypedResults.NoContent();
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