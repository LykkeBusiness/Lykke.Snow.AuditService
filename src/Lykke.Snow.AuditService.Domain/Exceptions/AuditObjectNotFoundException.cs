using System;

namespace Lykke.Snow.AuditService.Domain.Exceptions
{
    public class AuditObjectNotFoundException : Exception
    {
        public AuditObjectNotFoundException(string dataType, string dataReference) : base($"Audit object with type {dataType} and reference {dataReference} was not found.")
        {
            Data.Add("dataType", dataType);
            Data.Add("dataReference", dataReference);
        }
    }
}
