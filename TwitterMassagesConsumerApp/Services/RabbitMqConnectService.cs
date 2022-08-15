using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using TwitterMassagesConsumerApp.Models;
using TwitterMassagesConsumerApp.Services.Interfaces;

namespace TwitterMassagesConsumerApp.Services;

public class RabbitMqConnectService : IQueueConnectService
{
    private readonly IOptionsSnapshot<RabbitMqOptions> _rabbitMqOptions;

    public RabbitMqConnectService(IOptionsSnapshot<RabbitMqOptions> rabbitMqOptions)
    {
        _rabbitMqOptions = rabbitMqOptions;
    }

    public IModel Connect()
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.Value.Server
        };
        var chanel = ConnectChannel(factory);
        chanel.QueueDeclare(queue: _rabbitMqOptions.Value.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        return chanel;
    }
    
    private static IModel ConnectChannel(IConnectionFactory factory)
    {
        var connection = factory.CreateConnection();
        var chanel = connection.CreateModel();
        return chanel;
    }
}