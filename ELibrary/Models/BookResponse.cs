using ELibrary.Enums;

namespace ELibrary.Models
{
    public class BookResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public CategoryEnum Category { get; set; }

        public string Publisher { get; set; }

        public int Quantity { get; set; }

        public List<string>? Authors { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}