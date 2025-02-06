using Domain.Entities;
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
    ) : IRequest<ClinicalTrial>;
}
