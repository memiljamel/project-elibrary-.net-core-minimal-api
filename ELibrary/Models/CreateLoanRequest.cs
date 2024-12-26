namespace ELibrary.Models
{
    public class CreateLoanRequest
    {
        public DateOnly LoanDate { get; set; }
        
        public Guid BookId { get; set; }
        
        public Guid MemberId { get; set; }
    }
}