using Application.DTOMappers;
using Application.DTOs;
using Application.Exceptions;
using Application.Queries;
using Application.Responses;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers.QueryHandlers
{
    public class GetTrialByStatusHandler : IRequestHandler<GetTrialByStatusQuery, PaginatedResponse<ClinicalTrialDTO>>
    {
        private readonly IQueryRepository<ClinicalTrial> _queryRepository;
        private readonly ILogger _logger;

        public GetTrialByStatusHandler(IQueryRepository<ClinicalTrial> queryRepository, ILogger<GetTrialByStatusHandler> logger)
        {
            _queryRepository = queryRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ClinicalTrialDTO>> Handle(GetTrialByStatusQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return new PaginatedResponse<ClinicalTrialDTO>(
                    ClinicalTrialDTOMapper.EntitiesToDTOs(await _queryRepository.GetByStatusAsync(request.status.ToString(), request.page, request.size)),
                    await _queryRepository.GetCountByStatusAsync(request.status.ToString()),
                    request.page,
                    request.size
                );                    
            }
            catch (RepositoryException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occured in handler: {nameof(GetTrialByStatusHandler)}");
                throw;
            }
        }
    }
}
