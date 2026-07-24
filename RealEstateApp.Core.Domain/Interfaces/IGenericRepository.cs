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
    Task<List<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params string[] includeProperties);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultWithIncludesAsync(Expression<Func<T, bool>> predicate, params string[] includeProperties);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    Task SaveChangesAsync();
}
