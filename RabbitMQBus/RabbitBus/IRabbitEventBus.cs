using RabbitMQBus.Command;
using RabbitMQBus.Events;

namespace RabbitMQBus.RabbitBus
{
    public interface IRabbitEventBus
    {
        Task SendCommand<T>(T command) where T : RabbitCommand;

        void Publish<T>(T @event) where T : RabbitEvent;

        void Subscribe<T, TH>() where T : RabbitEvent
                                where TH : IDriveRabbitEvent<T>;
    }
}
