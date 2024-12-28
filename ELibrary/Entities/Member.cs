using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ELibrary.Entities
{
    [Table("Members")]
    [Index(nameof(MemberNumber), IsUnique = true)]
    public class Member : BaseEntity
    {
        [Required]
        [MaxLength(16)]
        [Column(Order = 1)]
        public string MemberNumber { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(Order = 2)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1024)]
        [Column(Order = 3)]
        public string Address { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(Order = 4)]
        public string Email { get; set; }
        
        [Column(Order = 5)]
        public string? ImageUrl { get; set; }

        public ICollection<Phone> Phones { get; set; }

        public ICollection<Loan> Loans { get; set; }
    }
}