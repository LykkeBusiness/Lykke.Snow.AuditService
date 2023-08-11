// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Autofac;
using Lykke.RabbitMqBroker;
using Lykke.Snow.AuditService.Settings;
using Lykke.Snow.AuditService.Subscribers;

namespace Lykke.Snow.AuditService.Modules
{
    public class RabbitMqModule : Module
    {
        private readonly AuditServiceSettings _auditServiceSettings;

        public RabbitMqModule(AuditServiceSettings auditServiceSettings)
        {
            _auditServiceSettings = auditServiceSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            if(_auditServiceSettings.Subscribers == null)
                throw new ArgumentNullException(nameof(_auditServiceSettings.Subscribers));

            builder.RegisterType<RfqEventSubscriber>()
                .WithParameter(TypedParameter.From(_auditServiceSettings.Subscribers.RfqEventSubscriber))
                .As<IStartStop>()
                .SingleInstance();
        }
    }
}