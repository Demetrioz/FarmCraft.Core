using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmCraft.Core.Services.Messaging.Publisher
{
    public class RabbitMQOptions : MessageBusOptions
    {
        public string Exchange { get; set; }
        public string ExchangeType { get; set; }
        public string RoutingKey { get; set; }
    }
}
