using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using TwitterStreamV2App.Interfaces;
using TwitterStreamV2App.Models;

namespace TwitterStreamV2App.Services;

public class RabbitMqService : IQueueService
{
    private readonly IOptionsSnapshot<RabbitMqOptions> _rabbitMqOptions;

    private IModel Channel { get; }

    public RabbitMqService(IOptionsSnapshot<RabbitMqOptions> rabbitMqOptions, IQueueConnectService queueConnectService)
    {
        _rabbitMqOptions = rabbitMqOptions;

        Channel = queueConnectService.Connect();
    }

    public void SendMessage(object message)
    {
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            
        Channel.BasicPublish(exchange: "",
            routingKey: _rabbitMqOptions.Value.QueueName,
            basicProperties: null,
            body: body);
    }
}