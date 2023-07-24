using RabbitMQBus.Events;

namespace RabbitMQBus.Command
{
    public abstract class RabbitCommand : RabbitMessage
    {
        public DateTime Timestamp { get; protected set; }

        protected RabbitCommand()
        {
            Timestamp = DateTime.Now;
        }
    }
}
