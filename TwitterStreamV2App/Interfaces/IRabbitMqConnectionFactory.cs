using RabbitMQ.Client;

namespace TwitterStreamV2App.Interfaces;

public interface IRabbitMqConnectionFactory
{
    IConnection CreateConnection();
}