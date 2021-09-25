using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fit5032.order_receiver.ConfigOptions
{
    public class ListenerServiceBusConfig
    {
        public string ConnectionString { get; set; }
        public string Topic { get; set; }
        public string Subscription { get; set; }
    }
}
