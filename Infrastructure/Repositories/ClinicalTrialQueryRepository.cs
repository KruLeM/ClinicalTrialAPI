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

        public async Task<IEnumerable<ClinicalTrial>> GetAllTrialsAsync()
        {
            return await _dbContext.ClinicalTrials.AsNoTracking().ToListAsync();
        }

        public async Task<ClinicalTrial> GetTrialAsync(string trialId)
        {
            return await _dbContext.ClinicalTrials
                .FirstOrDefaultAsync(x => x.TrialId == trialId);
        }

        //ToDo -> i saw someone doing this with IQueryable...
        public async Task<IEnumerable<ClinicalTrial>> GetTrialsAsync(TrialStatus trialStatus)
        {
            IQueryable<ClinicalTrial> query = _dbContext.ClinicalTrials.Where(x => x.Status == trialStatus);
            return await query.ToListAsync();
        }
    }
}
