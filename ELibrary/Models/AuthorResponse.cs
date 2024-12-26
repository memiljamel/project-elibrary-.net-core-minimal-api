namespace ELibrary.Models
{
    public class AuthorResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Email { get; set; }

        public int? BookCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}