using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TwitterMassagesConsumerApp;
using TwitterMassagesConsumerApp.Configuration;
using TwitterMassagesConsumerApp.Repositories;
using TwitterMassagesConsumerApp.Repositories.Interfaces;
using TwitterMassagesConsumerApp.Services;
using TwitterMassagesConsumerApp.Services.Interfaces;

var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
var config = builder.Build();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Async(wt => wt.Console())
    .CreateLogger();

using var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((_, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(config))
    .ConfigureServices((_, services) =>
    {
        OptionsConfig.Register(services, config);
        services.AddSingleton<IQueueConnectService, RabbitMqConnectService>();
        services.AddSingleton<IStorageRepository, StorageRepository>();
        services.AddTransient<IQueueService, RabbitMqService>();
        services.AddScoped<App>();
    }).Build();

await host.Services.GetService<App>()?.StartRunningAsync()!;