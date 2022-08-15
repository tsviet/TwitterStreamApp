using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitterStreamV2App;
using TwitterStreamV2App.Configuration;
using TwitterStreamV2App.Interfaces;
using TwitterStreamV2App.Services;

var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
var config = builder.Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        OptionsConfig.Register(services, config);
        services.AddScoped<IQueueConnectService, RabbitMqConnectService>();
        services.AddTransient<IRestClientService, RestClientService>();
        services.AddTransient<ITwitterStreamService, TwitterStreamService>();
        services.AddTransient<IQueueService, RabbitMqService>();
        services.AddScoped<App>();
    }).Build();

await host.Services.GetService<App>()?.StartRunningAsync()!;