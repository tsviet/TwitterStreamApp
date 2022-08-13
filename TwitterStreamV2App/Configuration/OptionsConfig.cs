using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitterStreamV2App.Models;
using TwitterStreamV2App.Services;

namespace TwitterStreamV2App.Configuration;

internal static class OptionsConfig
{
    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TwitterOptions>(configuration.GetSection("App:Twitter"));
        services.Configure<RabbitMqOptions>(configuration.GetSection("App:RabbitMq"));
    }
}