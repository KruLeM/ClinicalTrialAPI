using Domain.Entities;
using MediatR;

namespace Application.Commands
{
    //ToDO -> why we use Record ?
    //Comment - klasa se koristi samo za objekte koji pruzaju neku funkcionalnost
    //Comment - record: moze da bude i class i struct i immutable
    //Comment - struct : cuva se na steku (vrednosti tip podataka) 
    public record SaveTrialCommand (
        string TrialId,
        string Title,
        DateTime StartDate,
        DateTime? EndDate,
        int? Participants,
        string Status
    ) : IRequest<ClinicalTrial>;
}
