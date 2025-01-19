namespace ELibrary.Models
{
    public class MetaResponse
    {
        public int CurrentPage { get; set; }
        
        public int PerPage { get; set; }
        
        public int Total { get; set; }
        
        public int TotalPage { get; set; }
        
        public  bool HasPreviousPage { get; set; }
        
        public  bool HasNextPage { get; set; }
    }
}