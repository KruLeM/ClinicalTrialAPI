using Application.DTOs;
using Application.Responses;
using MediatR;

namespace Application.Queries
{
    public record GetTrialByStatusQuery(
        string status,
        int? page,
        int? size
    )  : IRequest<PaginatedResponse<ClinicalTrialDTO>>;
}
