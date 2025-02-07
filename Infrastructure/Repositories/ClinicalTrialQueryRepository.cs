﻿using Domain.Entities;
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
            return await _dbContext.ClinicalTrials
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ClinicalTrial> GetByIdAsync(string trialId)
        {
            return await _dbContext.ClinicalTrials
                .Where(ct => ct.TrialId == trialId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ClinicalTrial>> GetByStatusAsync(string trialStatus)
        {
            var trialStatusEnum = Enum.Parse<TrialStatus>(trialStatus.Replace(" ", ""));
            return await _dbContext.ClinicalTrials
                .Where(ct => ct.Status == trialStatusEnum)
                .ToListAsync();
        }
    }
}
