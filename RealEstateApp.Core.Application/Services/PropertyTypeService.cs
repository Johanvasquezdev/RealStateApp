using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class PropertyTypeService(IGenericRepository<PropertyType> repository, IGenericRepository<Property> propertyRepository, IMapper mapper)
    : GenericService<SavePropertyTypeViewModel, PropertyTypeViewModel, PropertyType>(repository, mapper), IPropertyTypeService
    {
        private readonly IGenericRepository<Property> _propertyRepository = propertyRepository;

        public override async Task<SavePropertyTypeViewModel> Add(SavePropertyTypeViewModel vm)
        {
            if (await _repository.AnyAsync(x => x.Name == vm.Name))
                throw new InvalidOperationException("Ya existe un tipo de propiedad con este nombre.");
            return await base.Add(vm);
        }
        public override async Task Delete(int id)
        {
            var propertyType = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("El tipo de propiedad solicitado no existe.");

            var affected = await _propertyRepository.FindAsync(p => p.PropertyTypeId == id);
            foreach (var property in affected)
            {
                await _propertyRepository.DeleteAsync(property);
            }
            await _propertyRepository.SaveChangesAsync();

            await _repository.DeleteAsync(propertyType);
            await _repository.SaveChangesAsync();
        }
        public override async Task<List<PropertyTypeViewModel>> GetAllViewModel()
        {
            var list = await _repository.GetAllWithIncludeAsync(["Properties"]);

            return list.Select(pt => new PropertyTypeViewModel
            {
                Id = pt.Id,
                Name = pt.Name,
                Description = pt.Description,
                PropertiesCount = pt.Properties?.Count ?? 0
            }).ToList();
        }
    }
}