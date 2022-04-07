using Azure.Messaging.ServiceBus;

namespace EventBus.WebApi.ESB
{
    public interface IMessageProcessor
    {
        /// <summary>
        /// try to come up with a convetion based topic name
        /// CommandType.CommadOperation
        /// Organisation.Delete
        /// </summary>
        string TopicName { get; }

        /// <summary>
        /// Usually the name of the app. Do not give option to choose this
        /// </summary>
        string SubscriptionName { get; }

        Task MessageHandler(ProcessMessageEventArgs args);

        //Task ErrorHandler(ProcessErrorEventArgs args);
    }
}
