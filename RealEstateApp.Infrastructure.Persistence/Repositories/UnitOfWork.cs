using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private bool _disposed;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        return new GenericRepository<T>(_dbContext);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<ITransaction> BeginTransactionAsync()
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync();
        return new EfTransaction(transaction);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _dbContext.Dispose();
            _disposed = true;
        }
    }
}

internal class EfTransaction : ITransaction
{
    private readonly Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction _transaction;
    private bool _disposed;

    public EfTransaction(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task CommitAsync()
    {
        await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction.Dispose();
            _disposed = true;
        }
    }
}
