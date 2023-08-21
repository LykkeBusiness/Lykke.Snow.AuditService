// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

namespace Lykke.Snow.AuditService.Domain.Exceptions
{
    public sealed class EntityAlreadyExistsException : Exception
    {
        public EntityAlreadyExistsException(object entity) : base("The entity already exists in the database.")
        {
            Data.Add("entity", entity);
        }
        
        public EntityAlreadyExistsException()
        {
        }
    }
}