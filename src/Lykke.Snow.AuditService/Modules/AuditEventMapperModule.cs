// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Autofac;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.AuditService.DomainServices.AuditEventMappers;
using MarginTrading.Backend.Contracts.Events;

namespace Lykke.Snow.AuditService.Modules
{
    public class AuditEventMapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RfqAuditEventMapper>()
                .As<IAuditEventMapper<RfqEvent>>()
                .SingleInstance();
        }
    }
}