using ELibrary.Entities;
using ELibrary.Models;

namespace ELibrary.Repositories
{
    public interface ILoanRepository : IGenericRepository<Loan>
    {
        Task<PaginatedList<Loan>> GetPagedLoans(
            DateOnly? loanDate,
            DateOnly? returnDate,
            string? bookTitle,
            string? memberNumber,
            bool? isReturned,
            int pageNumber,
            int pageSize);
    }
}