namespace ELibrary.Models
{
    public class UpdateLoanRequest
    {
        public Guid Id { get; set; }
        
        public DateOnly? ReturnDate { get; set; }
        
        public Guid BookId { get; set; }
        
        public Guid MemberId { get; set; }
    }
}