using System.Threading.Tasks;
using Lykke.Snow.Audit;

namespace Lykke.Snow.AuditService.Domain.Repositories
{
    public interface IAuditEventRepository
    {
        Task<AuditModel<T>> GetAsync<T>(string dataReference);
        Task AddAsync<T>(AuditModel<T> auditEvent);
    }
}