using RabbitMQ.Client;

namespace TwitterStreamV2App.Interfaces;

public interface IQueueConnectService
{
    public IModel Connect();
}