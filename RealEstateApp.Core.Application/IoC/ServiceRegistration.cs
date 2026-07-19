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
        services.AddTransient<Interfaces.ImprovementService, Services.ImprovementService>();
        services.AddTransient<Interfaces.PropertyTypeService, Services.PropertyTypeService>();
        services.AddTransient<Interfaces.SaleTypeService, Services.SaleTypeService>();

        services.AddTransient<Interfaces.Services.AgentService, Services.AgentService>();
        services.AddTransient<Interfaces.Services.AuthService, Services.AuthService>();
        services.AddTransient<IPropertyService, PropertyService>();
        services.AddTransient<ProfileService, PerfilService>();
        services.AddTransient<Interfaces.Services.OfferService, Services.OfferService>();
        services.AddTransient<Interfaces.Services.ChatService, Services.ChatService>();

        // Persona 2: Cliente Services
        services.AddTransient<Interfaces.Services.ClientAgentService, Services.ClientAgentService>();
        services.AddTransient<Interfaces.Services.ClientFavoriteService, Services.ClientFavoriteService>();
        services.AddTransient<Interfaces.Services.ClientOfferService, Services.ClientOfferService>();
        services.AddTransient<Interfaces.Services.ClientChatService, Services.ClientChatService>();
        services.AddTransient<Interfaces.Services.ClientPropertyService, Services.ClientPropertyService>();

        // Punch Card Services
        services.AddTransient<Interfaces.Services.SummaryPunchCardService, Services.SummaryPunchCardService>();

        return services;
    }
}
