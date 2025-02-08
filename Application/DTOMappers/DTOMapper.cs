﻿using Application.DTOs;
using Domain.Entities;

namespace Application.DTOMappers
{
    public class ClinicalTrialDTOMapper
    {
        public static IEnumerable<ClinicalTrialDTO> EntitiesToDTOs(IEnumerable<ClinicalTrial> clinicalTrials)
        {
            if (clinicalTrials == null)
            {
                return null;
            }

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
            if (ct == null)
            {
                return null;
            }

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
