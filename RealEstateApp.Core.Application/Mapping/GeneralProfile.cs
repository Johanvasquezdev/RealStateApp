using AutoMapper;
using RealEstateApp.Core.Application.ViewModels.Improvement;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Application.ViewModels.Propiedad;
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

        CreateMap<Property, PropiedadViewModel>()
            .ForMember(d => d.PropertyType, o => o.MapFrom(s => s.PropertyType!.Name))
            .ForMember(d => d.SaleType, o => o.MapFrom(s => s.SaleType!.Name))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.ImagenPrincipal, o => o.Ignore())
            .ForMember(d => d.Imagenes, o => o.Ignore())
            .ForMember(d => d.Mejoras, o => o.Ignore());
    }
}
