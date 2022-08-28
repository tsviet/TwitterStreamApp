using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using TwitterStreamV2App.Interfaces;
using TwitterStreamV2App.Models;

namespace TwitterStreamV2App.Services;

public class RabbitMqConnection : IRabbitMqConnectionFactory
{
    private readonly IOptionsSnapshot<RabbitMqOptions> _rabbitMqOptions;

    public RabbitMqConnection(IOptionsSnapshot<RabbitMqOptions> rabbitMqOptions)
    {
        _rabbitMqOptions = rabbitMqOptions;
    }

    public IConnection CreateConnection() {
        var factory = new ConnectionFactory {
            HostName = _rabbitMqOptions.Value.Server,
        };
        var connection = factory.CreateConnection();
        return connection;
    }
}