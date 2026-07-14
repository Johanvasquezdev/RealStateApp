using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Domain.Common;public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync();
    Task<ITransaction> BeginTransactionAsync();
}
