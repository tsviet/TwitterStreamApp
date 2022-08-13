using System.Text;
using RabbitMQ.Client;
using TwitterStreamV2App.Interfaces;

namespace TwitterStreamV2App;

public class App
{
    private readonly ITwitterStreamService _twitterStreamService;

    public App(ITwitterStreamService twitterStreamService)
    {
        _twitterStreamService = twitterStreamService;
    }

    public async Task StartRunningAsync()
    {
        
        var ct = new CancellationToken();
        await _twitterStreamService.CollectTweets(ct);
    }
}