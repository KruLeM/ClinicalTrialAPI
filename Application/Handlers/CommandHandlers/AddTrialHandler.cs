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
    public class AddTrialHandler : IRequestHandler<AddTrialCommand, ClinicalTrialDTO>
    {
        private readonly ICommandRepository<ClinicalTrial> _commandRepository;
        private readonly IQueryRepository<ClinicalTrial> _queryRepository;
        private readonly ILogger _logger;

        public AddTrialHandler(ICommandRepository<ClinicalTrial> commandRepository, IQueryRepository<ClinicalTrial> queryRepository, ILogger<AddTrialHandler> logger)
        {
            _commandRepository = commandRepository;
            _queryRepository = queryRepository;
            _logger = logger;
        }

        public async Task<ClinicalTrialDTO> Handle(AddTrialCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dbTrial = await _queryRepository.GetByIdAsync(request.TrialId);
                if (!string.IsNullOrEmpty(dbTrial?.TrialId))
                {
                    throw new TrialDataException("TrialId already exists in database");
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

                var trial = new ClinicalTrial
                {
                    TrialId = request.TrialId,
                    Title = request.Title,
                    StartDate = request.StartDate,
                    EndDate = endDate,
                    Participants = request.Participants,
                    Status = trialStatus,
                    Duration = endDate.HasValue
                                ? (endDate.Value.ToDateTime(TimeOnly.MinValue) - request.StartDate.ToDateTime(TimeOnly.MinValue)).Days
                                : 0
                };

                return ClinicalTrialDTOMapper.EntityToDTO(await _commandRepository.AddTrialAsync(trial));
            }
            catch (RepositoryException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occured in handler: {nameof(AddTrialHandler)}");
                throw;
            }
        }

    }
}
