using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;

namespace Lykke.Snow.AuditService.Domain.Services
{
    public interface IAuditEventMapper<T>
    {
        AuditDataType GetAuditDataType(T evt);
        string GetDataReference(T evt);
        AuditModel<AuditDataType> MapAuditEvent(T evt, string diff);
        string GetStateInJson(T evt);
    }
}