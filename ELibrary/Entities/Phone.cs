using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELibrary.Entities
{
    [Table("Phones")]
    public class Phone : BaseEntity
    {
        [Required]
        [MaxLength(15)]
        [Column(Order = 1)]
        public string PhoneNumber { get; set; }

        [ForeignKey(nameof(Member))]
        [Column(Order = 2)]
        public Guid MemberId { get; set; }

        public Member Member { get; set; }
    }
}