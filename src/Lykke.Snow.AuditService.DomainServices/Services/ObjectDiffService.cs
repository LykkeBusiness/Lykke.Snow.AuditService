// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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

        public IEnumerable<IAuditModel<AuditDataType>> FilterBasedOnJsonDiff(IList<IAuditModel<AuditDataType>> auditEvents, JsonDiffFilter jsonDiffFilter)
        {
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
                
                foreach(JProperty property in properties)
                {
                    if(property.Name != jsonDiffFilter.PropertyName)
                        continue;

                    yield return auditEvent;
                }
            }
        }

    }
}