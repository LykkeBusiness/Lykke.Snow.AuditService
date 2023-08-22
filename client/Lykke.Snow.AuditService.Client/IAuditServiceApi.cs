﻿// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Client.Model;
using Lykke.Snow.AuditService.Client.Model.Rfq;
using MarginTrading.Backend.Contracts.Rfq;
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
        /// Endpoint for getting rfq audit events with filters
        /// </summary>
        /// <param name="request"></param>
        /// <param name="refinedEditType"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [Get("/api/audit/rfq")]
        Task<PaginatedResponse<AuditModel<AuditDataTypeContract>>> GetRfqAuditEvents(GetAuditEventsRequest<RfqOperationState> request, RfqRefinedEditActionTypeContract? refinedEditType, int? skip = null, int? take = null);
    }
}
