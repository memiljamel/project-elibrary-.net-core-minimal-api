using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ELibrary.Enums;

namespace ELibrary.Entities
{
    [Table("Books")]
    public class Book : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        [Column(Order = 1)]
        public string Title { get; set; }

        [Required] 
        [Column(Order = 2)] 
        public CategoryEnum Category { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(Order = 3)]
        public string Publisher { get; set; }

        [Required] 
        [Column("Qty", Order = 4)] 
        public int Quantity { get; set; }
        
        [Column(Order = 5)]
        public string? ImageUrl { get; set; }

        public ICollection<Loan> Loans { get; set; }

        public ICollection<BookAuthor> BooksAuthors { get; set; }
    }
}