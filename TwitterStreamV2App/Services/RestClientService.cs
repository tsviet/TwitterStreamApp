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

    public RestClientService(IOptionsSnapshot<TwitterOptions> twitterOptions)
    {
        var options = new RestClientOptions($"{twitterOptions.Value.BaseUrl}/{twitterOptions.Value.ApiVersion}")
        {
            MaxTimeout = 300000, //5 min
        };
        
        _client = new RestClient(options)
        {
            Authenticator = new JwtAuthenticator(twitterOptions.Value.BearerToken)
        };
    }
    
    public IAsyncEnumerable<T> GetTwitterStream<T>(string name, CancellationToken cancellationToken = default)
    {
       return _client.StreamJsonAsync<T>(
            name, cancellationToken
        );
    }
}