namespace TwitterMassagesConsumerApp.Repositories.Interfaces;

public interface IStorageRepository
{
    void AddNewTag(string value);
    void IncrementTagValue(string value);
    IEnumerable<KeyValuePair<string, int>> GetTop10Hashes();
}