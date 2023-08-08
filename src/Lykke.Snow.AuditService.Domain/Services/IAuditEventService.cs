using System.Threading.Tasks;
using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;

namespace Lykke.Snow.AuditService.Domain.Services
{
    public interface IAuditEventService
    {
        Task<PaginatedResponse<IAuditModel<AuditDataType>>> GetAll(AuditTrailFilter<AuditDataType> filter, int? skip, int? take);
    }
}


