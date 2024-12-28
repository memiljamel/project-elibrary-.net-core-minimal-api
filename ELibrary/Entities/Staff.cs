using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ELibrary.Enums;
using Microsoft.EntityFrameworkCore;

namespace ELibrary.Entities
{
    [Table("Staffs")]
    [Index(nameof(Username), nameof(StaffNumber), IsUnique = true)]
    public class Staff : BaseEntity
    {
        [Required]
        [Column(Order = 1)]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [Column(Order = 2)]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(Order = 3)]
        public string Name { get; set; }

        [Required]
        [MaxLength(16)]
        [Column(Order = 4)]
        public string StaffNumber { get; set; }

        [Required] 
        [Column("Role", Order = 5)] 
        public AccessLevelEnum AccessLevel { get; set; }
        
        [Column(Order = 6)]
        public string? ImageUrl { get; set; }
    }
}