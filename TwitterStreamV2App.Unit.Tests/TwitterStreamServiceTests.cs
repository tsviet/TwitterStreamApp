using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TwitterStreamV2App.Interfaces;
using TwitterStreamV2App.Models;
using TwitterStreamV2App.Services;

namespace TwitterStreamV2App.Unit.Tests;

public class TwitterStreamServiceTests
{
    private readonly ITwitterStreamService _sut;
    private readonly IRestClientService _restClientService = Substitute.For<IRestClientService>();
    private readonly IQueueService _queueService = Substitute.For<IQueueService>();
    private readonly ILogger<TwitterStreamService> _logger = Substitute.For<ILogger<TwitterStreamService>>();
    
    public TwitterStreamServiceTests()
    {
        _sut = new TwitterStreamService(_restClientService, _queueService, _logger);
    }

    [Fact]
    public async Task CollectTweets_ShouldHaveNoExceptions_CanConnectToTwitterApi()
    {
        //Arrange 
        var ct = new CancellationToken();
        var mockData = new List<TwitterSingleObject>
        {
            new()
            {
                Data = new TwitterStreamResponse
                {
                    Entities = new Entities
                    {
                        Hashtags = new List<Hashtag>
                        {
                            new()
                            {
                                Tag = "Test1"
                            },
                            new()
                            {
                                Tag = "Test2"
                            }
                        }
                    }
                },
            }
        };
       
        _restClientService.GetTwitterStream<TwitterSingleObject>("test", CancellationToken.None)
            .Returns(mockData.ToAsyncEnumerable());
        
        //Act
        var result = new TwitterSingleObject();
        await foreach (var twitterStreamResponse in _sut.RequestTwitterStreamAsync(ct))
        {
            result = twitterStreamResponse;
        }
        //Assert
        result.Data.Entities?.Hashtags?.FirstOrDefault()?.Tag.Should().Be("Test1");
        
    }
}
