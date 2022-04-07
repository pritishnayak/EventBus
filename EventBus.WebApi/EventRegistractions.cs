using Azure.Messaging.ServiceBus;

namespace EventBus.WebApi
{
    public class EventRegistractions : IHostedService
    {
        private const string ConnectionString = "Endpoint=sb://event-bus-demo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=TAqAO0CzrGTOnnEQ7uYupRR4Npp8Zfq032PH4U3uJzY=";
        private const string TopicName = "Activity1";
        private const string SubscriptionName = "Sub1";
        
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly ILogger<EventRegistractions> _logger;

        public EventRegistractions(ILogger<EventRegistractions> logger)
        {
            _client = new(ConnectionString);
            _processor = _client.CreateProcessor(TopicName, SubscriptionName, new ServiceBusProcessorOptions());
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _processor.ProcessMessageAsync += MessageHandler;

            // add handler to process any errors
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
            await _client.DisposeAsync();
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            _logger.LogInformation("Received: {BusMsgBody} from subscription: {SubscriptionName}", body, SubscriptionName);

            // complete the message. messages is deleted from the subscription. 
            await args.CompleteMessageAsync(args.Message);
        }

        // handle any errors when receiving messages
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            // TODO: what to do when unexpected error occurs
            _logger.LogError(args.Exception, "Exception: {Exception}\nErrorSource: {ErrorSource}", args.Exception.ToString(), args.ErrorSource.ToString());
            return Task.CompletedTask;
        }

        
    }
}
