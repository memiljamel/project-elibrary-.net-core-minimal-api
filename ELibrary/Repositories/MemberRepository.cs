using ELibrary.Data;
using ELibrary.Entities;
using ELibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ELibrary.Repositories
{
    public class MemberRepository : GenericRepository<Member>, IMemberRepository
    {
        public MemberRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PaginatedList<Member>> GetPagedMembers(
            string? memberNumber,
            string? name,
            string? address,
            string? email,
            string? phone,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Members
                .Include(m => m.Phones)
                .AsNoTracking();

            if (memberNumber != null)
            {
                query = query.Where(m => m.MemberNumber.ToLower().Contains(memberNumber.ToLower()));
            }

            if (name != null)
            {
                query = query.Where(m => m.Name.ToLower().Contains(name.ToLower()));
            }

            if (address != null)
            {
                query = query.Where(m => m.Address.ToLower().Contains(address.ToLower()));
            }

            if (email != null)
            {
                query = query.Where(m => m.Email.ToLower().Contains(email.ToLower()));
            }

            if (phone != null)
            {
                query = query.Where(m => m.Phones.Any(p => p.PhoneNumber.ToLower().Contains(phone.ToLower())));
            }

            query = query.OrderByDescending(m => m.CreatedAt);

            return await PaginatedList<Member>.CreateAsync(query, pageNumber, pageSize);
        }

        public bool IsMemberNumberUnique(string memberNumber, Guid? id)
        {
            return !_context.Members.Any(m => m.MemberNumber == memberNumber && m.Id != id);
        }
        
        public bool IsMemberExists(Guid id)
        {
            return _context.Members.Any(m => m.Id == id);
        }

        public async Task<Member?> GetById(Guid? id)
        {
            return await _context.Members
                .Include(m => m.Phones)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}