using System.Text.Json.Serialization;
using ELibrary.Enums;

namespace ELibrary.Models
{
    public class UpdateBookRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        
        public CategoryEnum Category { get; set; }
        
        public string Publisher { get; set; }
        
        public int Quantity { get; set; }
        
        public List<Guid> AuthorIds { get; set; }
        
        public IFormFile? Image { get; set; }
    }
}