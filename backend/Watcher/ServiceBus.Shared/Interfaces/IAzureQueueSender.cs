﻿namespace ServiceBus.Shared.Queue
{
    using Microsoft.Azure.ServiceBus;
    using ServiceBus.Shared.Messages;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAzureQueueSender
    {
        Task SendAsync<T>(QueueClient client, T item) where T : InstanceMessage;

        Task SendAsync<T>(QueueClient client, T item, Dictionary<string, object> properties) where T : InstanceMessage;
    }
}
