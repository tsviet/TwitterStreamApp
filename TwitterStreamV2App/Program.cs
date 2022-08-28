using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TwitterStreamV2App;
using TwitterStreamV2App.Configuration;
using TwitterStreamV2App.Interfaces;
using TwitterStreamV2App.Services;

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
        services.AddTransient<IQueueConnectService, RabbitMqConnectService>();
        services.AddTransient<IRestClientService, RestClientService>();
        services.AddTransient<ITwitterStreamService, TwitterStreamService>();
        services.AddTransient<IQueueService, RabbitMqService>();
        services.AddTransient<IRabbitMqConnectionFactory, RabbitMqConnection>();
        services.AddScoped<App>();
    }).Build();

await host.Services.GetService<App>()?.StartRunningAsync()!;