using RabbitMQBus.Events;

namespace RabbitMQBus.RabbitBus
{
    public  interface IDriveRabbitEvent<in TEvent> : IDriveRabbitEvent where TEvent: RabbitEvent
    {
        Task Handle(TEvent @event);
    }

    public interface IDriveRabbitEvent
    {

    }
}
