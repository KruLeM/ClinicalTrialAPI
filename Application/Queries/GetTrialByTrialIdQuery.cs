using Domain.Entities;
using MediatR;

namespace Application.Queries
{
    public record GetTrialByTrialIdQuery (string trialId)  : IRequest<ClinicalTrial>;   
}
