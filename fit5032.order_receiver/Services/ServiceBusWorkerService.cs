using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace fit5032.order_receiver.Services
{
    public class ServiceBusWorkerService : IHostedService
    {
        private readonly IServiceBusMessageProcessorService _serviceBusMessageProcessorService;

        public ServiceBusWorkerService(IServiceBusMessageProcessorService serviceBusMessageProcessorService)
        {
            _serviceBusMessageProcessorService = serviceBusMessageProcessorService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _serviceBusMessageProcessorService.HandleMessagesAsync().ConfigureAwait(false);
        }


        public async Task StopAsync(CancellationToken stoppingToken)
        {
            await _serviceBusMessageProcessorService.CloseSubscriptionAsync().ConfigureAwait(false);
        }
    }
}
