namespace Domain.Repositories
{
    public interface IQueryRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(int? page, int? size);
        Task<int> GetAllCountAsync();
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetByStatusAsync(string status);
    }
}
