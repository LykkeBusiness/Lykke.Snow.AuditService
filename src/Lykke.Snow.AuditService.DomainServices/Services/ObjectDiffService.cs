using System.Linq.Expressions;

using Common;
using JsonDiffPatchDotNet;

using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Services;

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

        public IAuditModel<T> FilterBasedOnJsonDiff<T>(Expression<System.Func<T>> expr)
        {
            throw new System.NotImplementedException();
        }

    }
}