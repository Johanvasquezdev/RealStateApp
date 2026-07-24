using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces;

using RealEstateApp.Core.Application.Services;

namespace RealEstateApp.Core.Application.IoC;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddAutoMapper(typeof(ServiceRegistration).Assembly);

        services.AddTransient(typeof(IGenericService<,,>), typeof(GenericService<,,>));
        services.AddTransient<IImprovementService, ImprovementService>();
        services.AddTransient<IPropertyTypeService, PropertyTypeService>();
        services.AddTransient<ISaleTypeService, SaleTypeService>();

        services.AddTransient<IAgentService, AgentService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IPropertyService, PropertyService>();
        services.AddTransient<IProfileService, ProfileService>();
        services.AddTransient<IOfferService, OfferService>();
        services.AddTransient<IChatService, ChatService>();

        services.AddTransient<IClientAgentService, ClientAgentService>();
        services.AddTransient<IClientFavoriteService, ClientFavoriteService>();
        services.AddTransient<IClientOfferService, ClientOfferService>();
        services.AddTransient<IClientChatService, ClientChatService>();
        services.AddTransient<IClientPropertyService, ClientPropertyService>();


        return services;
    }
}

