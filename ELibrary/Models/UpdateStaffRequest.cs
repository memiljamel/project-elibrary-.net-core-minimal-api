using System.Text.Json.Serialization;
using ELibrary.Enums;

namespace ELibrary.Models
{
    public class UpdateStaffRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        
        public string Username { get; set; }
        
        public string? Password { get; set; }
        
        public string? PasswordConfirmation { get; set; }
        
        public string Name { get; set; }
        
        public string StaffNumber { get; set; }
        
        public AccessLevelEnum AccessLevel { get; set; }
        
        public IFormFile? Image { get; set; }
    }
}