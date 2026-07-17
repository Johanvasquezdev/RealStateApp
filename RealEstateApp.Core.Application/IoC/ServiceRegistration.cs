using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces;

using RealEstateApp.Core.Application.Services;

namespace RealEstateApp.Core.Application.IoC;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ServiceRegistration).Assembly);

<<<<<<< HEAD
        services.AddTransient(typeof(Interfaces.IGenericService<,,>), typeof(Services.GenericService<,,>));
        services.AddTransient<Interfaces.IImprovementService, Services.ImprovementService>();
        services.AddTransient<Interfaces.IPropertyTypeService, Services.PropertyTypeService>();
        services.AddTransient<Interfaces.ISaleTypeService, Services.SaleTypeService>();
=======
        services.AddTransient(typeof(IGenericService<,,>), typeof(Services.GenericService<,,>));
        services.AddTransient<IImprovementService, Services.ImprovementService>();
        services.AddTransient<IPropertyTypeService, Services.PropertyTypeService>();
        services.AddTransient<ISaleTypeService, Services.SaleTypeService>();
>>>>>>> 7066142a1bf1513e7928d18cd381eb409d7b6bf2

        services.AddTransient<IAgentService, Services.AgentService>();
        services.AddTransient<IAuthService, Services.AuthService>();
        services.AddTransient<IPropertyService, PropertyService>();
        services.AddTransient<IProfileService, Services.ProfileService>();
        services.AddTransient<IOfferService, Services.OfferService>();
        services.AddTransient<IChatService, Services.ChatService>();

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

