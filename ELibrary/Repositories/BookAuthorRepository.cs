using ELibrary.Data;
using ELibrary.Entities;

namespace ELibrary.Repositories
{
    public class BookAuthorRepository : GenericRepository<BookAuthor>, IBookAuthorRepository
    {
        public BookAuthorRepository(AppDbContext context) : base(context)
        {
        }
    }
}