// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Client.Model.Request.Rfq;
using Lykke.Snow.AuditService.Domain.Enum;
using Refit;

namespace Lykke.Snow.AuditService.Client
{
    /// <summary>
    /// AuditService client API interface.
    /// </summary>
    [PublicAPI]
    public interface IAuditServiceApi
    {
        /// <summary>
        /// Endpoint for getting rfq audit events with filters.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [Get("/api/audit/rfq")]
        Task<PaginatedResponse<AuditModel<AuditDataType>>> GetRfqAuditEvents(GetRfqAuditEventsRequest request, int? skip = null, int? take = null);

        /// <summary>
        /// Endpoint for getting rfq audit events with filters in CSV format.
        /// Please set Accept: text/csv header
        /// </summary>
        /// <param name="request"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [Get("/api/audit/rfq/csv")]
        Task<IList<AuditModel<AuditDataType>>> ExportRfqAuditEventsToCSV(GetRfqAuditEventsRequest request, int? skip = null, int? take = null);
    }
}
