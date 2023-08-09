using AutoMapper;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Client.Model.Request;
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

            CreateMap<GetRfqAuditTrailRequest, AuditTrailFilter<AuditDataType>>()
                .ForMember(dest => dest.DataTypes, opt => opt.MapFrom(src => new AuditDataType[] { AuditDataType.Rfq }))
                .ForMember(dest => dest.ActionTypeDetails, opt => opt.MapFrom(src => src.State));
        }
    }
}