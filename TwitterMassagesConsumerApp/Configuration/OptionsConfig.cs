using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitterMassagesConsumerApp.Models;

namespace TwitterMassagesConsumerApp.Configuration;

internal static class OptionsConfig
{
    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection("App:RabbitMq"));
    }
}