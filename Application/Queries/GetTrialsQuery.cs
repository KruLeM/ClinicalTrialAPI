using Domain.Entities;
using MediatR;

namespace Application.Queries
{
    public record GetTrialsQuery : IRequest<List<ClinicalTrial>>;
}
