using ELibrary.Entities;
using ELibrary.Models;
using ELibrary.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ELibrary.Endpoint
{
    public static class LoanEndpoint
    {
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/api/loans")
                .WithTags("Loans");

            group.MapGet("/", GetLoans)
                .RequireAuthorization("All");

            group.MapGet("/{loanId:guid}", GetLoan)
                .RequireAuthorization("All");

            group.MapPost("/", CreateLoan)
                .RequireAuthorization("All");

            group.MapPut("/{loanId:guid}", UpdateLoan)
                .RequireAuthorization("All");

            group.MapDelete("/{loanId:guid}", DeleteLoan)
                .RequireAuthorization("All");
        }

        private static async Task<Results<Ok<WebResponse<PaginatedList<LoanResponse>>>, NotFound>> GetLoans(
            [FromServices] IUnitOfWork unitOfWork,
            [FromQuery] DateOnly? loanDate,
            [FromQuery] DateOnly? returnDate,
            [FromQuery] string? bookTitle,
            [FromQuery] string? memberNumber,
            [FromQuery] bool? isReturned,
            [FromQuery(Name = "page")] int pageNumber = 1,
            [FromQuery(Name = "size")] int pageSize = 15)
        {
            if (pageNumber < 1)
            {
                return TypedResults.NotFound();
            }

            var loans = await unitOfWork.LoanRepository.GetPagedLoans(
                loanDate,
                returnDate,
                bookTitle,
                memberNumber,
                isReturned,
                pageNumber,
                pageSize);

            if (loans.PageIndex != 1 && pageNumber > loans.TotalPages)
            {
                return TypedResults.NotFound();
            }

            var response = loans.Select(a => ToLoanResponse(a))
                .ToList();

            return TypedResults.Ok(new WebResponse<PaginatedList<LoanResponse>>
            {
                Code = 200,
                Status = "Ok",
                Data = new PaginatedList<LoanResponse>(
                    response,
                    loans.TotalCount,
                    loans.PageIndex,
                    pageSize),
                Meta = new MetaResponse
                {
                    CurrentPage = loans.PageIndex,
                    PerPage = loans.PageSize,
                    Total = loans.TotalCount,
                    TotalPage = loans.TotalPages
                }
            });
        }

        private static async Task<Results<Ok<WebResponse<LoanResponse>>, NotFound>> GetLoan(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid loanId)
        {
            var loan = await unitOfWork.LoanRepository.GetById(loanId);
            if (loan == null)
            {
                return TypedResults.NotFound();
            }

            var response = ToLoanResponse(loan);

            return TypedResults.Ok(new WebResponse<LoanResponse>
            {
                Code = 200,
                Status = "Ok",
                Data = response
            });
        }

        private static async Task<Results<Created<WebResponse<LoanResponse>>, ValidationProblem>> CreateLoan(
            [FromServices] IValidator<CreateLoanRequest> validator,
            [FromServices] IUnitOfWork unitOfWork,
            [FromBody] CreateLoanRequest request)
        {
            var result = await validator.ValidateAsync(request);

            if (result.IsValid)
            {
                await unitOfWork.BeginTransactionAsync();

                try
                {
                    var loan = new Loan
                    {
                        LoanDate = request.LoanDate,
                        MemberId = request.MemberId,
                        BookId = request.BookId,
                    };
                    unitOfWork.LoanRepository.Add(loan);

                    await unitOfWork.SaveChangesAsync();

                    var book = await unitOfWork.BookRepository.GetById(request.BookId);
                    if (book != null)
                    {
                        book.Quantity -= 1;
                    }

                    await unitOfWork.SaveChangesAsync();
                    await unitOfWork.CommitAsync();

                    var response = ToLoanResponse(loan);

                    return TypedResults.Created($"/api/loans/{loan.Id}", new WebResponse<LoanResponse>
                    {
                        Code = 201,
                        Status = "Created",
                        Data = response
                    });
                }
                catch (Exception e)
                {
                    await unitOfWork.RollbackAsync();

                    throw new Exception(e.Message);
                }
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static async Task<Results<Ok<WebResponse<LoanResponse>>, NotFound, ValidationProblem>> UpdateLoan(
            [FromServices] IValidator<UpdateLoanRequest> validator,
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid loanId,
            [FromBody] UpdateLoanRequest request)
        {
            request.Id = loanId;

            var result = await validator.ValidateAsync(request);

            if (result.IsValid)
            {
                await unitOfWork.BeginTransactionAsync();

                try
                {
                    var loan = await unitOfWork.LoanRepository.GetById(request.Id);
                    if (loan == null)
                    {
                        return TypedResults.NotFound();
                    }

                    loan.ReturnDate = request.ReturnDate;
                    loan.BookId = request.BookId;
                    loan.MemberId = request.MemberId;
                    loan.UpdatedAt = DateTime.UtcNow;

                    if (request.ReturnDate != null)
                    {
                        loan.Book.Quantity += 1;
                    }

                    await unitOfWork.SaveChangesAsync();
                    await unitOfWork.CommitAsync();

                    var response = ToLoanResponse(loan);

                    return TypedResults.Ok(new WebResponse<LoanResponse>
                    {
                        Code = 200,
                        Status = "Ok",
                        Data = response
                    });
                }
                catch (Exception e)
                {
                    await unitOfWork.RollbackAsync();

                    throw new Exception(e.Message);
                }
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static async Task<Results<Ok<WebResponse<object>>, NotFound>> DeleteLoan(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid loanId)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var loan = await unitOfWork.LoanRepository.GetById(loanId);
                if (loan == null)
                {
                    return TypedResults.NotFound();
                }

                unitOfWork.LoanRepository.Remove(loan);

                if (loan.ReturnDate == null)
                {
                    loan.Book.Quantity += 1;
                }

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return TypedResults.Ok(new WebResponse<object>
                {
                    Code = 200,
                    Status = "Ok",
                    Data = null
                });
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackAsync();

                throw new Exception(e.Message);
            }
        }

        private static LoanResponse ToLoanResponse(Loan loan)
        {
            return new LoanResponse
            {
                Id = loan.Id,
                LoanDate = loan.LoanDate,
                ReturnDate = loan.ReturnDate,
                BookTitle = loan.Book?.Title,
                MemberNumber = loan.Member?.MemberNumber,
                IsReturned = loan.ReturnDate != null,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt
            };
        }
    }
}