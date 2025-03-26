using Domain.Interfaces;
using MyProject.Domain.Interfaces;
using MyProject.Infrastructure.Data;
using MyProject.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private Dictionary<Type, object> _repositories;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
    }

    public IRepository<T> Repository<T>() where T : class
    {
        if (_repositories.ContainsKey(typeof(T)))
            return (IRepository<T>)_repositories[typeof(T)];

        var repository = new Repository<T>(_context);
        _repositories.Add(typeof(T), repository);
        return repository;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task RollbackAsync()
    {
        await _context.Database.RollbackTransactionAsync();
        _context.ChangeTracker.Clear();
    }

    public void Dispose() => _context?.Dispose();
}