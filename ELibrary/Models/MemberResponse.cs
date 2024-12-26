namespace ELibrary.Models
{
    public class MemberResponse
    {
        public Guid Id { get; set; }

        public string MemberNumber { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public List<string>? Phones { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}