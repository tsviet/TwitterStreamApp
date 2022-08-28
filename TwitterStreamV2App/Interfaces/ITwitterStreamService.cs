using System.Runtime.CompilerServices;
using TwitterStreamV2App.Models;
using TwitterStreamV2App.Services;

namespace TwitterStreamV2App.Interfaces;

public interface ITwitterStreamService
{
    public Task CollectTweets(CancellationToken cancellationToken = default);

    IAsyncEnumerable<TwitterSingleObject>  RequestTwitterStreamAsync(CancellationToken cancellationToken = default);
}