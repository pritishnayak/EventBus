using Azure.Messaging.ServiceBus;
using System.Diagnostics;

namespace EventBus.WebApi.ESB.MessageProcessors
{
    public abstract class Broker<T> : IBroker<T>
    {
        public abstract string TopicName { get; }
        public abstract string SubscriptionName { get; }
        public Func<ProcessMessageEventArgs, Task> ProcessMessageAsync => MessageHandlerAsync;
        public Func<ProcessErrorEventArgs, Task> ProcessErrorAsync => ErrorHandlerAsync;
        public IReadOnlyDictionary<string, object> ApplicationProperties { get; private set; }

        public abstract Task<MessageStatus> OnMessageReceivedAsync(T payload);

        public virtual Task OnErrorAsync(Exception exception)
        {
            // TODO: what to do when unexpected error occurs
            return Task.CompletedTask;
        }

        private async Task MessageHandlerAsync(ProcessMessageEventArgs args)
        {
            ApplicationProperties = args.Message.ApplicationProperties;
            var payload = args.Message.Body.ToObjectFromJson<T>();

            // TODO: validate if message is of type T. If not send to posion queue
            var action = await OnMessageReceivedAsync(payload);

            switch (action)
            {
                case MessageStatus.Success:
                    await args.CompleteMessageAsync(args.Message);
                    break;
                case MessageStatus.Abandon:
                    await args.AbandonMessageAsync(args.Message);
                    break;
                case MessageStatus.Failure:
                    await args.DeadLetterMessageAsync(args.Message);
                    break;
                case MessageStatus.Defer:
                    Debug.WriteLine($"LockToken to receive deferedMessage: {args.Message.LockToken}");
                    await args.DeferMessageAsync(args.Message);
                    break;
                default:
                    throw new Exception("BOOM!!! unhandled exception");
            }
        }

        private Task ErrorHandlerAsync(ProcessErrorEventArgs args)
        {
            // TODO: what to do when unexpected error occurs
            Debug.WriteLine("Exception: {0}\nErrorSource: {1}", args.Exception.ToString(), args.ErrorSource.ToString());
            return OnErrorAsync(args.Exception);
        }
    }
}
