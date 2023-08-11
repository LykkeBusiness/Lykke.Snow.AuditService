using System;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Services;

namespace Lykke.Snow.AuditService.DomainServices.Services
{
    public class AuditObjectStateFactory : IAuditObjectStateFactory
    {
        public AuditObjectState Create(AuditDataType auditDataType, string dataReference, string currentStateInJson, DateTime lastModified)
        {
            return new AuditObjectState(dataType: auditDataType, dataReference: dataReference, stateInJson: currentStateInJson, lastModified: lastModified);
        }
    }

}