using Azure.Messaging.ServiceBus;
using EventBus.Library.Attributes;
using System.Reflection;

namespace EventBus.Library.Sender;

public interface IMessageSender
{
    Task SendAsync<T>(T message) where T : class, new();
}

public class MessageSender : IMessageSender, IAsyncDisposable
{
    private const string ConnectionString = "Endpoint=sb://asbdemoaspnet.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Uagd1fODkS82MXg6VwGcFxKh5196K3Bk4nfXi7yGqAo=";
    private readonly ServiceBusSender _sender;

    public MessageSender(string topicName)
    {
        ServiceBusClient client = new(ConnectionString);
        _sender = client.CreateSender(topicName);
    }

    public async Task SendAsync<T>(T message) where T : class, new()
    {
        ServiceBusMessage busMsg = new(BinaryData.FromObjectAsJson(message));
        var properties = GetHeadersFromAttribute(message);
        SetHeaders(busMsg, properties);

        await _sender.SendMessagesAsync(new[] { busMsg });
    }

    public async Task SendAsync(IDictionary<string, object> properties)
    {
        ServiceBusMessage busMsg = new();
        SetHeaders(busMsg, properties);

        await _sender.SendMessagesAsync(new[] { busMsg });
    }

    private void SetHeaders(ServiceBusMessage busMsg, IDictionary<string, object> properties)
    {
        foreach (var item in properties)
        {
            // Also consider using this value to signal
            // whether adding heade was a success or not.
            busMsg.ApplicationProperties.TryAdd(item.Key, item.Value);
        }
    }

    private IDictionary<string, object> GetHeadersFromAttribute<T>(T obj)
    {
        var headers = new Dictionary<string, object>();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            IncludeInHeaderAttribute? attr;
            if ((attr = prop.GetCustomAttribute<IncludeInHeaderAttribute>()) != null)
            {
                headers.TryAdd(attr.Key ?? prop.Name, prop.GetValue(obj));
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
