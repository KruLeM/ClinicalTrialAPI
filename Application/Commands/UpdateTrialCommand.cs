using Application.DTOs;
using MediatR;

namespace Application.Commands
{
    public record UpdateTrialCommand(
        string TrialId,
        string Title,
        DateOnly StartDate,
        DateOnly? EndDate,
        int Participants,
        string Status
    ) : IRequest<ClinicalTrialDTO>;
}
