using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly.RateLimit;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.Json;
using TwitterStreamV2App.Interfaces;
using TwitterStreamV2App.Models;

namespace TwitterStreamV2App.Services;

public class RestClientService : IRestClientService
{
    private readonly RestClient _client;
    private HttpStatusCode StatusCode { get; set; }

    public RestClientService(IOptions<TwitterOptions> twitterOptions)
    {
        var options = new RestClientOptions($"{twitterOptions.Value.BaseUrl}/{twitterOptions.Value.ApiVersion}")
        {
            MaxTimeout = 300000, //5 min
        };
        
        _client = new RestClient(options)
        {
            Authenticator = new JwtAuthenticator(twitterOptions.Value.BearerToken),
        };
    }
    
    public IAsyncEnumerable<T> GetTwitterStream<T>(string name, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(name)
        {
            OnAfterRequest = (response) =>
            {
                StatusCode = response.StatusCode;
                return ValueTask.CompletedTask;
            }
        };

        return StreamTwitterJsonAsync<T>(
            _client, request, cancellationToken
        );
    }

    private async IAsyncEnumerable<T> StreamTwitterJsonAsync<T>(
        RestClient client,
        RestRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken
    ) {
        await using var stream = await client.DownloadStreamAsync(request, cancellationToken).ConfigureAwait(false);

        if (stream == null) yield break;

        HandleTwitterExceptions();

        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested) {
            var line = await reader.ReadLineAsync().ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(line)) continue;

            var response = new RestResponse { Content = line };
            yield return new SystemTextJsonSerializer().Deserialize<T>(response)!;
        }
    }
    
    private void HandleTwitterExceptions()
    {
        switch (StatusCode)
        {
            case HttpStatusCode.BadRequest:
                throw new BadHttpRequestException("Invalid parameters");
            case HttpStatusCode.Unauthorized
                or HttpStatusCode.Forbidden:
                throw new UnauthorizedAccessException("Invalid token");
            case HttpStatusCode.Gone:
            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.GatewayTimeout:
                throw new HttpRequestException("Service error retry");
            case HttpStatusCode.TooManyRequests:
                //App rate limit (Application-only): 50 requests per 15-minute window shared among all users of your app
                throw new RateLimitRejectedException(TimeSpan.FromMinutes(15));
            case HttpStatusCode.Conflict:
                throw new ConnectionAbortedException("Returned when attempting to connect to a filtered stream that has no rules.");
        }
    }
}