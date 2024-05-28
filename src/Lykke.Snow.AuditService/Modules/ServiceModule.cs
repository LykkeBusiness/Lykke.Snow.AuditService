// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Autofac;
using Lykke.Middlewares.Mappers;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.AuditService.DomainServices.Services;
using Lykke.Snow.AuditService.Settings;
using Lykke.Snow.Common.Startup.Extensions;

using Microsoft.AspNetCore.Authentication;

namespace Lykke.Snow.AuditService.Modules
{
    internal class ServiceModule : Module
    {
        private readonly AuditServiceSettings _auditServiceSettings;

        public ServiceModule(AuditServiceSettings auditServiceSettings)
        {
            _auditServiceSettings = auditServiceSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.AddBrokerId(_auditServiceSettings.BrokerId);
            
            builder.RegisterType<DefaultHttpStatusCodeMapper>()
                .As<IHttpStatusCodeMapper>()
                .SingleInstance();

            builder.RegisterType<DefaultLogLevelMapper>()
                .As<ILogLevelMapper>()
                .SingleInstance();

            builder.RegisterType<SystemClock>()
                .As<ISystemClock>()
                .SingleInstance();
            
            builder.RegisterType<AuditEventProcessor>()
                .As<IAuditEventProcessor>()
                .SingleInstance();

            builder.RegisterType<ObjectDiffService>()
                .As<IObjectDiffService>()
                .SingleInstance();

            builder.RegisterType<AuditEventService>()
                .As<IAuditEventService>()
                .SingleInstance();

            builder.RegisterType<AuditObjectStateFactory>()
                .As<IAuditObjectStateFactory>()
                .SingleInstance();
        }
    }
}
