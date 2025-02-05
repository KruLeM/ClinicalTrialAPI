using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistance;

namespace Infrastructure.Repositories
{
    //Comment Mirko - Logika ide u application layer, sto znaci da bi Sampro Save isao u Applicaiton a ovde samo Add ili Update....
    //Comment - po konvenciji, razdvajaju se update i insert
    public class ClinicalTrialCommandRepository : ICommandRepository<ClinicalTrial>
    {
        private readonly AppDbContext _dbContext;

        public ClinicalTrialCommandRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClinicalTrial> AddTrialAsync(ClinicalTrial trial)
        {
            var result = await _dbContext.ClinicalTrials.AddAsync(trial);
            await _dbContext.SaveChangesAsync();

            return result.Entity;
        }
        public async Task<ClinicalTrial> UpdateTrialAsync(ClinicalTrial trial)
        {
            _dbContext.ClinicalTrials.Update(trial);
            await _dbContext.SaveChangesAsync();

            return trial;
        }
    }
}
