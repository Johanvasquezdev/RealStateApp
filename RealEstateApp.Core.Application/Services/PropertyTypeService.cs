using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class PropertyTypeService(IGenericRepository<PropertyType> repository, IMapper mapper)
    : GenericService<SavePropertyTypeViewModel, PropertyTypeViewModel, PropertyType>(repository, mapper), Interfaces.PropertyTypeService
    {
    }
}
