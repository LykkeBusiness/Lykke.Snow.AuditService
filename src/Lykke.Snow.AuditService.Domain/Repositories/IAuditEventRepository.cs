using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;

namespace Lykke.Snow.AuditService.Domain.Repositories
{
    public interface IAuditEventRepository
    {
        Task<IList<IAuditModel<AuditDataType>>> GetAllAsync(AuditTrailFilter<AuditDataType> filter);
        Task AddAsync(IAuditModel<AuditDataType> auditEvent);
    }
}