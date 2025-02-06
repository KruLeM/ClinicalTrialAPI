using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class ClinicalTrial
    {
        [JsonPropertyName("trialId")]
        [Key]
        public string TrialId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("startDate")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateOnly? EndDate { get; set; }

        [JsonPropertyName("participants")]
        public int Participants { get; set; }

        [JsonPropertyName("status")]
        public TrialStatus Status { get; set; }
        public int? Duration { get; set; }
    }
}
