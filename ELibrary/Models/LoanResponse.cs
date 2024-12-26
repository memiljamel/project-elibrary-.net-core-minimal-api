namespace ELibrary.Models
{
    public class LoanResponse
    {
        public Guid Id { get; set; }
        
        public DateOnly LoanDate { get; set; }
        
        public DateOnly? ReturnDate { get; set; }
        
        public string? BookTitle { get; set; }
        
        public string? MemberNumber { get; set; }

        public bool IsReturned { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}