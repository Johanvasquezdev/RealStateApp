using RealEstateApp.Core.Application.ViewModels.SaleType;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface ISaleTypeService : IGenericService<SaveSaleTypeViewModel, SaleTypeViewModel, SaleType> 
    {

    }
}
