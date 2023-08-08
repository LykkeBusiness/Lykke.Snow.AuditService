using Common;
using JsonDiffPatchDotNet;
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
    }
}