﻿using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class ClinicalTrial
    {
        [JsonPropertyName("trialId")]
        public string TrialId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("participants")]
        public int? Participants { get; set; }

        [JsonPropertyName("status")]
        public TrialStatus Status { get; set; }
        public int? Duration { get; set; }
    }
}
