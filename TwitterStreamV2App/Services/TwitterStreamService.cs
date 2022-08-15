using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Options;
using Polly;
using RestSharp;
using RestSharp.Authenticators;
using TwitterStreamV2App.Interfaces;
using TwitterStreamV2App.Models;

namespace TwitterStreamV2App.Services;

public class TwitterStreamService : ITwitterStreamService
{
    private const string TweetSampleStreamEndpointName = "tweets/sample/stream?tweet.fields=entities";
    
    private readonly IRestClientService _restClientService;
    private readonly IQueueService _queueService;
    
    public TwitterStreamService(IRestClientService restClientService, IQueueService queueService)
    {
        _restClientService = restClientService;
        _queueService = queueService;
    }

    public async Task CollectTweets(CancellationToken cancellationToken = default)
    {
        try
        {
            var count = 10;
            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(retryCount: count, sleepDurationProvider: _ => TimeSpan.FromSeconds(Math.Pow(2, count)));

            await retryPolicy.ExecuteAsync(async () =>
            {
                await foreach (var twitterStreamResponse in RequestTwitterStreamAsync(cancellationToken))
                {
                    _queueService.SendMessage(twitterStreamResponse);
                }
            });
        }
        catch (Exception)
        {
            Console.WriteLine("Rabbit MQ fail to connect...");
        }
        
    }

    public async IAsyncEnumerable<TwitterStreamResponse> RequestTwitterStreamAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response =
            _restClientService.GetTwitterStream<TwitterSingleObject<TwitterStreamResponse>>(
                TweetSampleStreamEndpointName, cancellationToken);
        await foreach (var item in response.WithCancellation(cancellationToken)) {
            yield return item.Data;
        }
    }
    
}