using ELibrary.Enums;

namespace ELibrary.Models
{
    public class StaffResponse
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public string StaffNumber { get; set; }

        public AccessLevelEnum AccessLevel { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}