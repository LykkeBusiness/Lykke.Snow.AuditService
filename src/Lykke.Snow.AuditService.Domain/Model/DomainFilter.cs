using System.Collections.Generic;

using Lykke.Snow.AuditService.Domain.Enum;


namespace Lykke.Snow.AuditService.Domain.Model
{
    public class DomainFilter
    {
        /// <summary>
        /// AuditDomain (e.g. Assets, Axle, Meteor etc.) to filter mapping
        /// </summary>
        public IReadOnlyDictionary<AuditDataType, IList<JsonDiffFilter>> Filters { get; }
        
        public DomainFilter(IDictionary<AuditDataType, IList<JsonDiffFilter>> filters)
        {
            Filters = filters as IReadOnlyDictionary<AuditDataType, IList<JsonDiffFilter>> ?? 
                new Dictionary<AuditDataType, IList<JsonDiffFilter>>();
        }
    }
}