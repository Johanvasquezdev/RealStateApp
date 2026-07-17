using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class PropertyTypeService(IGenericRepository<PropertyType> repository, IMapper mapper)
    : GenericService<SavePropertyTypeViewModel, PropertyTypeViewModel, PropertyType>(repository, mapper), Interfaces.IPropertyTypeService
    {
        private readonly IGenericRepository<Property> _propertyRepository = propertyRepository;
        public override async Task Delete(int id)
        {
            var propertyType = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("El tipo de propiedad solicitado no existe.");

            var affected = await _propertyRepository.FindAsync(p => p.PropertyTypeId == id);
            foreach (var property in affected)
            {
                await _propertyRepository.DeleteAsync(property);
            }

            await _repository.DeleteAsync(propertyType);
        }
    }
}
