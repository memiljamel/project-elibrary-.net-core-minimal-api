using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELibrary.Entities
{
    [Table("Authors")]
    public class Author : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        [Column(Order = 1)]
        public string Name { get; set; }

        [MaxLength(100)] 
        [Column(Order = 2)] 
        public string? Email { get; set; }

        public ICollection<BookAuthor> BooksAuthors { get; set; }
    }
}