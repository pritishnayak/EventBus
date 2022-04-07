using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventBus.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private const string ConnectionString = "Endpoint=sb://event-bus-demo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=TAqAO0CzrGTOnnEQ7uYupRR4Npp8Zfq032PH4U3uJzY=";
        private const string TopicName = "Activity1";

        private readonly ILogger<ActivityController> _logger;

        public ActivityController(ILogger<ActivityController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost(Name = "CreateActivity")]
        public async Task<IActionResult> CreateAsync([FromBody] string payload)
        {
            _logger.LogInformation("Received payload: {Payload}", payload);

            await using ServiceBusClient client = new(ConnectionString);
            await using ServiceBusSender sender = client.CreateSender(TopicName);

            ServiceBusMessage busmsg = new(payload);
            await sender.SendMessagesAsync(new[] { busmsg });
            _logger.LogInformation("A bus {BusMsgId} messages has been published to the {TopicName}.", busmsg.MessageId, TopicName);

            return Ok();
        }
    }
}
