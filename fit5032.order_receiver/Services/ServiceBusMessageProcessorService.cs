using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using fit5032.order_receiver.ConfigOptions;
using fit5032.order_receiver.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace fit5032.order_receiver.Services
{
    public class ServiceBusMessageProcessorService: IServiceBusMessageProcessorService
    {
        private readonly ServiceBusProcessor _serviceBusProcessor;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ServiceBusMessageProcessorService(ServiceBusClient serviceBusClient, IOptions<ListenerServiceBusConfig> serviceBusConfig, IServiceScopeFactory serviceScopeFactory)
        {
            var topic = serviceBusConfig.Value.Topic;
            var subscription = serviceBusConfig.Value.Subscription;

            _serviceBusProcessor = serviceBusClient.CreateProcessor(topic, subscription, new ServiceBusProcessorOptions());
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task HandleMessagesAsync()
        {
            ServiceBusProcessorOptions _serviceBusProcessorOptions = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false,
            };

            // add handler to process messages
            _serviceBusProcessor.ProcessMessageAsync += MessageHandler;

            // add handler to process any errors
            _serviceBusProcessor.ProcessErrorAsync += ErrorHandler;

            await _serviceBusProcessor.StartProcessingAsync().ConfigureAwait(false);
        }

        public async Task CloseSubscriptionAsync()
        {
            await _serviceBusProcessor.CloseAsync().ConfigureAwait(false);
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            // Extract Order from Serviec Bus Message Body
            var order = JsonConvert.DeserializeObject<CustomerOrder>(args.Message.Body.ToString());

            // Db Context is a scoped service, while Hosted Service is a singleton service.
            // We need to manually create a scope in order to resolve scoped services in a singleton
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // Get the DB Context service
                var orderingDBContext = scope.ServiceProvider.GetService<OrderingDBContext>();

                // Save order to the Database
                await orderingDBContext.CustomerOrders.AddAsync(order);

                await orderingDBContext.SaveChangesAsync();
            }


            // complete the message. messages is deleted from the subscription. 
            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
