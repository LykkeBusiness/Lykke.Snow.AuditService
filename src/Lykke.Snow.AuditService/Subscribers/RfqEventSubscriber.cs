// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Deserializers;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.Snow.AuditService.Domain.Services;
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
        private readonly string _brokerId;
        private readonly ILogger<RfqEventSubscriber> _logger;
        private readonly IAuditEventProcessor _auditEventProcessor;
        private readonly IAuditEventMapper<RfqEvent> _rfqAuditEventMapper;

        public RfqEventSubscriber(IAuditEventProcessor auditEventProcessor,
            ILoggerFactory loggerFactory,
            SubscriptionSettings settings,
            string brokerId,
            ILogger<RfqEventSubscriber> logger,
            IAuditEventMapper<RfqEvent> rfqAuditEventMapper)
        {
            if (string.IsNullOrEmpty(brokerId))
            {
                throw new ArgumentException("BrokerId must be configured");
            }
            
            _auditEventProcessor = auditEventProcessor;
            _loggerFactory = loggerFactory;
            _settings = settings;
            _brokerId = brokerId;
            _logger = logger;
            _rfqAuditEventMapper = rfqAuditEventMapper;

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
                  .SetMessageDeserializer(new JsonMessageDeserializer<RfqEvent>())
                  .SetMessageReadStrategy(new MessageReadWithTemporaryQueueStrategy())
                  .Subscribe(ProcessMessageAsync)
                  .Start();
        }
        
        public Task ProcessMessageAsync(RfqEvent e)
        {
            if(!string.Equals(e.BrokerId, _brokerId, StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            _logger.LogDebug("Incoming RfqEvent {@e}", e);

            return _auditEventProcessor.ProcessEvent(e, _rfqAuditEventMapper);
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