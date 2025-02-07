using Application.DTOs;
using MediatR;

namespace Application.Commands
{
    public record AddTrialCommand (
        string TrialId,
        string Title,
        DateOnly StartDate,
        DateOnly? EndDate,
        int Participants,
        string Status
    ) : IRequest<ClinicalTrialDTO>;
}
