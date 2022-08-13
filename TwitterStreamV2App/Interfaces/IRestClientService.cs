namespace TwitterStreamV2App.Interfaces;

public interface IRestClientService
{
    IAsyncEnumerable<T> GetTwitterStream<T>(string name, CancellationToken cancellationToken = default);
}