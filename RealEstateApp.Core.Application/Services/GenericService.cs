using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;
using System.Linq.Expressions;

namespace RealEstateApp.Core.Application.Services
{
    public class GenericService<TSaveViewModel, TViewModel, TEntity>(IGenericRepository<TEntity> repository, IMapper mapper) 
<<<<<<< HEAD
        : Interfaces.IGenericService<TSaveViewModel, TViewModel, TEntity>
=======
        : IGenericService<TSaveViewModel, TViewModel, TEntity>
>>>>>>> 7066142a1bf1513e7928d18cd381eb409d7b6bf2
        where TSaveViewModel : class
        where TViewModel : class
        where TEntity : class
    {
        protected readonly IGenericRepository<TEntity> _repository = repository;
        protected readonly IMapper _mapper = mapper;

        public virtual async Task<TSaveViewModel> Add(TSaveViewModel vm)
        {
            TEntity entity = _mapper.Map<TEntity>(vm);
            entity = await _repository.AddAsync(entity);
            return _mapper.Map<TSaveViewModel>(entity);
        }

        public virtual async Task Delete(int id)
        {
            TEntity? entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"El registro con el ID {id} no fue encontrado.");
            await _repository.DeleteAsync(entity);
        }

        public virtual async Task<List<TViewModel>> GetAllViewModel()
        {
            var entityList = await _repository.GetAllAsync();
            return _mapper.Map<List<TViewModel>>(entityList);
        }

        public virtual async Task<TViewModel> GetByIdViewModel(int id)
        {
            TEntity? entity = await _repository.GetByIdAsync(id);

            return entity is null ? throw new KeyNotFoundException($"El registro con el ID {id} no fue encontrado.")
                : _mapper.Map<TViewModel>(entity);
        }

        public virtual async Task Update(TSaveViewModel vm, int id)
        {
            TEntity entity = _mapper.Map<TEntity>(vm);
            await _repository.UpdateAsync(entity);
        }
        public virtual async Task<List<TViewModel>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entityList = await _repository.FindAsync(predicate);
            return _mapper.Map<List<TViewModel>>(entityList);
        }
    }
}


