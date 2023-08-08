using System.Threading.Tasks;
using MarginTrading.Backend.Contracts.Events;

namespace Lykke.Snow.AuditService.Domain.Services
{
    public interface IRfqAuditTrailService
    {
        Task ProcessRfqEvent(RfqEvent rfqEvent);
    }
}