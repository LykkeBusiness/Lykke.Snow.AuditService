using System;
using System.Linq.Expressions;

using Lykke.Snow.Audit.Abstractions;

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
        IAuditModel<T> FilterBasedOnJsonDiff<T>(Expression<Func<T>> expr);
    }
}