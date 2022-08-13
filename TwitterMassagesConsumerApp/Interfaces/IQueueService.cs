namespace TwitterMassagesConsumerApp.Interfaces;

public interface IQueueService
{
    public Task GetMessagesAsync();
}