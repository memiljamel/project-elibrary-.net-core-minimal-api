using ELibrary.Data;
using ELibrary.Entities;
using ELibrary.Enums;
using ELibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ELibrary.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PaginatedList<Book>> GetPagedBooks(
            string? title,
            CategoryEnum? category,
            string? publisher,
            int? quantity,
            string? authors,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Books
                .Include(b => b.BooksAuthors)
                .ThenInclude(ba => ba.Author)
                .AsQueryable();

            if (title != null)
            {
                query = query.Where(b => b.Title.ToLower().Contains(title.ToLower()));
            }

            if (category.HasValue)
            {
                query = query.Where(b => b.Category == category.Value);
            }

            if (publisher != null)
            {
                query = query.Where(b => b.Publisher.ToLower().Contains(publisher.ToLower()));
            }

            if (quantity.HasValue)
            {
                query = query.Where(b => b.Quantity >= quantity.Value);
            }

            if (authors != null)
            {
                query = query.Where(b =>
                    b.BooksAuthors.Any(ba => ba.Author.Name.ToLower().Contains(authors.ToLower())));
            }

            query = query.OrderByDescending(b => b.CreatedAt);

            return await PaginatedList<Book>.CreateAsync(query, pageNumber, pageSize);
        }
        
        public bool IsBookExists(Guid id)
        {
            return _context.Books.Any(b => b.Id == id);
        }

        public async Task<Book?> GetById(Guid? id)
        {
            return await _context.Books
                .Include(b => b.BooksAuthors)
                .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}