using ELibrary.Data;
using ELibrary.Entities;
using ELibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ELibrary.Repositories
{
    public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PaginatedList<Author>> GetPagedAuthors(
            string? name,
            string? email,
            int? bookCount,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Authors
                .Include(a => a.BooksAuthors)
                .AsQueryable();

            if (name != null)
            {
                query = query.Where(a => a.Name.ToLower().Contains(name.ToLower()));
            }

            if (email != null)
            {
                query = query.Where(a => a.Email.ToLower().Contains(email.ToLower()));
            }

            if (bookCount.HasValue)
            {
                query = query.Where(a => a.BooksAuthors.Count >= bookCount.Value);
            }

            query = query.OrderByDescending(a => a.CreatedAt);

            return await PaginatedList<Author>.CreateAsync(query, pageNumber, pageSize);
        }

        public bool IsNameUnique(string name, Guid? id)
        {
            return !_context.Authors.Any(a => a.Name == name && a.Id != id);
        }

        public bool IsAuthorExists(Guid id)
        {
            return _context.Authors.Any(a => a.Id == id);
        }

        public async Task<Author?> GetById(Guid? id)
        {
            return await _context.Authors
                .Include(a => a.BooksAuthors)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}