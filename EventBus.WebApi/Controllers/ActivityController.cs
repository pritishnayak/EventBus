using EventBus.Library.Sender;
using Microsoft.AspNetCore.Mvc;

namespace EventBus.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActivityController : ControllerBase
{
    //private const string ConnectionString = "Endpoint=sb://asbdemoaspnet.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Uagd1fODkS82MXg6VwGcFxKh5196K3Bk4nfXi7yGqAo=";
    //private const string TopicName = "Help.Help.Help";

    private readonly IMessageSender<AntMan> _antSender;
    private readonly ILogger<ActivityController> _logger;

    public ActivityController(IMessageSender<AntMan> antSender, ILogger<ActivityController> logger)
    {
        _antSender = antSender;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost(Name = "CreateActivity")]
    public async Task<ActionResult<AntMan>> CreateAsync([FromBody] string name)
    {
        _logger.LogInformation("Received payload: {Payload}", name);

        //await using ServiceBusClient client = new(ConnectionString);
        //await using ServiceBusSender sender = client.CreateSender(TopicName);

        AntMan newAnt = new(Random.Shared.Next(0, 10), name);
        //ServiceBusMessage busmsg = new(BinaryData.FromObjectAsJson(newAnt));
        await _antSender.SendAsync(newAnt);
        //_logger.LogInformation("A bus {BusMsgId} messages has been published to the {TopicName}.", busmsg.MessageId, TopicName);

        return Ok(newAnt);
    }
}
