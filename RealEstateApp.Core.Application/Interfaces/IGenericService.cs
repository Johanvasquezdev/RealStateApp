using System.Linq.Expressions;

namespace RealEstateApp.Core.Application.Interfaces;

public interface IGenericService<TSaveViewModel, TViewModel, TEntity>
    where TSaveViewModel : class
    where TViewModel : class
    where TEntity : class
{
    Task<TSaveViewModel> Add(TSaveViewModel vm);
    Task Update(TSaveViewModel vm, int id);
    Task Delete(int id);
    Task<TViewModel> GetByIdViewModel(int id);
    Task<List<TViewModel>> GetAllViewModel();
    Task<List<TViewModel>> Find(Expression<Func<TEntity, bool>> predicate);
}
