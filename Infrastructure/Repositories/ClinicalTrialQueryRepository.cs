using Application.Exceptions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClinicalTrialQueryRepository : IQueryRepository<ClinicalTrial>
    {
        private readonly AppDbContext _dbContext;
        public ClinicalTrialQueryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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
                throw new RepositoryException("Error occurred while getting data from db.", ex);
            }
        }
    }
}
