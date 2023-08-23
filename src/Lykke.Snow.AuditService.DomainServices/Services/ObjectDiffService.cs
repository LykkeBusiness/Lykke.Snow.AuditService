// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Common;
using JsonDiffPatchDotNet;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lykke.Snow.AuditService.DomainServices.Services
{
    public class ObjectDiffService : IObjectDiffService
    {
        private readonly ILogger<ObjectDiffService> _logger;

        public ObjectDiffService(ILogger<ObjectDiffService> logger)
        {
            _logger = logger;
        }

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

        public IEnumerable<IAuditModel<AuditDataType>> FilterBasedOnJsonDiff(IList<IAuditModel<AuditDataType>> auditEvents, IEnumerable<JsonDiffFilter> jsonDiffFilters)
        {
            var hashset = jsonDiffFilters.Select(x => x.PropertyName).ToHashSet();

            foreach(var auditEvent in auditEvents)
            {
                if(string.IsNullOrEmpty(auditEvent.DataDiff))
                    continue;

                JObject jobject = new JObject();

                try 
                {
                    jobject = JObject.Parse(auditEvent.DataDiff);
                }
                catch(JsonReaderException e)
                {
                    _logger.LogError(e, "DataDiff was not a valid json. AuditEvent: {AuditEvent}", auditEvent.ToJson());
                    
                    throw;
                }

                var properties = jobject.Properties();
                
                if(!properties.Any(x => hashset.Contains(x.Name)))
                    continue;

                if(CheckJsonProperties(properties, jsonDiffFilters))
                    yield return auditEvent;
            }
        }
        
        public bool CheckJsonProperties(IEnumerable<JProperty> properties, IEnumerable<JsonDiffFilter> jsonDiffFilters)
        {
            foreach(var prop in properties)
            {
                var filter = jsonDiffFilters.FirstOrDefault(x => x.PropertyName == prop.Name);
                
                // There's no such filter, move on to the next property
                if(filter == null)
                    continue;
                
                // Filter does not care what value is, add to the result collection
                if(filter.Value == null)
                    return true;
                
                string? propValue = string.Empty;
                
                var x = prop.Value.Count();

                // Value initialization - there's no old value
                if(prop.Value.Count() == 1)
                    propValue = prop.Value[0]?.Value<string>();
                // Value transition, pick up the new value
                else if(prop.Value.Count() == 2)
                    propValue = prop.Value[1]?.Value<string>();

                // Values are equal, add  result collection
                if(filter.Value.ToString() == propValue)
                    return true;
            }

            return false;
        }
    }
}