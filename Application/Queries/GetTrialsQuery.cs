using Application.DTOs;
using Application.Returns;
using MediatR;

namespace Application.Queries
{
    public record GetTrialsQuery (int? page, int? size) : IRequest<PaginatedResponse<ClinicalTrialDTO>>;
}
