// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Services;

namespace Lykke.Snow.AuditService.DomainServices.Services
{
    /// <summary>
    /// Factory class for creating AuditObjectState instances - created for standardized instantiation
    /// </summary>
    public class AuditObjectStateFactory : IAuditObjectStateFactory
    {
        public AuditObjectState Create(AuditDataType auditDataType, string dataReference, string currentStateInJson, DateTime lastModified)
        {
            return new AuditObjectState(dataType: auditDataType, dataReference: dataReference, stateInJson: currentStateInJson, lastModified: lastModified);
        }
    }

}