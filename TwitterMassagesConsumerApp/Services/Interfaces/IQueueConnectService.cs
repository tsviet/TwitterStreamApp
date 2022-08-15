using RabbitMQ.Client;

namespace TwitterMassagesConsumerApp.Services.Interfaces;

public interface IQueueConnectService
{
    public IModel Connect();
}