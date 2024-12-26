using ELibrary.Data;
using ELibrary.Entities;

namespace ELibrary.Repositories
{
    public class PhoneRepository : GenericRepository<Phone>, IPhoneRepository
    {
        public PhoneRepository(AppDbContext context) : base(context)
        {
        }
    }
}