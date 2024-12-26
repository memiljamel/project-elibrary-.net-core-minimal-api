using ELibrary.Entities;
using ELibrary.Enums;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace ELibrary.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookAuthor> BooksAuthors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var staff = new Staff
            {
                Username = "administrator",
                Password = BC.HashPassword("password"),
                Name = "Administrator",
                StaffNumber = "STF-0000",
                AccessLevel = AccessLevelEnum.Administrator
            };

            modelBuilder.Entity<Staff>().HasData(staff);
        }
    }
}