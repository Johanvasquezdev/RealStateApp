using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface PropertyTypeService : GenericService<SavePropertyTypeViewModel, PropertyTypeViewModel, PropertyType>
    {
    }
}
