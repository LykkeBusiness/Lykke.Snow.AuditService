using System.Threading.Tasks;
using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Repositories;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.Common;

namespace Lykke.Snow.AuditService.DomainServices.Services
{
    public class AuditEventService : IAuditEventService
    {
        private readonly IAuditEventRepository _auditEventRepository;

        public AuditEventService(IAuditEventRepository auditEventRepository)
        {
            _auditEventRepository = auditEventRepository;
        }

        public Task<PaginatedResponse<IAuditModel<AuditDataType>>> GetAll(AuditTrailFilter<AuditDataType> filter, int? skip, int? take)
        {
            (skip, take) = PaginationUtils.ValidateSkipAndTake(skip, take);

            return _auditEventRepository.GetAllAsync(filter, skip, take);
        }
    }
}