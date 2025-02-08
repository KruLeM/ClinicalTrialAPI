using Application.DTOMappers;
using Application.DTOs;
using Application.Exceptions;
using Application.Queries;
using Application.Returns;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers.QueryHandlers
{
    public class GetTrialsHandler : IRequestHandler<GetTrialsQuery, PaginatedResponse<ClinicalTrialDTO>>
    {
        private readonly IQueryRepository<ClinicalTrial> _queryRepository;
        private readonly ILogger _logger;
        public GetTrialsHandler(IQueryRepository<ClinicalTrial> queryRepository, ILogger<GetTrialsHandler> logger)
        {
            _queryRepository = queryRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ClinicalTrialDTO>> Handle(GetTrialsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return new PaginatedResponse<ClinicalTrialDTO>(
                    ClinicalTrialDTOMapper.EntitiesToDTOs(await _queryRepository.GetAllAsync(request.page, request.size)),
                    await _queryRepository.GetAllCountAsync(),
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
                _logger.LogError(ex, $"Exception occured in handler: {nameof(GetTrialsHandler)}");
                throw;
            }
        }
    }
}
