using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.Services;

namespace RealEstateApp.Core.Application.IoC;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ServiceRegistration).Assembly);

        services.AddTransient(typeof(IGenericService<,,>), typeof(GenericService<,,>));
        services.AddTransient<IImprovementService, ImprovementService>();
        services.AddTransient<IPropertyTypeService, PropertyTypeService>();
        services.AddTransient<ISaleTypeService, SaleTypeService>();

        services.AddTransient<IAgenteService, AgentService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IPropiedadService, PropertyService>();
        services.AddTransient<IPerfilService, PerfilService>();
        services.AddTransient<IOfertaService, OfferService>();
        services.AddTransient<IChatService, ChatService>();

        // Persona 2: Cliente Services
        services.AddTransient<IClientAgentService, ClientAgentService>();
        services.AddTransient<IClientFavoriteService, ClientFavoriteService>();
        services.AddTransient<IClientOfferService, ClientOfferService>();
        services.AddTransient<IClientChatService, ClientChatService>();
        services.AddTransient<IClientPropertyService, ClientPropertyService>();
        return services;
    }
}
