using ELibrary.Entities;
using ELibrary.Enums;
using ELibrary.Models;

namespace ELibrary.Repositories
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<PaginatedList<Book>> GetPagedBooks(
            string? title,
            CategoryEnum? category,
            string? publisher,
            int? quantity,
            string? authors,
            int pageNumber,
            int pageSize);
        
        bool IsBookExists(Guid id);
    }
}