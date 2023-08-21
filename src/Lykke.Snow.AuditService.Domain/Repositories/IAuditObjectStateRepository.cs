// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;

namespace Lykke.Snow.AuditService.Domain.Repositories
{
    public interface IAuditObjectStateRepository
    {
        Task<AuditObjectState> GetByDataReferenceAsync(AuditDataType dataType, string dataReference);
        Task AddOrUpdate(AuditObjectState objectState);
    }
}