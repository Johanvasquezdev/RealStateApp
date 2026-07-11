using System.Linq.Expressions;

namespace RealEstateApp.Core.Domain.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllWithIncludeAsync(List<string> properties);

    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
}
