using AutoMapper;
using RealEstateApp.Core.Application.ViewModels.Improvement;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Application.ViewModels.SaleType;
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
    }
}