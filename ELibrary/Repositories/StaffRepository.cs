using ELibrary.Data;
using ELibrary.Entities;
using ELibrary.Enums;
using ELibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ELibrary.Repositories
{
    public class StaffRepository : GenericRepository<Staff>, IStaffRepository
    {
        public StaffRepository(AppDbContext context)
            : base(context)
        {
        }
        
        public async Task<PaginatedList<Staff>> GetPagedStaffs(
            string? username,
            string? name,
            string? staffNumber,
            AccessLevelEnum? accessLevel,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Staffs.AsNoTracking();

            if (username != null)
            {
                query = query.Where(s => s.Username.ToLower().Contains(username.ToLower()));
            }

            if (name != null)
            {
                query = query.Where(s => s.Name.ToLower().Contains(name.ToLower()));
            }

            if (staffNumber != null)
            {
                query = query.Where(s => s.StaffNumber.ToLower().Contains(staffNumber.ToLower()));
            }

            if (accessLevel.HasValue)
            {
                query = query.Where(s => s.AccessLevel == accessLevel.Value);
            }

            query = query.OrderByDescending(s => s.CreatedAt);

            return await PaginatedList<Staff>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<Staff?> GetByUsername(string? username)
        {
            return await _context.Staffs.FirstOrDefaultAsync(s => s.Username == username);
        }

        public bool IsStaffNumberUnique(string staffNumber, Guid? id)
        {
            return !_context.Staffs.Any(s => s.StaffNumber == staffNumber && s.Id != id);
        }

        public bool IsUsernameUnique(string username, Guid? id)
        {
            return !_context.Staffs.Any(s => s.Username == username && s.Id != id);
        }
    }
}