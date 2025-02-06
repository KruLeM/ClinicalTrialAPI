using Application.Commands;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers.CommandHandlers
{
    public class UpdateTrialHandler : IRequestHandler<UpdateTrialCommand, ClinicalTrial>
    {
        private readonly ICommandRepository<ClinicalTrial> _commandRepository;
        private readonly IQueryRepository<ClinicalTrial> _queryRepository;
        private readonly ILogger _logger;

        public UpdateTrialHandler(ICommandRepository<ClinicalTrial> commandRepository, IQueryRepository<ClinicalTrial> queryRepository, ILogger logger)
        {
            _commandRepository = commandRepository;
            _queryRepository = queryRepository;
            _logger = logger;
        }

        public async Task<ClinicalTrial> Handle(UpdateTrialCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dbTrial = await _queryRepository.GetTrialAsync(request.TrialId);
                if (string.IsNullOrEmpty(dbTrial?.TrialId))
                {
                    throw new Exception("TrialId don't exists in db");
                }

                TrialStatus trialStatus = Enum.Parse<TrialStatus>(request.Status);
                DateOnly? endDate = request.EndDate;
                if (!endDate.HasValue && trialStatus == TrialStatus.Ongoing)
                    endDate = request.StartDate.AddMonths(1);

                dbTrial.TrialId = request.TrialId;
                dbTrial.Title = request.Title;
                dbTrial.StartDate = request.StartDate;
                dbTrial.EndDate = endDate;
                dbTrial.Participants = request.Participants;
                dbTrial.Status = trialStatus;
                dbTrial.Duration = endDate.HasValue ? (Convert.ToDateTime(endDate) - Convert.ToDateTime(request.StartDate)).Days : 0;

                var result = await _commandRepository.AddTrialAsync(dbTrial);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occured in handler: {nameof(UpdateTrialHandler)}");
                throw;
            }
        }

    }
}
