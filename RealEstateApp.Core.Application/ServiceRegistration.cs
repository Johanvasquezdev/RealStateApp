using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.Services;

namespace RealEstateApp.Core.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ServiceRegistration).Assembly);

        services.AddTransient(typeof(IGenericService<,,>), typeof(GenericService<,,>));
        services.AddTransient<IImprovementService, ImprovementService>();
        services.AddTransient<IPropertyTypeService, PropertyTypeService>();
        services.AddTransient<ISaleTypeService, SaleTypeService>();

        services.AddTransient<IAgenteService, AgenteService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IPropiedadService, PropiedadService>();
        services.AddTransient<IPerfilService, PerfilService>();
        services.AddTransient<IOfertaService, OfertaService>();
        services.AddTransient<IChatService, ChatService>();

        return services;
    }
}
