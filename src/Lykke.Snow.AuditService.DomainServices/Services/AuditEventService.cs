using System.Collections.Generic;
using System.Linq;
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

        private readonly IObjectDiffService _objectDiffService;

        public AuditEventService(IAuditEventRepository auditEventRepository, 
            IObjectDiffService objectDiffService)
        {
            _auditEventRepository = auditEventRepository;
            _objectDiffService = objectDiffService;
        }

        public async Task<PaginatedResponse<IAuditModel<AuditDataType>>> GetAll(AuditTrailFilter<AuditDataType> filter, JsonDiffFilter? jsonDiffFilter, int? skip, int? take)
        {
            (skip, take) = PaginationUtils.ValidateSkipAndTake(skip, take);
            
            var results = await _auditEventRepository.GetAllAsync(filter);

            if(jsonDiffFilter != null)
                results = _objectDiffService.FilterBasedOnJsonDiff(results, jsonDiffFilter).ToList();

            var total = results.Count;

            if (skip.HasValue && take.HasValue)
                results = results.Skip(skip.Value).Take(take.Value).ToList();

            var result = new PaginatedResponse<IAuditModel<AuditDataType>>(
                contents: results as IReadOnlyList<IAuditModel<AuditDataType>>,
                start: skip ?? 0,
                size: results.Count,
                totalSize: total
            );

            return result;
        }
    }
}