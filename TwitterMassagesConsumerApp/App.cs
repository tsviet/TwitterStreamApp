using TwitterMassagesConsumerApp.Services.Interfaces;

namespace TwitterMassagesConsumerApp;

public class App
{
    private readonly IQueueService _queueService;

    public App(IQueueService queueService)
    {
        _queueService = queueService;
    }

    public async Task StartRunningAsync()
    {
        await _queueService.GetMessagesAsync();
    }
}