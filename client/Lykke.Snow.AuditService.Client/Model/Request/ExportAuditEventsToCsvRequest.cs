using System.ComponentModel.DataAnnotations;

using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;

namespace Lykke.Snow.AuditService.Client.Model.Request
{
    /// <summary>
    /// Request class that's used to export audit events in CSV format
    /// </summary>
    public class ExportAuditEventsToCsvRequest
    {
        /// <summary>
        /// Filter for csv export 
        /// </summary>
        public AuditTrailFilter<AuditDataType>? Filter { get; set; }
    }
}