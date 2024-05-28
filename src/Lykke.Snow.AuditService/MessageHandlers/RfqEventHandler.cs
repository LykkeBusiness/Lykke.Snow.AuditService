// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

using JetBrains.Annotations;

using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.Common.Startup;

using MarginTrading.Backend.Contracts.Events;

using Microsoft.Extensions.Logging;

namespace Lykke.Snow.AuditService.MessageHandlers
{
    [UsedImplicitly]
    internal sealed class RfqEventHandler : IMessageHandler<RfqEvent>
    {
        private readonly BrokerId _brokerId;
        private readonly ILogger<RfqEventHandler> _logger;
        private readonly IAuditEventProcessor _auditEventProcessor;
        private readonly IAuditEventMapper<RfqEvent> _rfqAuditEventMapper;

        public RfqEventHandler(BrokerId brokerId,
            ILogger<RfqEventHandler> logger,
            IAuditEventProcessor auditEventProcessor,
            IAuditEventMapper<RfqEvent> rfqAuditEventMapper)
        {
            _brokerId = brokerId;
            _logger = logger;
            _auditEventProcessor = auditEventProcessor;
            _rfqAuditEventMapper = rfqAuditEventMapper;
        }

        public Task Handle(RfqEvent message)
        {
            if (_brokerId == message.BrokerId)
            {
                return Task.CompletedTask;
            }

            _logger.LogDebug("Incoming RfqEvent {@e}", message);

            return _auditEventProcessor.ProcessEvent(message, _rfqAuditEventMapper);
        }
    }
}