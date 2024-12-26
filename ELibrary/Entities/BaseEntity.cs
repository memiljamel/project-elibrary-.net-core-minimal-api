using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELibrary.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(Order = 98)] 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(Order = 99)] 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}