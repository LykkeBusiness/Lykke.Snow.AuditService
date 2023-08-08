// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Autofac;
using Lykke.Middlewares.Mappers;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.AuditService.DomainServices.Services;
using Lykke.Snow.AuditService.Startup;
using Microsoft.AspNetCore.Authentication;

namespace Lykke.Snow.AuditService.Modules
{
    internal class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultHttpStatusCodeMapper>()
                .As<IHttpStatusCodeMapper>()
                .SingleInstance();

            builder.RegisterType<DefaultLogLevelMapper>()
                .As<ILogLevelMapper>()
                .SingleInstance();

            builder.RegisterType<SystemClock>()
                .As<ISystemClock>()
                .SingleInstance();
            
            builder.RegisterType<StartupManager>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<RfqAuditTrailService>()
                .As<IRfqAuditTrailService>()
                .SingleInstance();
        }
    }
}
