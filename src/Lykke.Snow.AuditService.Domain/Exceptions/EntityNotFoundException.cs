// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

namespace Lykke.Snow.AuditService.Domain.Exceptions
{
    public sealed class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string key) : base($"The requested entity with key {key} was not found.")
        {
            Data.Add("key", key);
        }

        public EntityNotFoundException(int key) : base($"The requested entity with key {key} was not found.")
        {
            Data.Add("key", key);
        }
        
        public EntityNotFoundException(int[] keys) : base($"One or more requested entities with keys {string.Join(", ", keys)} were not found.")
        {
            Data.Add("key", keys);
        }

        public EntityNotFoundException()
        {
        }
    }
}