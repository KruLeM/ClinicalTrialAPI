using Application.DTOMappers;
using Application.DTOs;
using Application.Exceptions;
using Application.Queries;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers.QueryHandlers
{
    public class GetTrialByTrialIdHandler : IRequestHandler<GetTrialByTrialIdQuery, ClinicalTrialDTO>
    {
        private readonly IQueryRepository<ClinicalTrial> _queryRepository;
        private readonly ILogger _logger;
        public GetTrialByTrialIdHandler(IQueryRepository<ClinicalTrial> queryRepository, ILogger<GetTrialByTrialIdHandler> logger)
        {
            _queryRepository = queryRepository;
            _logger = logger;
        }

        public async Task<ClinicalTrialDTO> Handle(GetTrialByTrialIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return ClinicalTrialDTOMapper.EntityToDTO(await _queryRepository.GetByIdAsync(request.trialId));
            }
            catch (RepositoryException dbEx)
            {
                _logger.LogError(dbEx, $"Exception occurred while retrieving data in handler: {nameof(GetTrialByTrialIdHandler)}.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occured in handler: {nameof(GetTrialByTrialIdHandler)}");
                throw;
            }
        }
    }
}
