using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories;

public class GenericRepository<T>(ApplicationDbContext dbContext) : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await Task.CompletedTask;
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public virtual async Task<List<T>> GetAllWithIncludeAsync(List<string> properties)
    {
        var query = _dbContext.Set<T>().AsQueryable();
        foreach (var property in properties)
            query = query.Include(property);
        return await query.ToListAsync();
    }

    public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().Where(predicate).ToListAsync();
    }

    public virtual async Task<List<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params string[] includeProperties)
    {
        var query = _dbContext.Set<T>().Where(predicate);
        foreach (var property in includeProperties)
            query = query.Include(property);
        return await query.ToListAsync();
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<T?> FirstOrDefaultWithIncludesAsync(Expression<Func<T, bool>> predicate, params string[] includeProperties)
    {
        var query = _dbContext.Set<T>().Where(predicate);
        foreach (var property in includeProperties)
            query = query.Include(property);
        return await query.FirstOrDefaultAsync();
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().AnyAsync(predicate);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().CountAsync(predicate);
    }

    public virtual async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
