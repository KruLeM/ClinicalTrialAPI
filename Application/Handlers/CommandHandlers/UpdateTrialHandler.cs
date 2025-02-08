using Application.Commands;
using Application.DTOMappers;
using Application.DTOs;
using Application.Exceptions;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers.CommandHandlers
{
    public class UpdateTrialHandler : IRequestHandler<UpdateTrialCommand, ClinicalTrialDTO>
    {
        private readonly ICommandRepository<ClinicalTrial> _commandRepository;
        private readonly IQueryRepository<ClinicalTrial> _queryRepository;
        private readonly ILogger _logger;

        public UpdateTrialHandler(ICommandRepository<ClinicalTrial> commandRepository, IQueryRepository<ClinicalTrial> queryRepository, ILogger<UpdateTrialHandler> logger)
        {
            _commandRepository = commandRepository;
            _queryRepository = queryRepository;
            _logger = logger;
        }

        public async Task<ClinicalTrialDTO> Handle(UpdateTrialCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dbTrial = await _queryRepository.GetByIdAsync(request.TrialId);
                if (string.IsNullOrEmpty(dbTrial?.TrialId))
                {
                    throw new TrialDataException("TrialId does not exists in database");
                }

                var trialStatus = Enum.Parse<TrialStatus>(request.Status.Replace(" ", ""));
                var endDate = request.EndDate;

                string validationMessage = "";
                if (!CommonHandlerHelper.CheckTrialEndDateValue(request.StartDate, endDate, trialStatus, out validationMessage)
                    || !CommonHandlerHelper.CheckTrialIdLength(request.TrialId, out validationMessage))
                {
                    throw new TrialDataException(validationMessage);
                }

                if (!endDate.HasValue && trialStatus == TrialStatus.Ongoing)
                {
                    endDate = request.StartDate.AddMonths(1);
                }

                dbTrial.TrialId = request.TrialId;
                dbTrial.Title = request.Title;
                dbTrial.StartDate = request.StartDate;
                dbTrial.EndDate = request.EndDate;
                dbTrial.Participants = request.Participants;
                dbTrial.Status = trialStatus;
                dbTrial.Duration = endDate.HasValue
                                ? (endDate.Value.ToDateTime(TimeOnly.MinValue) - request.StartDate.ToDateTime(TimeOnly.MinValue)).Days
                                : 0;

                return ClinicalTrialDTOMapper.EntityToDTO(await _commandRepository.UpdateTrialAsync(dbTrial));
            }
            catch (RepositoryException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occured in handler: {nameof(UpdateTrialHandler)}");
                throw;
            }
        }

    }
}
