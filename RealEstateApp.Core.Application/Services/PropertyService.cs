using AutoMapper;
using Microsoft.AspNetCore.Http;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.AgentProperties;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;

    public PropertyService(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<AgentPropertyViewModel>> GetAvailablePropertiesAsync()
    {
        var repo = _unitOfWork.Repository<Property>();
        var properties = await repo.FindWithIncludesAsync(
            p => p.Status == PropertyStatus.Available,
            "PropertyType", "SaleType", "Images", "PropertyImprovements.Improvement");
        var sorted = properties.OrderByDescending(p => p.Created).ToList();
        return MapWithImages(sorted);
    }

    public async Task<List<AgentPropertyViewModel>> GetPropertiesByAgentAsync(string agentId)
    {
        var repo = _unitOfWork.Repository<Property>();
        var properties = await repo.FindWithIncludesAsync(
            p => p.AgentId == agentId,
            "PropertyType", "SaleType", "Images", "PropertyImprovements.Improvement");
        return MapWithImages(properties);
    }

    public async Task<List<AgentPropertyViewModel>> GetAvailablePropertiesByAgentAsync(string agentId)
    {
        var repo = _unitOfWork.Repository<Property>();
        var properties = await repo.FindWithIncludesAsync(
            p => p.AgentId == agentId && p.Status == PropertyStatus.Available,
            "PropertyType", "SaleType", "Images", "PropertyImprovements.Improvement");
        return MapWithImages(properties);
    }

    public async Task<AgentPropertySaveViewModel> GetPropertyForEditAsync(int id, string agentId)
    {
        var repo = _unitOfWork.Repository<Property>();
        var prop = await repo.FirstOrDefaultWithIncludesAsync(
            p => p.Id == id && p.AgentId == agentId && p.Status == PropertyStatus.Available,
            "Images", "PropertyImprovements");
        if (prop is null) return null!;
        return new AgentPropertySaveViewModel
        {
            Id = prop.Id,
            PropertyTypeId = prop.PropertyTypeId,
            SaleTypeId = prop.SaleTypeId,
            Price = prop.Price,
            Description = prop.Description,
            LandSize = prop.LandSize,
            Rooms = prop.Rooms,
            Bathrooms = prop.Bathrooms,
            ImprovementIds = prop.PropertyImprovements.Select(pi => pi.ImprovementId).ToList(),
            ExistingImages = prop.Images.Select(i => string.IsNullOrEmpty(i.ImageUrl) ? "" : (i.ImageUrl.StartsWith("http") || i.ImageUrl.StartsWith("/") ? i.ImageUrl : $"/Images/properties/{i.ImageUrl}")).ToList()
        };
    }

    public async Task<AgentPropertyViewModel?> GetPropertyDetailAsync(int id)
    {
        var repo = _unitOfWork.Repository<Property>();
        var prop = await repo.FirstOrDefaultWithIncludesAsync(
            p => p.Id == id,
            "PropertyType", "SaleType", "Images", "PropertyImprovements.Improvement");
        if (prop is null) return null;
        var vm = _mapper.Map<AgentPropertyViewModel>(prop);
        var mainImageUrl = prop.Images.FirstOrDefault()?.ImageUrl;
        vm.MainImage = string.IsNullOrEmpty(mainImageUrl) ? "" : (mainImageUrl.StartsWith("http") || mainImageUrl.StartsWith("/") ? mainImageUrl : $"/Images/properties/{mainImageUrl}");
        vm.Images = prop.Images.Select(i => string.IsNullOrEmpty(i.ImageUrl) ? "" : (i.ImageUrl.StartsWith("http") || i.ImageUrl.StartsWith("/") ? i.ImageUrl : $"/Images/properties/{i.ImageUrl}")).ToList();
        vm.Improvements = prop.PropertyImprovements.Select(pi => pi.Improvement.Name).ToList();
        return vm;
    }

    public async Task<int> CreateAsync(AgentPropertySaveViewModel vm, string agentId)
    {
        var code = await GenerateUniqueCodeAsync();
        var propertyRepo = _unitOfWork.Repository<Property>();
        var improvementRepo = _unitOfWork.Repository<PropertyImprovement>();
        var imageRepo = _unitOfWork.Repository<PropertyImage>();

        var entity = new Property
        {
            Code = code,
            Price = vm.Price,
            Description = vm.Description,
            LandSize = vm.LandSize,
            Rooms = vm.Rooms,
            Bathrooms = vm.Bathrooms,
            PropertyTypeId = vm.PropertyTypeId,
            SaleTypeId = vm.SaleTypeId,
            AgentId = agentId,
            Status = PropertyStatus.Available
        };

        await propertyRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        if (vm.ImprovementIds.Any())
        {
            foreach (var impId in vm.ImprovementIds)
                await improvementRepo.AddAsync(new PropertyImprovement { PropertyId = entity.Id, ImprovementId = impId });
        }

        if (vm.Images is not null)
        {
            foreach (var img in vm.Images)
            {
                var url = _fileStorageService.UploadFile(img, "properties", false, "");
                await imageRepo.AddAsync(new PropertyImage { ImageUrl = url, PropertyId = entity.Id });
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(AgentPropertySaveViewModel vm, string agentId)
    {
        var propertyRepo = _unitOfWork.Repository<Property>();
        var improvementRepo = _unitOfWork.Repository<PropertyImprovement>();
        var imageRepo = _unitOfWork.Repository<PropertyImage>();

        var entity = await propertyRepo.FirstOrDefaultWithIncludesAsync(
            p => p.Id == vm.Id && p.AgentId == agentId && p.Status == PropertyStatus.Available,
            "Images", "PropertyImprovements");
        if (entity is null) return;

        entity.Price = vm.Price;
        entity.Description = vm.Description;
        entity.LandSize = vm.LandSize;
        entity.Rooms = vm.Rooms;
        entity.Bathrooms = vm.Bathrooms;
        entity.PropertyTypeId = vm.PropertyTypeId;
        entity.SaleTypeId = vm.SaleTypeId;

        var existingImprovements = await improvementRepo.FindAsync(pi => pi.PropertyId == entity.Id);
        foreach (var imp in existingImprovements)
            await improvementRepo.DeleteAsync(imp);

        foreach (var impId in vm.ImprovementIds)
            await improvementRepo.AddAsync(new PropertyImprovement { PropertyId = entity.Id, ImprovementId = impId });

        if (vm.ImagesToDelete != null && vm.ImagesToDelete.Any())
        {
            foreach (var urlToDelete in vm.ImagesToDelete)
            {
                var imgEntity = entity.Images.FirstOrDefault(i => i.ImageUrl == urlToDelete);
                if (imgEntity != null)
                {
                    _fileStorageService.DeleteFile(imgEntity.ImageUrl, "properties");
                    await imageRepo.DeleteAsync(imgEntity);
                }
            }
        }

        if (vm.Images is not null && vm.Images.Any())
        {
            foreach (var img in vm.Images)
            {
                var url = _fileStorageService.UploadFile(img, "properties", false, "");
                await imageRepo.AddAsync(new PropertyImage { ImageUrl = url, PropertyId = entity.Id });
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string agentId)
    {
        var propertyRepo = _unitOfWork.Repository<Property>();
        var entity = await propertyRepo.FirstOrDefaultWithIncludesAsync(
            p => p.Id == id && p.AgentId == agentId && p.Status == PropertyStatus.Available,
            "Images");
        if (entity is null) return;

        foreach (var img in entity.Images)
        {
            _fileStorageService.DeleteFile(img.ImageUrl, "properties");
        }

        await propertyRepo.DeleteAsync(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<string?> GetFirstImageAsync(int propertyId)
    {
        var imageRepo = _unitOfWork.Repository<PropertyImage>();
        var img = await imageRepo.FirstOrDefaultAsync(i => i.PropertyId == propertyId);
        return img?.ImageUrl;
    }

    private async Task<string> GenerateUniqueCodeAsync()
    {
        var repo = _unitOfWork.Repository<Property>();
        var random = new Random();
        string code;
        do
        {
            code = random.Next(100000, 999999).ToString();
        } while (await repo.AnyAsync(p => p.Code == code));
        return code;
    }



    private List<AgentPropertyViewModel> MapWithImages(List<Property> properties)
    {
        var list = _mapper.Map<List<AgentPropertyViewModel>>(properties);
        foreach (var vm in list)
        {
            var prop = properties.First(p => p.Id == vm.Id);
            var mainImageUrl = prop.Images.FirstOrDefault()?.ImageUrl;
            vm.MainImage = string.IsNullOrEmpty(mainImageUrl) ? "" : (mainImageUrl.StartsWith("http") || mainImageUrl.StartsWith("/") ? mainImageUrl : $"/Images/properties/{mainImageUrl}");
        }
        return list;
    }
}



