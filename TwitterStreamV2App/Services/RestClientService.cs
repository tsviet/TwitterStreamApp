using System.Text;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;
using TwitterStreamV2App.Interfaces;
using TwitterStreamV2App.Models;

namespace TwitterStreamV2App.Services;

public class RestClientService : IRestClientService
{
    private readonly RestClient _client;
    private readonly IOptionsSnapshot<TwitterOptions> _twitterOptions;
    
    public RestClientService(IOptionsSnapshot<TwitterOptions> twitterOptions)
    {
        _twitterOptions = twitterOptions;
        var options = new RestClientOptions($"{_twitterOptions.Value.BaseUrl}/{_twitterOptions.Value.ApiVersion}")
        {
            MaxTimeout = 300000, //5 min
        };
        
        _client = new RestClient(options)
        {
            Authenticator = new JwtAuthenticator(_twitterOptions.Value.BearerToken)
        };
    }
    
    public IAsyncEnumerable<T> GetTwitterStream<T>(string name, CancellationToken cancellationToken = default)
    {
       return _client.StreamJsonAsync<T>(
            name, cancellationToken
        );
    }
}