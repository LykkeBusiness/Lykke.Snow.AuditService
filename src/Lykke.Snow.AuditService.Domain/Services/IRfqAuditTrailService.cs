using System.Threading.Tasks;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;

using MarginTrading.Backend.Contracts.Events;

namespace Lykke.Snow.AuditService.Domain.Services
{
    public interface IRfqAuditTrailService
    {
        Task ProcessRfqEvent(RfqEvent rfqEvent);
        AuditModel<AuditDataType> GetAuditEvent(RfqEvent rfqEvent, string jsonDiff);
    }
}