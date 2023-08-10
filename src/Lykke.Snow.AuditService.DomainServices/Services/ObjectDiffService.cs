using System.Collections.Generic;
using Common;
using JsonDiffPatchDotNet;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Services;

using Newtonsoft.Json.Linq;

namespace Lykke.Snow.AuditService.DomainServices.Services
{
    public class ObjectDiffService : IObjectDiffService
    {
        public string GenerateNewJsonDiff<T>(T newState)
        {
            var oldStateJson = "{}";
            var newStateJson = newState.ToJson();

            var jdp = new JsonDiffPatch();
            var jsonDiff = jdp.Diff(oldStateJson, newStateJson);

            return jsonDiff;
        }

        public string GetJsonDiff<T>(T oldState, T newState)
        {
            var oldStateJson = oldState.ToJson();
            var newStateJson = newState.ToJson();
            
            var jdp = new JsonDiffPatch();
            var jsonDiff = jdp.Diff(oldStateJson, newStateJson);

            return jsonDiff;
        }

        public string GetJsonDiff(string oldStateJson, string newStateJson)
        {
            var jdp = new JsonDiffPatch();
            var jsonDiff = jdp.Diff(oldStateJson, newStateJson);

            return jsonDiff;
        }

        public IEnumerable<IAuditModel<AuditDataType>> FilterBasedOnJsonDiff(IList<IAuditModel<AuditDataType>> auditEvents, JsonDiffFilter jsonDiffFilter)
        {
            foreach(var auditEvent in auditEvents)
            {
                var diff = auditEvent.DataDiff;

                JObject jobject = JObject.Parse(auditEvent.DataDiff);

                var properties = jobject.Properties();
                
                foreach(JProperty property in properties)
                {
                    if(property.Name != jsonDiffFilter.PropertyName)
                        continue;

                    if(jsonDiffFilter.Value == null)
                        yield return auditEvent;
                        
                    if(property.Value.ToString() == jsonDiffFilter.Value)
                        yield return auditEvent;
                }
            }
        }

    }
}