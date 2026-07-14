using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.SaleType;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class SaleTypeService(IGenericRepository<SaleType> repository, IMapper mapper)
    : GenericService<SaveSaleTypeViewModel, SaleTypeViewModel, SaleType>(repository, mapper), ISaleTypeService
    {
    }
}
