using System.Threading.Tasks;
using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;

namespace Lykke.Snow.AuditService.Domain.Repositories
{
    public interface IAuditEventRepository
    {
        Task<PaginatedResponse<IAuditModel<AuditDataType>>> GetAllAsync(AuditTrailFilter<AuditDataType> filter, int? skip, int? take);
        Task AddAsync(IAuditModel<AuditDataType> auditEvent);
    }
}