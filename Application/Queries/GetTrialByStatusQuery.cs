using Domain.Entities;
using MediatR;

namespace Application.Queries
{
    public record GetTrialByStatusQuery(TrialStatus status)  : IRequest<List<ClinicalTrial>>;   
}
