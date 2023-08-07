using System.Threading.Tasks;

using Lykke.Snow.AuditService.Domain.Model;

namespace Lykke.Snow.AuditService.Domain.Repositories
{
    public interface IAuditEventRepository
    {
        Task AddAsync(AuditEvent auditEvent);
    }
}