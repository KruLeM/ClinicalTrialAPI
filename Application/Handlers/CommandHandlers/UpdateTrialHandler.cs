﻿using Application.Commands;
using Application.Exceptions;
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

        public UpdateTrialHandler(ICommandRepository<ClinicalTrial> commandRepository, IQueryRepository<ClinicalTrial> queryRepository, ILogger<UpdateTrialHandler> logger)
        {
            _commandRepository = commandRepository;
            _queryRepository = queryRepository;
            _logger = logger;
        }

        public async Task<ClinicalTrial> Handle(UpdateTrialCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dbTrial = await _queryRepository.GetByIdAsync(request.TrialId);
                if (string.IsNullOrEmpty(dbTrial?.TrialId))
                {
                    _logger.LogError(request.TrialId, $"Exception occured in handler: {nameof(UpdateTrialHandler)}. TrialId don't exists in db");
                    throw new TrialDataException("TrialId don't exists in db");
                }

                var trialStatus = Enum.Parse<TrialStatus>(request.Status.Replace(" ", ""));
                var endDate = request.EndDate;

                if (!CommonHandlerHelper.CheckTrialEndDateValue(request.StartDate, endDate, trialStatus, out string validationMessage))
                {
                    _logger.LogError(request.TrialId, $"Exception occured in handler: {nameof(UpdateTrialHandler)}. {validationMessage}");
                    throw new TrialDataException(validationMessage);
                }

                if (!endDate.HasValue && trialStatus == TrialStatus.Ongoing)
                {
                    endDate = request.StartDate.AddMonths(1);
                }

                dbTrial.TrialId = request.TrialId;
                dbTrial.Title = request.Title;
                dbTrial.StartDate = request.StartDate;
                dbTrial.EndDate = endDate;
                dbTrial.Participants = request.Participants;
                dbTrial.Status = trialStatus;
                dbTrial.Duration = endDate.HasValue
                                ? (endDate.Value.ToDateTime(TimeOnly.MinValue) - request.StartDate.ToDateTime(TimeOnly.MinValue)).Days
                                : 0;

                var result = await _commandRepository.UpdateTrialAsync(dbTrial);
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
