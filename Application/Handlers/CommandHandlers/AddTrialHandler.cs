using Application.Commands;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers.CommandHandlers
{
    public class AddTrialHandler : IRequestHandler<AddTrialCommand, ClinicalTrial>
    {
        private readonly ICommandRepository<ClinicalTrial> _commandRepository;
        private readonly IQueryRepository<ClinicalTrial> _queryRepository;
        private readonly ILogger _logger;

        public AddTrialHandler(ICommandRepository<ClinicalTrial> commandRepository, IQueryRepository<ClinicalTrial> queryRepository, ILogger logger)
        {
            _commandRepository = commandRepository;
            _queryRepository = queryRepository;
            _logger = logger;
        }

        public async Task<ClinicalTrial> Handle(AddTrialCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dbTrial = await _queryRepository.GetTrialAsync(request.TrialId);
                if (!string.IsNullOrEmpty(dbTrial?.TrialId))
                {
                    throw new Exception("TrialId already exists in db");
                }

                TrialStatus trialStatus = Enum.Parse<TrialStatus>(request.Status);
                DateOnly? endDate = request.EndDate;
                if (!endDate.HasValue && trialStatus == TrialStatus.Ongoing)
                    endDate = request.StartDate.AddMonths(1);

                var trial = new ClinicalTrial
                {
                    TrialId = request.TrialId,
                    Title = request.Title,
                    StartDate = request.StartDate,
                    EndDate = endDate,
                    Participants = request.Participants,
                    Status = trialStatus,
                    Duration = endDate.HasValue ? (Convert.ToDateTime(endDate) - Convert.ToDateTime(request.StartDate)).Days : 0
                };

                var result = await _commandRepository.AddTrialAsync(trial);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occured in handler: {nameof(AddTrialHandler)}");
                throw;
            }
        }

    }
}
