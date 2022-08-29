using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using TwitterStreamV2App.Interfaces;
using TwitterStreamV2App.Models;

namespace TwitterStreamV2App.Services;

public class TwitterStreamService : ITwitterStreamService
{
    private const string TweetSampleStreamEndpointName = "tweets/sample/stream?tweet.fields=entities";
    
    private readonly IRestClientService _restClientService;
    private readonly IQueueService _queueService;
    private readonly ILogger<TwitterStreamService> _logger;
    
    public TwitterStreamService(IRestClientService restClientService, IQueueService queueService, ILogger<TwitterStreamService> logger)
    {
        _restClientService = restClientService;
        _queueService = queueService;
        _logger = logger;
    }

    public async Task CollectTweets(CancellationToken cancellationToken = default)
    {
        try
        {
            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    sleepDurationProvider: retryNumber => TimeSpan.FromSeconds(Math.Pow(2, retryNumber)),
                    (exception, attempt) =>
                    {
                        _logger.LogInformation("Attempt: {Attempt}, Found new {Message}", attempt, exception.Message );
                        ShouldStopExecution(exception);
                    });

            await retryPolicy.ExecuteAsync(async () =>
            {
                await foreach (var twitterStreamResponse in RequestTwitterStreamAsync(cancellationToken))
                {
                    _queueService.SendMessage(twitterStreamResponse);
                    
                    //_logger.LogInformation("HashTags: {SerializeObject}", JsonConvert.SerializeObject(twitterStreamResponse.Data?.Entities?.Hashtags ?? new List<Hashtag>()));
                }
            });
        }
        catch (Exception)
        {
            _logger.LogError("Fail to get Twits after max retry....");
        }
        
    }

    public async IAsyncEnumerable<TwitterSingleObject> RequestTwitterStreamAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response =
            _restClientService.GetTwitterStream<TwitterSingleObject>(
                TweetSampleStreamEndpointName, cancellationToken);
        await foreach (var item in response.WithCancellation(cancellationToken)) {
            yield return item;
        }
    }
    
    private void ShouldStopExecution(Exception ex)
    {
        switch (ex)
        {
            case UnauthorizedAccessException:
            case BadHttpRequestException:
                throw ex;
        }
    }
    
}