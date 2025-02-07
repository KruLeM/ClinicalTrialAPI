using Application.DTOMappers;
using Application.DTOs;
using Application.Queries;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers.QueryHandlers
{
    public class GetTrialsHandler : IRequestHandler<GetTrialsQuery, IEnumerable<ClinicalTrialDTO>>
    {
        private readonly IQueryRepository<ClinicalTrial> _queryRepository;
        private readonly ILogger _logger;
        public GetTrialsHandler(IQueryRepository<ClinicalTrial> queryRepository, ILogger<GetTrialsHandler> logger)
        {
            _queryRepository = queryRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ClinicalTrialDTO>> Handle(GetTrialsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return ClinicalTrialDTOMapper.EntitiesToDTOs(await _queryRepository.GetAllAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occured in handler: {nameof(GetTrialsHandler)}");
                throw;
            }
        }
    }
}
