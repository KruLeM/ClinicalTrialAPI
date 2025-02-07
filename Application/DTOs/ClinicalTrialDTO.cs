namespace Application.DTOs
{
    public class ClinicalTrialDTO 
    {
        public string TrialId { get; set; }
        public string Title { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int Participants { get; set; }
        public string Status { get; set; }
        public int? Duration { get; set; }
    }
}
