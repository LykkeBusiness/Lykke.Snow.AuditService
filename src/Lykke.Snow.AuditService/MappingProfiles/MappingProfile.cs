using AutoMapper;

using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.SqlRepositories.Entities;

namespace Lykke.Snow.AuditService.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuditEvent, AuditEventEntity>()
                .ReverseMap();
        }
    }
}