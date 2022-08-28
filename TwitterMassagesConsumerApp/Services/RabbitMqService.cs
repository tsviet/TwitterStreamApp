using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TwitterMassagesConsumerApp.Models;
using TwitterMassagesConsumerApp.Repositories.Interfaces;
using TwitterMassagesConsumerApp.Services.Interfaces;
using TwitterStreamV2App.Models;

namespace TwitterMassagesConsumerApp.Services;

public class RabbitMqService : IQueueService
{
    private readonly IOptionsSnapshot<RabbitMqOptions> _rabbitMqOptions;
    private readonly IStorageRepository _storageRepository;
    private static int _totalCount;

    private IModel? Channel { get; set; }

    public RabbitMqService(IOptionsSnapshot<RabbitMqOptions> rabbitMqOptions, IQueueConnectService queueConnectService,
        IStorageRepository storageRepository)
    {
        _rabbitMqOptions = rabbitMqOptions;
        _storageRepository = storageRepository;

        try
        {
            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetry(retryCount: 10, sleepDurationProvider: _ => TimeSpan.FromSeconds(5));

            retryPolicy.Execute(() => { Channel = queueConnectService.Connect(); });
        }
        catch (Exception)
        {
            Console.WriteLine("Rabbit MQ fail to connect...");
        }
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
        consumer.Received += MessageHandler;
        return consumer;
    }

    private void MessageHandler(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
    {
        var body = basicDeliverEventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var twitterStreamResponse = JsonConvert.DeserializeObject<TwitterSingleObject>(message);

        if(twitterStreamResponse?.Data?.Entities?.Hashtags == null) return;

        _totalCount++;
        foreach (var hashtag in twitterStreamResponse.Data.Entities.Hashtags)
        {
            if (hashtag.Tag == null) continue;
            
            _storageRepository.AddNewTag(hashtag.Tag);
            _storageRepository.IncrementTagValue(hashtag.Tag);
        }

        if (_totalCount % 50 != 0) return;
        var topTenHashes = _storageRepository.GetTop10Hashes();
        PrintResults(topTenHashes);
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