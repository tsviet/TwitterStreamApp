using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitterMassagesConsumerApp;
using TwitterMassagesConsumerApp.Configuration;
using TwitterMassagesConsumerApp.Interfaces;
using TwitterMassagesConsumerApp.Services;

var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
var config = builder.Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        OptionsConfig.Register(services, config);
        services.AddSingleton<IQueueConnectService, QueueConnectService>();
        services.AddTransient<IQueueService, QueueService>();
        services.AddScoped<App>();
    }).Build();

await host.Services.GetService<App>()?.StartRunningAsync()!;