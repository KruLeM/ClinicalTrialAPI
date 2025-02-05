namespace Domain.Repositories
{
    public interface ICommandRepository<T> where T : class
    {
        Task<T> AddTrialAsync(T trial);
        Task<T> UpdateTrialAsync(T trial);
    }
}
