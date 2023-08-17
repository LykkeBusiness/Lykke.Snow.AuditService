// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace Lykke.Snow.AuditService.Domain.Services
{
    /// <summary>
    /// Service interface for handling rfq-related logic - saving audit events.
    /// </summary>
    public interface IAuditEventProcessor
    {
        Task ProcessEvent<T>(T rabbitMqEvent);
        string GetJsonDiff(string oldState, string newState);
    }
}