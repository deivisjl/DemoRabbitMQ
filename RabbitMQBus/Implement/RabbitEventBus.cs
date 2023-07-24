using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQBus.Command;
using RabbitMQBus.Events;
using RabbitMQBus.RabbitBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Reflection;

namespace RabbitMQBus.Implement
{
    public class RabbitEventBus : IRabbitEventBus
    {
        private readonly IMediator _mediator;
        private readonly Dictionary<string, List<Type>> _rabbitDrive;
        private readonly List<Type> _eventType;
        private readonly string RABBIT_MQ_SERVICE = "RABBIT_MQ_SERVICE";

        public RabbitEventBus(IMediator mediator)
        {
            _mediator = mediator;
            _rabbitDrive = new Dictionary<string, List<Type>>();
            _eventType = new List<Type>();
            _mediator = mediator;
        }
        public void Publish<T>(T @event) where T : RabbitEvent
        {
            var factory = new ConnectionFactory() { HostName = RABBIT_MQ_SERVICE };//Nombre del contenedor
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var eventName = _eventType.GetType().Name;
                channel.QueueDeclare(eventName, false, false, false, null);

                var message = JsonConvert.SerializeObject(_eventType);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("", eventName, null, body);
            }
        }

        public Task SendCommand<T>(T command) where T : RabbitCommand
        {
            return _mediator.Send(command);
        }

        public void Subscribe<T, TH>()
            where T : RabbitEvent
            where TH : IDriveRabbitEvent<T>
        {
            var eventName = typeof(T).Name;
            var driveEventType = typeof(TH);

            if(!_eventType.Contains(typeof(T)))
            {
                _eventType.Add(typeof(T));
            }

            if(!_rabbitDrive.ContainsKey(eventName))
            {
                _rabbitDrive.Add(eventName,new List<Type>());

            }

            if (_rabbitDrive[eventName].Any( x=> x.GetType() == driveEventType))
            {
                throw new ArgumentException($"The drive {driveEventType.Name} was register by {eventName} already");
            }

            _rabbitDrive[eventName].Add(driveEventType);

            var factory = new ConnectionFactory()
            {
                HostName = RABBIT_MQ_SERVICE,//Nombre del contenedor
                DispatchConsumersAsync = true
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(eventName, false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += DelegateConsumer;

            channel.BasicConsume(eventName, true, consumer);
        }

        private async Task DelegateConsumer(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            try
            {
                if(_rabbitDrive.ContainsKey(eventName))
                {
                    var subscriptions = _rabbitDrive[eventName];

                    foreach (var subscription in subscriptions)
                    {
                        var drive = Activator.CreateInstance(subscription);

                        if(drive != null)
                        {
                            var eventTypeAux = _eventType.SingleOrDefault(x => x.Name == eventName);
                            var eventDS = JsonConvert.DeserializeObject(message, eventTypeAux);
                            
                            var eventBaseType = typeof(IDriveRabbitEvent<>).MakeGenericType(eventTypeAux);

                            await (Task)eventBaseType.GetMethod("Handle").Invoke(drive, new object[] {eventDS});
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
