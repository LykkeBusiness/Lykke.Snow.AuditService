// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;

namespace Lykke.Snow.AuditService.Domain.Services
{
    public interface IAuditEventService
    {
        /// <summary>
        /// Gets all audit events based on given filters
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="jsonDiffFilter"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        Task<PaginatedResponse<IAuditModel<AuditDataType>>> GetAll(
            AuditTrailFilter<AuditDataType> filter, 
            IDictionary<AuditDataType, List<JsonDiffFilter>> domainFilters,
            int? skip = null, 
            int? take = null);
    }
}


