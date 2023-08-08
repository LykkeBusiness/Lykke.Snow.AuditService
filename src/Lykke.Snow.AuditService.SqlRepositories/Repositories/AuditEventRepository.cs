using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.MsSql;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Repositories;
using Lykke.Snow.AuditService.SqlRepositories.Entities;

namespace Lykke.Snow.AuditService.SqlRepositories.Repositories
{
    public class AuditEventRepository : IAuditEventRepository
    {
        private readonly Lykke.Common.MsSql.IDbContextFactory<AuditDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public AuditEventRepository(IDbContextFactory<AuditDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task AddAsync<T>(AuditModel<T> auditEvent)
        {
            await using var context = _contextFactory.CreateDataContext();

            var entity = _mapper.Map<AuditEventEntity>(auditEvent);
            await context.Events.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public Task<AuditModel<T>> GetAsync<T>(string dataReference)
        {
            throw new System.NotImplementedException();
        }
    }
}