using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace fit5032.order_receiver.Services
{
    public interface IServiceBusMessageProcessorService
    {
        Task HandleMessagesAsync();
        Task CloseSubscriptionAsync();
    }
}
