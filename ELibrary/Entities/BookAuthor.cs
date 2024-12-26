using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ELibrary.Entities
{
    [Table("BooksAuthors")]
    [PrimaryKey(nameof(BookId), nameof(AuthorId))]
    public class BookAuthor
    {
        [ForeignKey(nameof(Book))] 
        public Guid BookId { get; set; }

        [ForeignKey(nameof(Author))] 
        public Guid AuthorId { get; set; }

        public Book Book { get; set; }

        public Author Author { get; set; }
    }
}