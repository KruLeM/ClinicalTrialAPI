using Application.Commands;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Handlers.CommandHandlers
{
    public class SaveTrialHandler : IRequestHandler<SaveTrialCommand, ClinicalTrial>
    {
        //Comment - ucitava se samo prilikom runtime
        private readonly ICommandRepository<ClinicalTrial> _commandRepository;
        private readonly IQueryRepository<ClinicalTrial> _queryRepository;
        public SaveTrialHandler(ICommandRepository<ClinicalTrial> commandRepository, IQueryRepository<ClinicalTrial> queryRepository)
        {
            _commandRepository = commandRepository;
            _queryRepository = queryRepository;
        }

        public async Task<ClinicalTrial> Handle(SaveTrialCommand request, CancellationToken cancellationToken)
        {
            var dbTrial = await _queryRepository.GetTrialAsync(request.TrialId);
            if (!string.IsNullOrEmpty(dbTrial.TrialId))
            {
                throw new Exception("TrialId already exists in db");
            }

            var trial = new ClinicalTrial
            {
                TrialId = request.TrialId,
                Title = request.Title,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Participants = request.Participants,
                Status = Enum.Parse<TrialStatus>(request.Status),
                Duration = request.EndDate.HasValue ? (request.EndDate.Value - request.StartDate).Days : 0
            };

            var result = await _commandRepository.AddTrialAsync(trial);
            return result;
        }

    }
}
