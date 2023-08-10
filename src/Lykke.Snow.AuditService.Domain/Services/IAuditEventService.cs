using System.Threading.Tasks;
using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;

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
            JsonDiffFilter? jsonDiffFilter = null,
            int? skip = null, 
            int? take = null);
    }
}


