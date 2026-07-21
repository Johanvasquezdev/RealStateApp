using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.SaleType;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class SaleTypeService(IGenericRepository<SaleType> repository, IGenericRepository<Property> propertyRepository, IMapper mapper)
    : GenericService<SaveSaleTypeViewModel, SaleTypeViewModel, SaleType>(repository, mapper), ISaleTypeService
    {
        private readonly IGenericRepository<Property> _propertyRepository = propertyRepository;

        public override async Task<SaveSaleTypeViewModel> Add(SaveSaleTypeViewModel vm)
        {
            if (await _repository.AnyAsync(x => x.Name == vm.Name))
                throw new InvalidOperationException("Ya existe un tipo de venta con este nombre.");
            return await base.Add(vm);
        }

        public override async Task Update(SaveSaleTypeViewModel vm, int id)
        {
            if (await _repository.AnyAsync(x => x.Name == vm.Name && x.Id != id))
                throw new InvalidOperationException("Ya existe un tipo de venta con este nombre.");
            await base.Update(vm, id);
        }
        public override async Task Delete(int id)
        {
            var saleType = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("El tipo de venta solicitado no existe.");

            var affected = await _propertyRepository.FindAsync(p => p.SaleTypeId == id);
            if (affected.Any())
            {
                throw new InvalidOperationException("No se puede eliminar porque existen propiedades asociadas a este tipo de venta.");
            }

            await _repository.DeleteAsync(saleType);
            await _repository.SaveChangesAsync();
        }
        public override async Task<List<SaleTypeViewModel>> GetAllViewModel()
        {
            var list = await _repository.GetAllWithIncludeAsync(["Properties"]);

            return list.Select(pt => new SaleTypeViewModel
            {
                Id = pt.Id,
                Name = pt.Name,
                Description = pt.Description,
                PropertiesCount = pt.Properties?.Count ?? 0
            }).ToList();
        }

        public override async Task<SaleTypeViewModel> GetByIdViewModel(int id)
        {
            var entity = await _repository.FirstOrDefaultWithIncludesAsync(x => x.Id == id, "Properties");

            if (entity is null)
                throw new KeyNotFoundException($"El registro con el ID {id} no fue encontrado.");

            var vm = _mapper.Map<SaleTypeViewModel>(entity);
            vm.PropertiesCount = entity.Properties?.Count ?? 0;

            return vm;
        }
    }
}