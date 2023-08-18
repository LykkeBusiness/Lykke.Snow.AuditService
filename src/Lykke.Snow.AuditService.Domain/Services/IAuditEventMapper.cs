// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;

namespace Lykke.Snow.AuditService.Domain.Services
{
    /// <summary>
    /// Interface for mapping different events into AuditModel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAuditEventMapper<T>
    {
        /// <summary>
        /// AuditDataType for the event (the entity type)
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        AuditDataType GetAuditDataType(T evt);
        
        /// <summary>
        /// The unique data reference for the entity represented by the event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        string GetDataReference(T evt);

        /// <summary>
        /// Returns the 'state' of the entity - which the diff will be built based on. (Entity payload)
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        string GetStateInJson(T evt);
        
        /// <summary>
        /// Method that maps event into AuditModel. The diff caused by the event must be passed.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="diff"></param>
        /// <returns></returns>
        AuditModel<AuditDataType> MapAuditEvent(T evt, string diffWithPreviousState);
    }
}