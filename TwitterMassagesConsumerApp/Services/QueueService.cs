using System.Numerics;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TwitterMassagesConsumerApp.Interfaces;
using TwitterMassagesConsumerApp.Models;

namespace TwitterMassagesConsumerApp.Services;

public class QueueService : IQueueService
{
    private readonly IOptionsSnapshot<RabbitMqOptions> _rabbitMqOptions;
    private static readonly Dictionary<string, int> Storage = new();
    private static readonly object Lock = new();
    private static int _totalCount = 0;

    private IModel Channel { get; }

    public QueueService(IOptionsSnapshot<RabbitMqOptions> rabbitMqOptions, IQueueConnectService queueConnectService)
    {
        _rabbitMqOptions = rabbitMqOptions;
        Channel = queueConnectService.Connect();
    }

    public Task GetMessagesAsync()
    {
        
        var consumer = new AsyncEventingBasicConsumer(Channel);
        consumer.Received += MessageHandlerAsync;
        
        while (true)
        {
            Channel.BasicConsume(queue: _rabbitMqOptions.Value.QueueName,
                autoAck: true,
                consumer: consumer);
        }
    }

    private Task MessageHandlerAsync(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
    {
        var body = basicDeliverEventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var twitterStreamResponse = JsonConvert.DeserializeObject<TwitterStreamResponse>(message);
        
        if(twitterStreamResponse == null) return Task.CompletedTask;

        lock (Lock)
        {
            _totalCount++;
            foreach (var hashtag in twitterStreamResponse.Entities.Hashtags)
            {
                AddNewTag(hashtag);
                IncrementTagValue(hashtag);
            }

            if (_totalCount % 200 != 0) return Task.CompletedTask;
        }

        var topTenHashes = Storage.OrderByDescending(x => x.Value).Take(10);
        PrintResults(topTenHashes);
        return Task.CompletedTask;
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

    private static void IncrementTagValue(Hashtag hashtag)
    {
        Storage[hashtag.Tag] += 1;
    }

    private static void AddNewTag(Hashtag hashtag)
    {
        if (!Storage.ContainsKey(hashtag.Tag))
            Storage.Add(hashtag.Tag, 1);
    }
}