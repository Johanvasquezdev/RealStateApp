using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Improvement;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Application.Services
{
    public class ImprovementService(
        IGenericRepository<Improvement> repository, 
        IGenericRepository<PropertyImprovement> linkRepository, 
        IMapper mapper,
        IUnitOfWork unitOfWork)
    : GenericService<SaveImprovementViewModel, ImprovementViewModel, Improvement>(repository, mapper), IImprovementService
    {
        private readonly IGenericRepository<PropertyImprovement> _linkRepository = linkRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public override async Task<SaveImprovementViewModel> Add(SaveImprovementViewModel vm)
        {
            if (await _repository.AnyAsync(x => x.Name == vm.Name))
                throw new InvalidOperationException("Ya existe una mejora con este nombre.");
            return await base.Add(vm);
        }

        public override async Task Update(SaveImprovementViewModel vm, int id)
        {
            if (await _repository.AnyAsync(x => x.Name == vm.Name && x.Id != id))
                throw new InvalidOperationException("Ya existe una mejora con este nombre.");
            await base.Update(vm, id);
        }
        public override async Task Delete(int id)
        {
            var improvement = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("La mejora solicitada no existe.");

            var affectedLinks = await _linkRepository.FindAsync(l => l.ImprovementId == id);
            
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var link in affectedLinks)
                {
                    await _linkRepository.DeleteAsync(link);
                }

                await _repository.DeleteAsync(improvement);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public override async Task<List<ImprovementViewModel>> GetAllViewModel()
        {
            var list = await _repository.GetAllWithIncludeAsync(["PropertyImprovements"]);

            return list.Select(pt => new ImprovementViewModel
            {
                Id = pt.Id,
                Name = pt.Name,
                Description = pt.Description,
                PropertiesCount = pt.PropertyImprovements?.Count ?? 0
            }).ToList();
        }

        public override async Task<ImprovementViewModel> GetByIdViewModel(int id)
        {
            var entity = await _repository.FirstOrDefaultWithIncludesAsync(x => x.Id == id, "PropertyImprovements");

            if (entity is null)
                throw new KeyNotFoundException($"El registro con el ID {id} no fue encontrado.");

            var vm = _mapper.Map<ImprovementViewModel>(entity);
            vm.PropertiesCount = entity.PropertyImprovements?.Count ?? 0;

            return vm;
        }
    }
}