// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Lykke.Snow.AuditService.Client.Model.Rfq
{
    /// <summary>
    /// Refined (more detailed) action type for Rfq Audit events 
    /// </summary>
    public enum RfqRefinedEditActionTypeContract
    {
        /// <summary>
        /// Represents the audit events in which the "State" property has changed.
        /// </summary>
        StatusChanged = 0
    }
}