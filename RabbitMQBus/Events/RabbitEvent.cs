using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQBus.Events
{
    public abstract class RabbitEvent
    {
        public DateTime Timestamp { get; protected set; }

        protected RabbitEvent()
        {
            Timestamp = DateTime.Now;
        }
    }
}
