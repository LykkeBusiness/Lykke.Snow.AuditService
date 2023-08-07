using Autofac;

using Lykke.Common.MsSql;
using Lykke.Snow.AuditService.Domain.Repositories;
using Lykke.Snow.AuditService.Settings;
using Lykke.Snow.AuditService.SqlRepositories;
using Lykke.Snow.AuditService.SqlRepositories.Repositories;

namespace Lykke.Snow.AuditService.Modules
{
    public class DalModule : Module
    {
        private readonly AuditServiceSettings _auditServiceSettings;

        public DalModule(AuditServiceSettings auditServiceSettings)
        {
            _auditServiceSettings = auditServiceSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMsSql(_auditServiceSettings.Db.ConnectionString,
                connStr => new AuditDbContext(connStr, isTracingEnabled: false), 
                dbConnection => new AuditDbContext(dbConnection));
        
            builder.RegisterType<AuditEventRepository>()
                .As<IAuditEventRepository>()
                .SingleInstance();
        }
    }
}