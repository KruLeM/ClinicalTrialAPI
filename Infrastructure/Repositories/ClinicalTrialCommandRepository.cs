using Application.Exceptions;
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
            try
            {
                var result = await _dbContext.ClinicalTrials.AddAsync(trial);
                await _dbContext.SaveChangesAsync();

                return result.Entity;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding a trial", ex);
            }
        }
        public async Task<ClinicalTrial> UpdateTrialAsync(ClinicalTrial trial)
        {
            try
            {
                _dbContext.ClinicalTrials.Update(trial);
                await _dbContext.SaveChangesAsync();

                return trial;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating a trial.", ex);
            }
        }
    }
}
