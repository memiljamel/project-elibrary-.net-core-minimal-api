namespace ELibrary.Models
{
    public class CreateMemberRequest
    {
        public string MemberNumber { get; set; }
        
        public string Name { get; set; }
        
        public string Address { get; set; }
        
        public string Email { get; set; }
        
        public List<string> Phones { get; set; }

        public IFormFile? Image { get; set; }
    }
}