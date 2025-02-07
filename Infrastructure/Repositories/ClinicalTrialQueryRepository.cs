using Application.Exceptions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class ClinicalTrialQueryRepository : IQueryRepository<ClinicalTrial>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public ClinicalTrialQueryRepository(AppDbContext dbContext, ILogger<ClinicalTrialQueryRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<ClinicalTrial>> GetAllAsync()
        {
            try
            {
                return await _dbContext.ClinicalTrials
                .AsNoTracking()
                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while retrieving data. Repository: {nameof(ClinicalTrialQueryRepository)}, method: {nameof(GetAllAsync)}.");
                throw new RepositoryException("Error occurred while getting data from db.", ex);
            }
        }

        public async Task<ClinicalTrial> GetByIdAsync(string trialId)
        {
            try
            {
                return await _dbContext.ClinicalTrials
                        .Where(ct => ct.TrialId == trialId)
                        .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while retrieving data. Repository: {nameof(ClinicalTrialQueryRepository)}, method: {nameof(GetByIdAsync)}.");
                throw new RepositoryException("Error occurred while getting data from db.", ex);
            }
        }

        public async Task<IEnumerable<ClinicalTrial>> GetByStatusAsync(string trialStatus)
        {
            try
            {
                var trialStatusEnum = Enum.Parse<TrialStatus>(trialStatus.Replace(" ", ""));
                return await _dbContext.ClinicalTrials
                    .Where(ct => ct.Status == trialStatusEnum)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while retrieving data. Repository: {nameof(ClinicalTrialQueryRepository)}, method: {nameof(GetByStatusAsync)}.");
                throw new RepositoryException("Error occurred while getting data from db.", ex);
            }
        }
    }
}
