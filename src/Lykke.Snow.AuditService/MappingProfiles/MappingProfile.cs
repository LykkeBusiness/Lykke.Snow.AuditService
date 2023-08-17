// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using AutoMapper;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Client.Model.Rfq;
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

            CreateMap<GetRfqAuditEventsRequest, AuditTrailFilter<AuditDataType>>()
                .ForMember(dest => dest.DataTypes, opt => opt.MapFrom(src => new AuditDataType[] { AuditDataType.Rfq }))
                .ForMember(dest => dest.AuditEventTypeDetails, opt => opt.MapFrom(src => src.State));
        }
    }
}