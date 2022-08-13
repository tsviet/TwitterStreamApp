﻿using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NSubstitute;
using TwitterStreamV2App.Interfaces;
using TwitterStreamV2App.Models;
using TwitterStreamV2App.Services;
using TwitterStreamV2App.Unit.Tests.Mocks;

namespace TwitterStreamV2App.Unit.Tests;

public class TwitterStreamServiceTests
{
    private readonly ITwitterStreamService _sut;
    private readonly IRestClientService _restClientService = Substitute.For<IRestClientService>();
    private readonly IQueueService _queueService = Substitute.For<IQueueService>();
    
    public TwitterStreamServiceTests()
    {
        _sut = new TwitterStreamService(_restClientService, _queueService);
    }

    [Fact]
    public async Task CollectTweets_ShouldHaveNoExceptions_CanConnectToTwitterApi()
    {
        //Arrange 
        var ct = new CancellationToken();
        var mockData = new List<TwitterStreamService.TwitterSingleObject<TwitterStreamResponse>>
        {
            new (new TwitterStreamResponse
            {
                Entities = new Entities
                {
                    Hashtags = new List<Hashtag>
                    {
                        new Hashtag
                        {
                            Tag = "Test1"
                        },
                        new Hashtag
                        {
                            Tag = "Test2"
                        }
                    }
                }
            }),
            new (new TwitterStreamResponse
            {
                Entities = new Entities
                {
                    Hashtags = new List<Hashtag>
                    {
                        new Hashtag
                        {
                            Tag = "Test3"
                        },
                        new Hashtag
                        {
                            Tag = "Test4"
                        }
                    }
                }
            }),
        };
       
        _restClientService.GetTwitterStream<TwitterStreamService.TwitterSingleObject<TwitterStreamResponse>>("test", CancellationToken.None)
            .Returns(mockData.ToAsyncEnumerable());
        
        //Act
        var result = new TwitterStreamResponse();
        await foreach (var twitterStreamResponse in _sut.RequestTwitterStreamAsync(ct))
        {
            result = twitterStreamResponse;
        }
        //Assert
        result.Entities.Hashtags?.FirstOrDefault()?.Tag.Should().Be("Test1");
        
    }
}
