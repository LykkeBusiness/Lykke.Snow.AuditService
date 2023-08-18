// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace Lykke.Snow.AuditService.Domain.Services
{
    /// <summary>
    /// Service interface for handling events upon consumption
    /// </summary>
    public interface IAuditEventProcessor
    {
        /// <summary>
        /// Main method that processes the event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rabbitMqEvent"></param>
        /// <returns></returns>
        Task ProcessEvent<T>(T rabbitMqEvent, IAuditEventMapper<T> eventMapper);
        
        /// <summary>
        /// The method that returns json diff based on newState and oldState of the entity.
        /// </summary>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        /// <returns></returns>
        string GetJsonDiff(string oldState, string newState);
    }
}