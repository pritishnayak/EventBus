namespace EventBus.WebApi.ESB.MessageProcessors;

public class BatProcessor : Broker<BatMan>
{
    private readonly ILogger<BatProcessor> _logger;

    public override string TopicName => "Help.Help.Help";
    public override string SubscriptionName => "BatMan";

    public BatProcessor(ILogger<BatProcessor> logger)
    {
        _logger = logger;
    }

    public override Task<MessageStatus> OnMessageReceivedAsync(BatMan bat)
    {
        // Do something meaningful 😉
        _logger.LogWarning("I am Batman!");
        _logger.LogDebug("Received a command from '{Topic}':'{SubscriptionName}'", TopicName, SubscriptionName);
        _logger.LogInformation("BatMan: \"Next time help yourself, '{Person}'!!!\"", bat.Name);

        var action = MessageStatus.Success;
        if (ApplicationProperties.TryGetValue("Action", out var act))
        {
            action = (MessageStatus)act;
        }
        return Task.FromResult(action);
    }
}
