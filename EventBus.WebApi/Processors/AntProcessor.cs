using EventBus.Library;

namespace EventBus.WebApi.Processors;

public class AntProcessor : Broker<AntMan>
{
    private readonly ILogger<Broker<AntMan>> _logger;

    public override string TopicName => "AntMan";
    public override string SubscriptionName => "AntMan";

    public AntProcessor(ILogger<Broker<AntMan>> logger)
    {
        _logger = logger;
    }

    public override Task<MessageStatus> OnMessageReceivedAsync(AntMan ant)
    {
        // Do something meaningful 😉
        _logger.LogWarning("Ant-man!");
        _logger.LogInformation("Received a command from '{Topic}':'{SubscriptionName}'", TopicName, SubscriptionName);
        _logger.LogInformation("AntMan: \"Happy to help, '{Person}'!!!\"", ant.Name);

        return Task.FromResult(MessageStatus.Success);
    }
}
