using AutoMapper;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.SqlRepositories.Entities;

namespace Lykke.Snow.AuditService.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuditModel<AuditDataType>, AuditEventEntity>()
                .ReverseMap();

            CreateMap<AuditObjectState, AuditObjectStateEntity>()
                .ReverseMap();
        }
    }
}