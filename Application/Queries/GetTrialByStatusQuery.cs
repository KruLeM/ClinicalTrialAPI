using Application.DTOs;
using Application.Returns;
using MediatR;

namespace Application.Queries
{
    public record GetTrialByStatusQuery(
        string status,
        int? page,
        int? size
    )  : IRequest<PaginatedResponse<ClinicalTrialDTO>>;
}
