using TwitterStreamV2App.Interfaces;

namespace TwitterStreamV2App.Unit.Tests;

public class RabbitMqConnectServiceTests
{
    private readonly IQueueConnectService _queueConnectService;

    public RabbitMqConnectServiceTests(IQueueConnectService queueConnectService)
    {
        _queueConnectService = queueConnectService;
    }

    [Fact]
    public void CanConnectToRabbitMq()
    {
        
    }
}