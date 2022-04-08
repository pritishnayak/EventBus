using Azure.Messaging.ServiceBus;
using EventBus.WebApi.ESB;
using System.Collections.Concurrent;

namespace EventBus.WebApi
{
    public class EventRegistractions : IHostedService
    {
        private const string ConnectionString = "Endpoint=sb://asbdemoaspnet.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Uagd1fODkS82MXg6VwGcFxKh5196K3Bk4nfXi7yGqAo=";

        private readonly ServiceBusClient _client;
        private readonly IEnumerable<IBroker> _brokers;

        private readonly ConcurrentDictionary<string, ServiceBusProcessor> _serviceBusProcessors = new();

        public EventRegistractions(IEnumerable<IBroker> brokers)
        {
            _client = new(ConnectionString);
            _brokers = brokers;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var broker in _brokers)
            {
                var processor = _client.CreateProcessor(broker.TopicName, broker.SubscriptionName, new ServiceBusProcessorOptions());
                processor.ProcessMessageAsync += broker.ProcessMessageAsync;
                processor.ProcessErrorAsync += broker.ProcessErrorAsync;

                _serviceBusProcessors.TryAdd($"{broker.TopicName}:{broker.SubscriptionName}", processor);
            }

            await Task.WhenAll(_serviceBusProcessors.Values.Select(p => p.StartProcessingAsync(cancellationToken)));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var processor in _serviceBusProcessors.Values)
            {
                await processor.StopProcessingAsync(cancellationToken);
                await processor.DisposeAsync();
            }
            await _client.DisposeAsync();
        }
    }
}
