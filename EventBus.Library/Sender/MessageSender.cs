using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using EventBus.Library.Attributes;
using System.Reflection;

namespace EventBus.Library.Sender;

public interface IMessageSender<T> : IMessageSender where T : class
{
    Task SendAsync(T message);
}

public interface IMessageSender
{
    Task SendAsync(IDictionary<string, object> properties);
}

public sealed class MessageSender<T> : MessageSender, IMessageSender, IMessageSender<T>, IAsyncDisposable where T : class
{
    public MessageSender() : base(typeof(T).Name)
    {
    }

    public async Task SendAsync(T message)
    {
        ServiceBusMessage busMsg = new(BinaryData.FromObjectAsJson(message));
        var properties = GetHeadersFromAttribute(message);
        SetHeaders(busMsg, properties);

        await _sender.SendMessageAsync(busMsg);
    }

    private static IDictionary<string, object> GetHeadersFromAttribute(T obj)
    {
        var headers = new Dictionary<string, object>();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            IncludeInHeaderAttribute? attr;
            object? value;
            if ((attr = prop.GetCustomAttribute<IncludeInHeaderAttribute>()) is not null
                && (value = prop.GetValue(obj)) is not null)
            {
                headers.TryAdd(attr.Key ?? prop.Name, value);
            }
        }

        return headers;
    }

    // TODO: inject cliet and dispose properly
    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
    }
}

public class MessageSender : IMessageSender
{
    private const string ConnectionString = "Endpoint=sb://asbdemoaspnet.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Uagd1fODkS82MXg6VwGcFxKh5196K3Bk4nfXi7yGqAo=";
    protected readonly ServiceBusSender _sender;

    public MessageSender(string topicName)
    {
        ServiceBusClient client = new(ConnectionString);
        ServiceBusAdministrationClient adminClient = new(ConnectionString);
        if (adminClient.TopicExistsAsync(topicName).Result)
        {
            TopicProperties topic = adminClient.CreateTopicAsync(topicName).Result;
            if (topic.Status != EntityStatus.Active)
            {
                throw new Exception();
            }
        }

        _sender = client.CreateSender(topicName);
    }

    public async Task SendAsync(IDictionary<string, object> properties)
    {
        ServiceBusMessage busMsg = new();
        SetHeaders(busMsg, properties);

        await _sender.SendMessageAsync(busMsg);
    }

    protected static void SetHeaders(ServiceBusMessage busMsg, IDictionary<string, object> properties)
    {
        foreach (var item in properties)
        {
            // Also consider using this value to signal
            // whether adding heade was a success or not.
            busMsg.ApplicationProperties.TryAdd(item.Key, item.Value);
        }
    }
}
