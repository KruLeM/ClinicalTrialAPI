using Domain.Entities;
using MediatR;

namespace Application.Queries
{
    public record GetTrialByStatusQuery(string status)  : IRequest<List<ClinicalTrial>>;   
}
