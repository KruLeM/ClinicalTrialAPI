using Application.DTOs;
using MediatR;

namespace Application.Queries
{
    public record GetTrialByTrialIdQuery (string trialId)  : IRequest<ClinicalTrialDTO>;   
}
