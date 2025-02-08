
namespace Infrastructure.DTOs
{
    public class GetTrialByStatusDTO
    {
        public string Status {  get; set; }
        public int? Page {  get; set; }
        public int? Size { get; set; }
    }
}
