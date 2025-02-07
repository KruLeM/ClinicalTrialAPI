using Application.DTOs;
using MediatR;

namespace Application.Queries
{
    public record GetTrialByStatusQuery(string status)  : IRequest<IEnumerable<ClinicalTrialDTO>>;   
}
