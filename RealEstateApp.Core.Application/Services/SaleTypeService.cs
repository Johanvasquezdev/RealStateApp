using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.SaleType;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class SaleTypeService(IGenericRepository<SaleType> repository, IGenericRepository<Property> propertyRepository, IMapper mapper)
    : GenericService<SaveSaleTypeViewModel, SaleTypeViewModel, SaleType>(repository, mapper), ISaleTypeService
    {
        private readonly IGenericRepository<Property> _propertyRepository = propertyRepository;
        public override async Task Delete(int id)
        {
            var saleType = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("El tipo de venta solicitado no existe.");

            var affected = await _propertyRepository.FindAsync(p => p.SaleTypeId == id);
            foreach (var property in affected)
            {
                await _propertyRepository.DeleteAsync(property);
            }

            await _repository.DeleteAsync(saleType);
        }
    }
}