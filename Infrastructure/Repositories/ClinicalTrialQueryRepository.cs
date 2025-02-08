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

        public async Task<IEnumerable<ClinicalTrial>> GetAllAsync(int? page, int? size)
        {
            try
            {
                var query = _dbContext.ClinicalTrials.AsQueryable();

                if (page.HasValue && size.HasValue)
                {
                    query = query.Skip((page.Value - 1) * size.Value).Take(size.Value);
                }

                return await query
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while retrieving data. Repository: {nameof(ClinicalTrialQueryRepository)}, method: {nameof(GetAllAsync)}.");
                throw new RepositoryException("Error occurred while getting data from db.", ex);
            }
        }
        public async Task<int> GetAllCountAsync()
        {
            try
            {
                int totalCount = await _dbContext.ClinicalTrials.CountAsync();

                return totalCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while retrieving data. Repository: {nameof(ClinicalTrialQueryRepository)}, method: {nameof(GetAllCountAsync)}.");
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

        public async Task<IEnumerable<ClinicalTrial>> GetByStatusAsync(string trialStatus, int? page, int? size)
        {
            try
            {
                var query = PrepareQueryForGetTrialsByStatus(trialStatus);

                if (page.HasValue && size.HasValue)
                {
                    query = query.Skip((page.Value - 1) * size.Value).Take(size.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while retrieving data. Repository: {nameof(ClinicalTrialQueryRepository)}, method: {nameof(GetByStatusAsync)}.");
                throw new RepositoryException("Error occurred while getting data from db.", ex);
            }
        }
        public async Task<int> GetCountByStatusAsync(string trialStatus)
        {
            try
            {
                var query = PrepareQueryForGetTrialsByStatus(trialStatus);
                int totalCount = await query.CountAsync();

                return totalCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while retrieving data. Repository: {nameof(ClinicalTrialQueryRepository)}, method: {nameof(GetCountByStatusAsync)}.");
                throw new RepositoryException("Error occurred while getting data from db.", ex);
            }
        }
        private IQueryable<ClinicalTrial> PrepareQueryForGetTrialsByStatus(string trialStatus)
        {
            var trialStatusEnum = Enum.Parse<TrialStatus>(trialStatus.Replace(" ", ""));
            var query = _dbContext.ClinicalTrials.Where(ct => ct.Status == trialStatusEnum);

            return query;
        }

    }
}
