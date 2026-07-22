using AutoMapper;
using RealEstateApp.Core.Application.ViewModels.Improvement;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Application.ViewModels.AgentProperties;
using RealEstateApp.Core.Application.ViewModels.SaleType;
using RealEstateApp.Core.Application.ViewModels.Properties;

using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mapping;

public class GeneralProfile : Profile
{
    public GeneralProfile()
    {

        CreateMap<Improvement, ImprovementViewModel>().ReverseMap();
        CreateMap<Improvement, SaveImprovementViewModel>().ReverseMap();

        CreateMap<PropertyType, PropertyTypeViewModel>().ReverseMap();
        CreateMap<PropertyType, SavePropertyTypeViewModel>().ReverseMap();

        CreateMap<SaleType, SaleTypeViewModel>().ReverseMap();
        CreateMap<SaleType, SaveSaleTypeViewModel>().ReverseMap();

        CreateMap<Property, AgentPropertyViewModel>()
            .ForMember(d => d.PropertyType, o => o.MapFrom(s => s.PropertyType!.Name))
            .ForMember(d => d.SaleType, o => o.MapFrom(s => s.SaleType!.Name))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.MainImage, o => o.Ignore())
            .ForMember(d => d.Images, o => o.Ignore())
            .ForMember(d => d.Improvements, o => o.Ignore());

        CreateMap<Property, ClientPropertyViewModel>()
            .ForMember(d => d.PropertyTypeName, o => o.MapFrom(s => s.PropertyType != null ? s.PropertyType.Name : string.Empty))
            .ForMember(d => d.SaleTypeName, o => o.MapFrom(s => s.SaleType != null ? s.SaleType.Name : string.Empty))
            .ForMember(d => d.AgentName, o => o.Ignore())
            .ForMember(d => d.AgentPhotoUrl, o => o.Ignore())
            .ForMember(d => d.MainImageUrl, o => o.Ignore());

        CreateMap<Property, ClientPropertyDetailViewModel>()
            .ForMember(d => d.PropertyTypeName, o => o.MapFrom(s => s.PropertyType != null ? s.PropertyType.Name : string.Empty))
            .ForMember(d => d.SaleTypeName, o => o.MapFrom(s => s.SaleType != null ? s.SaleType.Name : string.Empty))
            .ForMember(d => d.Improvements, o => o.MapFrom(s => s.PropertyImprovements.Select(pi => pi.Improvement!.Name).ToList()))
            .ForMember(d => d.AgentName, o => o.Ignore())
            .ForMember(d => d.AgentPhone, o => o.Ignore())
            .ForMember(d => d.AgentEmail, o => o.Ignore())
            .ForMember(d => d.AgentPhotoUrl, o => o.Ignore())
            .ForMember(d => d.ImageUrls, o => o.Ignore());

        CreateMap<Property, RealEstateApp.Core.Application.DTOs.Properties.PropertyListApiResponse>()
            .ForMember(d => d.PropertyType, o => o.MapFrom(s => s.PropertyType != null ? s.PropertyType.Name : string.Empty))
            .ForMember(d => d.SaleType, o => o.MapFrom(s => s.SaleType != null ? s.SaleType.Name : string.Empty))
            .ForMember(d => d.MainImageUrl, o => o.MapFrom(s => s.Images.FirstOrDefault() != null ? s.Images.FirstOrDefault()!.ImageUrl : string.Empty));

        CreateMap<Property, RealEstateApp.Core.Application.DTOs.Properties.PropertyApiResponse>()
            .ForMember(d => d.PropertyType, o => o.MapFrom(s => s.PropertyType != null ? s.PropertyType.Name : string.Empty))
            .ForMember(d => d.SaleType, o => o.MapFrom(s => s.SaleType != null ? s.SaleType.Name : string.Empty))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Images, o => o.MapFrom(s => s.Images.Select(i => i.ImageUrl).ToList()))
            .ForMember(d => d.Improvements, o => o.MapFrom(s => s.PropertyImprovements.Select(pi => pi.Improvement!.Name).ToList()))
            .ForMember(d => d.AgentName, o => o.Ignore());
    }
}
