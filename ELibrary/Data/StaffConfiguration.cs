using ELibrary.Entities;
using ELibrary.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BC = BCrypt.Net.BCrypt;

namespace ELibrary.Data
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.HasData(
                new Staff
                {
                    Username = "administrator",
                    Password = BC.HashPassword("password"),
                    Name = "Administrator",
                    StaffNumber = "STF-0000",
                    AccessLevel = AccessLevelEnum.Administrator
                }
            );
        }
    }
}