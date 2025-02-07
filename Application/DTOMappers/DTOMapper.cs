using Application.DTOs;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.DTOMappers
{
    public class ClinicalTrialDTOMapper
    {
        public static IEnumerable<ClinicalTrialDTO> EntitiesToDTOs(IEnumerable<ClinicalTrial> clinicalTrials)
        {
            return clinicalTrials.Select(ct => new ClinicalTrialDTO
            {
                TrialId = ct.TrialId,
                Title = ct.Title,
                StartDate = ct.StartDate,
                EndDate = ct.EndDate,
                Participants = ct.Participants,
                Status = ct.Status.ToString(),
                Duration = ct.Duration
            });
        }

        public static ClinicalTrialDTO EntityToDTO(ClinicalTrial ct)
        {
            return new ClinicalTrialDTO
            {
                TrialId = ct.TrialId,
                Title = ct.Title,
                StartDate = ct.StartDate,
                EndDate = ct.EndDate,
                Participants = ct.Participants,
                Status = ct.Status.ToString(),
                Duration = ct.Duration
            };
        }
    }
}
