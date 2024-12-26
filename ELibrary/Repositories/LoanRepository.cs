using ELibrary.Data;
using ELibrary.Entities;
using ELibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ELibrary.Repositories
{
    public class LoanRepository : GenericRepository<Loan>, ILoanRepository
    {
        public LoanRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PaginatedList<Loan>> GetPagedLoans(
            DateOnly? loanDate,
            DateOnly? returnDate,
            string? bookTitle,
            string? memberNumber,
            bool? isReturned,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Loans
                .Include(l => l.Member)
                .Include(l => l.Book)
                .AsQueryable();

            if (loanDate.HasValue)
            {
                query = query.Where(l => l.LoanDate >= loanDate.Value);
            }

            if (returnDate.HasValue)
            {
                query = query.Where(l => l.ReturnDate >= returnDate.Value);
            }

            if (bookTitle != null)
            {
                query = query.Where(l => l.Book.Title.ToLower().Contains(bookTitle.ToLower()));
            }

            if (memberNumber != null)
            {
                query = query.Where(l => l.Member.MemberNumber.ToLower().Contains(memberNumber.ToLower()));
            }

            if (isReturned.HasValue)
            {
                if (isReturned.Value)
                {
                    query = query.Where(l => l.ReturnDate != null);
                }
                else
                {
                    query = query.Where(l => l.ReturnDate == null);
                }
            }

            query = query.OrderByDescending(l => l.CreatedAt);

            return await PaginatedList<Loan>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<Loan?> GetById(Guid? id)
        {
            return await _context.Loans
                .Include(l => l.Member)
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);
        }
    }
}