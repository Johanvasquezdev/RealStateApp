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

        services.AddTransient(typeof(Interfaces.IGenericService<,,>), typeof(Services.GenericService<,,>));
        services.AddTransient<Interfaces.IImprovementService, Services.ImprovementService>();
        services.AddTransient<Interfaces.IPropertyTypeService, Services.PropertyTypeService>();
        services.AddTransient<Interfaces.ISaleTypeService, Services.SaleTypeService>();

        services.AddTransient<Interfaces.Services.AgentService, Services.AgentService>();
        services.AddTransient<Interfaces.Services.AuthService, Services.AuthService>();
        services.AddTransient<IPropertyService, PropertyService>();
        services.AddTransient<Interfaces.Services.ProfileService, Services.ProfileService>();
        services.AddTransient<Interfaces.Services.OfferService, Services.OfferService>();
        services.AddTransient<Interfaces.Services.ChatService, Services.ChatService>();

        // Persona 2: Cliente Services
        services.AddTransient<IClientAgentService, ClientAgentService>();
        services.AddTransient<IClientFavoriteService, ClientFavoriteService>();
        services.AddTransient<IClientOfferService, ClientOfferService>();
        services.AddTransient<IClientChatService, ClientChatService>();
        services.AddTransient<IClientPropertyService, ClientPropertyService>();

        // Punch Card Services
        services.AddTransient<ISummaryPunchCardService, SummaryPunchCardService>();

        return services;
    }
}
