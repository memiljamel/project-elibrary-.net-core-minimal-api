using System.Text.Json.Serialization;

namespace ELibrary.Models
{
    public class UpdateAuthorRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string? Email { get; set; }
    }
}