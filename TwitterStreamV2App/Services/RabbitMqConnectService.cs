using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using TwitterStreamV2App.Interfaces;
using TwitterStreamV2App.Models;

namespace TwitterStreamV2App.Services;

public class RabbitMqConnectService : IQueueConnectService
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly IOptionsSnapshot<RabbitMqOptions> _rabbitMqOptions;

    public RabbitMqConnectService(IRabbitMqConnectionFactory connectionFactory, IOptionsSnapshot<RabbitMqOptions> rabbitMqOptions)
    {
        _connectionFactory = connectionFactory;
        _rabbitMqOptions = rabbitMqOptions;
    }

    public IModel Connect()
    {
        var chanel = ConnectChannel(_connectionFactory);
        chanel.QueueDeclare(queue: _rabbitMqOptions.Value.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        return chanel;
    }
    
    private static IModel ConnectChannel(IRabbitMqConnectionFactory factory)
    {
        var connection = factory.CreateConnection();
        var chanel = connection.CreateModel();
        return chanel;
    }
}