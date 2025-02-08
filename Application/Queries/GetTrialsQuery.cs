using Application.DTOs;
using Application.Responses;
using MediatR;

namespace Application.Queries
{
    public record GetTrialsQuery (int? page, int? size) : IRequest<PaginatedResponse<ClinicalTrialDTO>>;
}
