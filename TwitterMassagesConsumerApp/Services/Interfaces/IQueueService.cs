namespace TwitterMassagesConsumerApp.Services.Interfaces;

public interface IQueueService
{
    public Task GetMessagesAsync();
}