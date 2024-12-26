using ELibrary.Entities;
using ELibrary.Models;

namespace ELibrary.Repositories
{
    public interface IMemberRepository : IGenericRepository<Member>
    {
        Task<PaginatedList<Member>> GetPagedMembers(
            string? memberNumber, 
            string? name, 
            string? address, 
            string? email, 
            string? phone, 
            int pageNumber, 
            int pageSize);
        
        bool IsMemberNumberUnique(string memberNumber, Guid? id);
        
        bool IsMemberExists(Guid id);
    }
}