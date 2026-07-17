using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class PropertyTypeService(IGenericRepository<PropertyType> repository, IMapper mapper)
<<<<<<< HEAD
    : GenericService<SavePropertyTypeViewModel, PropertyTypeViewModel, PropertyType>(repository, mapper), Interfaces.IPropertyTypeService
=======
    : GenericService<SavePropertyTypeViewModel, PropertyTypeViewModel, PropertyType>(repository, mapper), IPropertyTypeService
>>>>>>> 7066142a1bf1513e7928d18cd381eb409d7b6bf2
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


