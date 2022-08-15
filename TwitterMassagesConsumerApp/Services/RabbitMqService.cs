using System.Numerics;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TwitterMassagesConsumerApp.Models;
using TwitterMassagesConsumerApp.Repositories.Interfaces;
using TwitterMassagesConsumerApp.Services.Interfaces;

namespace TwitterMassagesConsumerApp.Services;

public class RabbitMqService : IQueueService
{
    private readonly IOptionsSnapshot<RabbitMqOptions> _rabbitMqOptions;
    private readonly IStorageRepository _storageRepository;
    private static readonly object Lock = new();
    private static int _totalCount = 0;

    private IModel Channel { get; }

    public RabbitMqService(IOptionsSnapshot<RabbitMqOptions> rabbitMqOptions, IQueueConnectService queueConnectService,
        IStorageRepository storageRepository)
    {
        _rabbitMqOptions = rabbitMqOptions;
        _storageRepository = storageRepository;
        Channel = queueConnectService.Connect();
    }

    public Task GetMessagesAsync()
    {
        var consumer = CreateConsumer();
        while (true)
        {
            Channel.BasicConsume(queue: _rabbitMqOptions.Value.QueueName,
                autoAck: true,
                consumer: consumer);
        }
    }

    private EventingBasicConsumer CreateConsumer()
    {
        var consumer = new EventingBasicConsumer(Channel);
        consumer.Received += MessageHandlerAsync;
        return consumer;
    }

    private void MessageHandlerAsync(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
    {
        var body = basicDeliverEventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var twitterStreamResponse = JsonConvert.DeserializeObject<TwitterStreamResponse>(message);
        
        if(twitterStreamResponse == null) return;

        lock (Lock)
        {
            _totalCount++;
            foreach (var hashtag in twitterStreamResponse.Entities.Hashtags)
            {
                _storageRepository.AddNewTag(hashtag.Tag);
                _storageRepository.IncrementTagValue(hashtag.Tag);
            }

            if (_totalCount % 200 != 0) return;
            var topTenHashes = _storageRepository.GetTop10Hashes();
            PrintResults(topTenHashes);
        }
    }
    
    private static void PrintResults(IEnumerable<KeyValuePair<string, int>> topTenHashes)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Total Count: {_totalCount}");

        var position = 0;
        foreach (var hashes in topTenHashes)
        {
            var percent = Math.Round(hashes.Value / (double) _totalCount * 100, 2);
            sb.AppendLine($"{++position}. Hashtag: {hashes.Key}, " +
                          $"Count: {hashes.Value}, " +
                          $"Percentile: {percent}");
        }

        Console.WriteLine(sb.ToString());
    }
}