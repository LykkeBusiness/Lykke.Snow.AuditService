using System.Threading.Tasks;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Deserializers;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Repositories;
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
        private readonly IAuditEventRepository _auditEventRepository;

        public RfqEventSubscriber(ILoggerFactory loggerFactory,
            SubscriptionSettings settings, 
            IAuditEventRepository auditEventRepository)
        {
            _loggerFactory = loggerFactory;
            _settings = settings;
            _auditEventRepository = auditEventRepository;
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
        
        public async Task ProcessMessageAsync(RfqEvent e)
        {
            // TODO: Handle the message
            await InsertRfqAuditEvent(e);
        }

        private async Task InsertRfqAuditEvent(RfqEvent rfqEvent)
        {
            var auditEvent = new AuditEvent(
                id: rfqEvent.RfqSnapshot.Id,
                timestamp: rfqEvent.RfqSnapshot.LastModified,
                correlationId: rfqEvent.RfqSnapshot.CausationOperationId,
                username: rfqEvent.RfqSnapshot.CreatedBy,
                actionType: rfqEvent.EventType.ToString(),
                actionTypeDetails: rfqEvent.RfqSnapshot.State.ToString(),
                dataType: nameof(RfqEvent),
                dataReference: rfqEvent.RfqSnapshot.Id,
                dataDiff: string.Empty
            );
            
            // TODO: how to obtain diff?

            await _auditEventRepository.AddAsync(auditEvent);
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