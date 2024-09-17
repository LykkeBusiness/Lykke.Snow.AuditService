// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

using Autofac;

using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Snow.AuditService.MessageHandlers;
using Lykke.Snow.AuditService.Settings;

using MarginTrading.Backend.Contracts.Events;

namespace Lykke.Snow.AuditService.Modules
{
    public class RabbitMqModule : Module
    {
        private readonly SubscriptionSettings _rfqEventSubscriberSettings;

        public RabbitMqModule(AuditServiceSettings auditServiceSettings)
        {
            _rfqEventSubscriberSettings =
                auditServiceSettings.Subscribers.RfqEventSubscriber ??
                throw new InvalidOperationException("RfqEventSubscriber settings are not configured");
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.AddRabbitMqConnectionProvider();

            builder.AddRabbitMqListener<RfqEvent, RfqEventHandler>(_rfqEventSubscriberSettings)
                .AddOptions(RabbitMqListenerOptions<RfqEvent>.Json.NoLoss)
                .AutoStart();
        }
    }
}