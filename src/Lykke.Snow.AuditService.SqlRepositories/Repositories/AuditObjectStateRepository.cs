// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.MsSql;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Exceptions;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Repositories;
using Lykke.Snow.AuditService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.AuditService.SqlRepositories.Repositories
{
    public class AuditObjectStateRepository : IAuditObjectStateRepository
    {
        private readonly Lykke.Common.MsSql.IDbContextFactory<AuditDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ConcurrentDictionary<(string dataType, string dataReference), SemaphoreSlim> _locks =
            new ConcurrentDictionary<(string dataType, string dataReference), SemaphoreSlim>();

        public AuditObjectStateRepository(Lykke.Common.MsSql.IDbContextFactory<AuditDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task AddOrUpdate(AuditObjectState objectState)
        {
            var lockObject = _locks.GetOrAdd(
                (objectState.DataType.ToString(), objectState.DataReference),
                _ => new SemaphoreSlim(1, 1));

            await lockObject.WaitAsync();

            try
            {
                await using var context = _contextFactory.CreateDataContext();

                 var existingEntity = await context.AuditObjectStates
                     .SingleOrDefaultAsync(x =>
                         x.DataType == objectState.DataType && x.DataReference == objectState.DataReference);

                 if (existingEntity == null)
                 {
                     await TryAddAsync(context, objectState);
                 }
                 else
                 {
                     await TryUpdateAsync(context, objectState, existingEntity);
                 }
            }
            finally
            {
                lockObject.Release();
            }
        }

        public async Task<AuditObjectState> GetByDataReferenceAsync(AuditDataType dataType, string dataReference)
        {
            await using var context = _contextFactory.CreateDataContext();

            var entity = await context.AuditObjectStates
                .SingleOrDefaultAsync(x => x.DataType == dataType && x.DataReference == dataReference);
            
            return _mapper.Map<AuditObjectState>(entity);
        }


        private async Task TryAddAsync(AuditDbContext context, AuditObjectState objectState)
        {
            var entity = _mapper.Map<AuditObjectStateEntity>(objectState);
            await context.AuditObjectStates.AddAsync(entity);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.ValueAlreadyExistsException())
                {
                    throw new EntityAlreadyExistsException(entity: entity);
                }

                throw;
            }
        }

        private async Task TryUpdateAsync(AuditDbContext context, 
            AuditObjectState objectState,
            AuditObjectStateEntity existingEntity)
        {
            _mapper.Map(objectState, existingEntity);
            context.AuditObjectStates.Update(existingEntity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                throw new EntityNotFoundException(existingEntity.Oid);
            }
        }
    }
}