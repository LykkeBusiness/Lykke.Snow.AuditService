using System.Collections.Generic;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;

namespace Lykke.Snow.AuditService.Domain.Services
{
    public interface IObjectDiffService
    {
        string GetJsonDiff<T>(T oldState, T newState);
        string GetJsonDiff(string oldStateJson, string newStateJson);

        /// <summary>
        /// Generates new diff from creatation where oldState is empty json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newState"></param>
        /// <returns></returns>
        string GenerateNewJsonDiff<T>(T newState);
        
        /// <summary>
        /// Filters audit events based on given json diff filter. (based on json properties - and their values if needed)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="auditEvents"></param>
        /// <param name="jsonDiffFilter"></param>
        /// <returns></returns>
        IEnumerable<IAuditModel<AuditDataType>> FilterBasedOnJsonDiff(IList<IAuditModel<AuditDataType>> auditEvents, JsonDiffFilter jsonDiffFilter);
    }
}