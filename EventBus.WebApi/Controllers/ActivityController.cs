using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;

namespace EventBus.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActivityController : ControllerBase
{
    private const string ConnectionString = "Endpoint=sb://asbdemoaspnet.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Uagd1fODkS82MXg6VwGcFxKh5196K3Bk4nfXi7yGqAo=";
    private const string TopicName = "Help.Help.Help";

    private readonly ILogger<ActivityController> _logger;

    public ActivityController(ILogger<ActivityController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost(Name = "CreateActivity")]
    public async Task<IActionResult> CreateAsync([FromBody] string name)
    {
        _logger.LogInformation("Received payload: {Payload}", name);

        await using ServiceBusClient client = new(ConnectionString);
        await using ServiceBusSender sender = client.CreateSender(TopicName);

        ServiceBusMessage busmsg = new(BinaryData.FromObjectAsJson(new AntMan(Random.Shared.Next(0, 10), name)));
        await sender.SendMessagesAsync(new[] { busmsg });
        _logger.LogInformation("A bus {BusMsgId} messages has been published to the {TopicName}.", busmsg.MessageId, TopicName);

        return Ok();
    }
}
