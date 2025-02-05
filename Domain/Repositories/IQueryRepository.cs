using Domain.Entities;

namespace Domain.Repositories
{
    public interface IQueryRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllTrialsAsync();
        Task<T> GetTrialAsync(string trialId);
        Task<IEnumerable<T>> GetTrialsAsync(TrialStatus trialStatus);

    }
}
