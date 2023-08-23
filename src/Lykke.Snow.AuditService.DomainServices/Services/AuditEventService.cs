// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Repositories;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.Common;

using Refit;


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

        public async Task<PaginatedResponse<IAuditModel<AuditDataType>>> GetAll(AuditTrailFilter<AuditDataType> filter, IDictionary<AuditDataType, List<JsonDiffFilter>> domainFilters, int? skip = null, int? take = null)
        {
            (skip, take) = PaginationUtils.ValidateSkipAndTake(skip, take);
            
            List<IAuditModel<AuditDataType>> results = new List<IAuditModel<AuditDataType>>();
            
            // Collect all datatypes from domain filters
            filter.DataTypes = domainFilters.Select(x => x.Key).ToArray();

            // First, we go to the database without json filters - only with common filters from unified request (AuditTrailFilter)
            var unfilteredResults = await _auditEventRepository.GetAllAsync(filter);

            foreach(var domainFilter in domainFilters)
            {
                var typeFiltered = unfilteredResults.Where(x => x.DataType == domainFilter.Key).ToList();

                // Apply domain filters (json diff filters)
                if(domainFilter.Value.Count > 0)
                    typeFiltered = _objectDiffService.FilterBasedOnJsonDiff(typeFiltered, jsonDiffFilters: domainFilter.Value).ToList();
                
                results.AddRange(typeFiltered);
            }

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