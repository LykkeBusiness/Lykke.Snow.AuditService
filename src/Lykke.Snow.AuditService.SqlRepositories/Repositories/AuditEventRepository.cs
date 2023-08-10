using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Repositories;
using Lykke.Snow.AuditService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Snow.AuditService.SqlRepositories.Repositories
{
    public class AuditEventRepository : IAuditEventRepository
    {
        private readonly Lykke.Common.MsSql.IDbContextFactory<AuditDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public AuditEventRepository(Lykke.Common.MsSql.IDbContextFactory<AuditDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task AddAsync(IAuditModel<AuditDataType> auditEvent)
        {
            await using var context = _contextFactory.CreateDataContext();

            var entity = _mapper.Map<AuditEventEntity>(auditEvent);
            await context.Events.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task<IList<IAuditModel<AuditDataType>>> GetAllAsync(AuditTrailFilter<AuditDataType> filter)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context
                    .Events
                    .AsNoTracking()
                    .ApplyFilter(filter);

                var total = await query.CountAsync();

                query = query.OrderByDescending(x => x.Timestamp);

                var contents = await query.ToListAsync();

                return contents;
            }
        }
    }
}