using RabbitMQ.Client;

namespace TwitterMassagesConsumerApp.Interfaces;

public interface IQueueConnectService
{
    public IModel Connect();
}