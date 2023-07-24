using RabbitMQBus.Events;

namespace RabbitMQBus.QueueEvent
{
    public class EmailEventQueue : RabbitEvent
    {
        public string Address { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public EmailEventQueue(string address, string title, string body)
        {
            Address = address;
            Title = title;
            Body = body;
        }
    }
}
