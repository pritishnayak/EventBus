using Azure.Messaging.ServiceBus;

namespace EventBus.WebApi.ESB.MessageProcessors
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly ILogger<MessageProcessor> _logger;

        public string TopicName => "Activity1";
        public string SubscriptionName => "Sub1";

        public MessageProcessor(ILogger<MessageProcessor> logger)
        {
            _logger = logger;
        }

        public async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            var action = args.Message.ApplicationProperties["Action"].ToString();

            // Do something meaningful 😉
            _logger.LogInformation("Received a '{Action}' command from '{Topic}':'{SubscriptionName}'", action, TopicName, SubscriptionName);
            _logger.LogInformation("Message: '{BusMsgBody}'", body);
            
            switch (action)
            {
                case "Success":
                    await args.CompleteMessageAsync(args.Message);
                    break;
                case "Abandon":
                    await args.AbandonMessageAsync(args.Message);
                    break;
                case "Failure":
                    await args.DeadLetterMessageAsync(args.Message);
                    break;
                case "Defer":
                    _logger.LogInformation("LockToken to receive deferedMessage: {LockToken}", args.Message.LockToken);
                    await args.DeferMessageAsync(args.Message);
                    break;
                default:
                    throw new Exception("BOOM!!! unhandled exception");
            }
        }
    }
}
