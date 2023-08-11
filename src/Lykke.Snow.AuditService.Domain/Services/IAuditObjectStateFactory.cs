// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;

namespace Lykke.Snow.AuditService.Domain.Services
{
    /// <summary>
    /// Factory interface for creating AuditObjectState
    /// </summary>
    public interface IAuditObjectStateFactory
    {
        AuditObjectState Create(AuditDataType auditDataType, 
            string dataReference,
            string currentStateInJson, 
            DateTime lastModified);
    }
}