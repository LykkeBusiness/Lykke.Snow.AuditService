using System.Threading.Tasks;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using MarginTrading.Backend.Contracts.Events;

namespace Lykke.Snow.AuditService.Domain.Services
{
    public interface IRfqAuditTrailService
    {
        string GetEventUsername(RfqEvent rfqEvent);
        Task ProcessRfqEvent(RfqEvent rfqEvent);
        AuditModel<AuditDataType> GetAuditEvent(RfqEvent rfqEvent, string username, string jsonDiff);
        string GetRfqJsonDiff(RfqEvent evt, AuditObjectState? existingObject = null);
    }
}