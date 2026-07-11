using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.Services;

namespace RealEstateApp.Core.Application.IoC;

public static class ServiceRegistration
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddTransient(typeof(IGenericService<,,>), typeof(GenericService<,,>)); 
        services.AddTransient<IImprovementService, ImprovementService>();
        services.AddTransient<IPropertyTypeService, PropertyTypeService>();
        services.AddTransient<ISaleTypeService, SaleTypeService>();
    }
}