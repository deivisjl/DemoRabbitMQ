using MediatR;

namespace RabbitMQBus.Events
{
    public abstract class RabbitMessage : IRequest<bool>
    {
        public string MessageType { get; protected set; }

        protected RabbitMessage()
        {
            MessageType = GetType().Name;
        }
    }
}
