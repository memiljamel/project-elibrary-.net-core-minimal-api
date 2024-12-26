using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELibrary.Entities
{
    [Table("Loans")]
    public class Loan : BaseEntity
    {
        [Required] 
        [Column(Order = 1)] 
        public DateOnly LoanDate { get; set; }

        [Column(Order = 2)] 
        public DateOnly? ReturnDate { get; set; }

        [ForeignKey(nameof(Member))]
        [Column(Order = 3)]
        public Guid MemberId { get; set; }

        [ForeignKey(nameof(Book))]
        [Column(Order = 4)]
        public Guid BookId { get; set; }

        public Member Member { get; set; }

        public Book Book { get; set; }
    }
}