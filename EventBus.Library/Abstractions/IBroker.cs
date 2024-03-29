﻿using Azure.Messaging.ServiceBus;

namespace EventBus.Library;

public interface IBroker
{
    /// <summary>
    /// try to come up with a convetion based topic name
    /// CommandType.CommadOperation.Request/Response
    /// Organisation.Delete
    /// </summary>
    string TopicName { get; }

    /// <summary>
    /// Usually the name of the app. Do not give option to choose this
    /// </summary>
    string SubscriptionName { get; }
    Func<ProcessMessageEventArgs, Task> ProcessMessageAsync { get; }
    Func<ProcessErrorEventArgs, Task> ProcessErrorAsync { get; }
}

public interface IBroker<in T> : IBroker
{
    Task<MessageStatus> OnMessageReceivedAsync(T payload);
}
