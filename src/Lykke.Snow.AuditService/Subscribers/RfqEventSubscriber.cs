using System.Threading.Tasks;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Deserializers;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.Snow.AuditService.Settings;

using MarginTrading.Backend.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.AuditService.Subscribers
{
    public class RfqEventSubscriber : IStartStop
    {
        private RabbitMqPullingSubscriber<RfqEvent>? _subscriber;
        private readonly ILoggerFactory _loggerFactory;
        private readonly RabbitMqSubscriptionSettings _settings;

        public RfqEventSubscriber(ILoggerFactory loggerFactory, 
            SubscriptionSettings settings)
        {
            _loggerFactory = loggerFactory;
            _settings = settings;
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            _subscriber = new RabbitMqPullingSubscriber<RfqEvent>(
                  _loggerFactory.CreateLogger<RabbitMqPullingSubscriber<RfqEvent>>(),
                  _settings)
                  .SetMessageDeserializer(new MessagePackMessageDeserializer<RfqEvent>())
                  .SetMessageReadStrategy(new MessageReadWithTemporaryQueueStrategy())
                  .Subscribe(ProcessMessageAsync)
                  .Start();
        }
        
        public Task ProcessMessageAsync(RfqEvent e)
        {
            // TODO: Handle the message
            return Task.CompletedTask;
        }

        public void Stop()
        {
            if(_subscriber != null)
            {
                _subscriber.Stop();
                _subscriber.Dispose();
            }
        }
    }
}