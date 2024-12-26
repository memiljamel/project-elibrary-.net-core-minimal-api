using ELibrary.Entities;
using ELibrary.Models;

namespace ELibrary.Repositories
{
    public interface IAuthorRepository : IGenericRepository<Author>
    {
        Task<PaginatedList<Author>> GetPagedAuthors(
            string? name,
            string? email,
            int? bookCount,
            int pageNumber,
            int pageSize);
        
        bool IsNameUnique(string name, Guid? id);
        
        bool IsAuthorExists(Guid id);
    }
}