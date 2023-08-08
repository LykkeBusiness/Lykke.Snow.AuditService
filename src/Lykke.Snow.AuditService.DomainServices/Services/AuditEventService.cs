using System.Threading.Tasks;
using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Services;

namespace Lykke.Snow.AuditService.DomainServices.Services
{
    public class AuditEventService : IAuditEventService
    {
        public Task<PaginatedResponse<IAuditModel<AuditDataType>>> GetAll(AuditTrailFilter<AuditDataType> filter, int? skip, int? take)
        {
            throw new System.NotImplementedException();
        }
    }
}