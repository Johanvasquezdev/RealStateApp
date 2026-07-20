using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Settings;
using RealEstateApp.Infrastructure.Shared.Services;

namespace RealEstateApp.Infrastructure.Shared.IoC;

public static class ServiceRegistration
{
    public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
        services.Configure<SupabaseSettings>(configuration.GetSection("SupabaseSettings"));
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IFileStorageService, SupabaseStorageService>();
    }
}

