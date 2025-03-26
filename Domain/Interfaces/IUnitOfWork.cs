using Domain.Interfaces;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync();
    }
}
