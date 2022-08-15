using TwitterMassagesConsumerApp.Repositories.Interfaces;

namespace TwitterMassagesConsumerApp.Repositories;

public class StorageRepository : IStorageRepository
{
    private static readonly Dictionary<string, int> Storage = new();


    public void AddNewTag(string value)
    {
        if (!Storage.ContainsKey(value))
            Storage.Add(value, 1);
    }

    public void IncrementTagValue(string value)
    {
        Storage[value] += 1;
    }

    public IEnumerable<KeyValuePair<string, int>> GetTop10Hashes()
    {
        return Storage.OrderByDescending(x => x.Value).Take(10);
    }
}