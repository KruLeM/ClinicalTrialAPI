using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class ClinicalTrial
    {
        [Key]
        public string TrialId { get; set; }
        public string Title { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int Participants { get; set; }
        public TrialStatus Status { get; set; }
        public int? Duration { get; set; }
    }
}
