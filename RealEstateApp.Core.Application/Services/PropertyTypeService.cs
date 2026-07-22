using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Application.Services
{
    public class PropertyTypeService(
        IGenericRepository<PropertyType> repository, 
        IGenericRepository<Property> propertyRepository, 
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    : GenericService<SavePropertyTypeViewModel, PropertyTypeViewModel, PropertyType>(repository, mapper), IPropertyTypeService
    {
        private readonly IGenericRepository<Property> _propertyRepository = propertyRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IFileStorageService _fileStorageService = fileStorageService;

        public override async Task<SavePropertyTypeViewModel> Add(SavePropertyTypeViewModel vm)
        {
            if (await _repository.AnyAsync(x => x.Name == vm.Name))
                throw new InvalidOperationException("Ya existe un tipo de propiedad con este nombre.");
            return await base.Add(vm);
        }

        public override async Task Update(SavePropertyTypeViewModel vm, int id)
        {
            if (await _repository.AnyAsync(x => x.Name == vm.Name && x.Id != id))
                throw new InvalidOperationException("Ya existe un tipo de propiedad con este nombre.");
            await base.Update(vm, id);
        }
        public override async Task Delete(int id)
        {
            var propertyType = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("El tipo de propiedad solicitado no existe.");

            var affected = await _propertyRepository.FindWithIncludesAsync(p => p.PropertyTypeId == id, "Images");
            
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var property in affected)
                    await _propertyRepository.DeleteAsync(property);

                await _repository.DeleteAsync(propertyType);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                foreach (var property in affected)
                foreach (var image in property.Images)
                    _fileStorageService.DeleteFile(image.ImageUrl, "properties");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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

        public override async Task<PropertyTypeViewModel> GetByIdViewModel(int id)
        {
            var entity = await _repository.FirstOrDefaultWithIncludesAsync(x => x.Id == id, "Properties");

            if (entity is null)
                throw new KeyNotFoundException($"El registro con el ID {id} no fue encontrado.");

            var vm = _mapper.Map<PropertyTypeViewModel>(entity);
            vm.PropertiesCount = entity.Properties?.Count ?? 0;

            return vm;
        }
    }
}