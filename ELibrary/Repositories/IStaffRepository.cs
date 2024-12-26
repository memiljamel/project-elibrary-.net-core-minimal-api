using ELibrary.Entities;
using ELibrary.Enums;
using ELibrary.Models;

namespace ELibrary.Repositories
{
    public interface IStaffRepository : IGenericRepository<Staff>
    {
        Task<PaginatedList<Staff>> GetPagedStaffs(
            string? username, 
            string? name, 
            string? staffNumber, 
            AccessLevelEnum? accessLevel, 
            int pageNumber, 
            int pageSize);

        Task<Staff?> GetByUsername(string? username);

        bool IsStaffNumberUnique(string staffNumber, Guid? id);

        bool IsUsernameUnique(string username, Guid? id);
    }
}