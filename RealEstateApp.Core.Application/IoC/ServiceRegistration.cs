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

        services.AddTransient(typeof(Interfaces.GenericService<,,>), typeof(Services.GenericService<,,>));
        services.AddTransient<ImprovementService, ImprovementService>();
        services.AddTransient<PropertyTypeService, PropertyTypeService>();
        services.AddTransient<SaleTypeService, SaleTypeService>();

        services.AddTransient<AgentService, AgentService>();
        services.AddTransient<AuthService, AuthService>();
        services.AddTransient<IPropertyService, PropertyService>();
        services.AddTransient<ProfileService, PerfilService>();
        services.AddTransient<OfferService, OfferService>();
        services.AddTransient<ChatService, ChatService>();

        // Persona 2: Cliente Services
        services.AddTransient<ClientAgentService, ClientAgentService>();
        services.AddTransient<ClientFavoriteService, ClientFavoriteService>();
        services.AddTransient<ClientOfferService, ClientOfferService>();
        services.AddTransient<ClientChatService, ClientChatService>();
        services.AddTransient<ClientPropertyService, ClientPropertyService>();

        // Punch Card Services
        services.AddTransient<SummaryPunchCardService, SummaryPunchCardService>();

        return services;
    }
}
