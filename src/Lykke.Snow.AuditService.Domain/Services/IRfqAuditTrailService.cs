// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Lykke.Snow.AuditService.Domain.Model;
using MarginTrading.Backend.Contracts.Events;

namespace Lykke.Snow.AuditService.Domain.Services
{
    /// <summary>
    /// Service interface for handling rfq-related logic - saving audit events.
    /// </summary>
    public interface IRfqAuditTrailService
    {
        string GetEventUsername(RfqEvent rfqEvent);
        Task ProcessRfqEvent(RfqEvent rfqEvent);
        string GetRfqJsonDiff(RfqEvent evt, AuditObjectState? existingObject = null);
    }
}