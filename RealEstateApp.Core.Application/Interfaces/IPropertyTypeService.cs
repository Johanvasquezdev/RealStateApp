using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IPropertyTypeService : IGenericService<SavePropertyTypeViewModel, PropertyTypeViewModel, PropertyType>
    {
    }
}