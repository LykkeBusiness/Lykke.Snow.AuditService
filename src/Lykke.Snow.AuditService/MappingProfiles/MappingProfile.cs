// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using AutoMapper;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Client.Model;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.SqlRepositories.Entities;
using MarginTrading.Backend.Contracts.Rfq;

namespace Lykke.Snow.AuditService.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuditModel<AuditDataType>, AuditEventEntity>()
                .ReverseMap();

            CreateMap<AuditDataTypeContract, AuditDataType>()
                .ReverseMap();

            CreateMap<AuditObjectState, AuditObjectStateEntity>()
                .ReverseMap();
            
            CreateMap<JsonDiffFilter, JsonDiffFilterContract>()
                .ReverseMap();

            // GetAuditEventRequest with ActionTypeDetails = 'RfqOperationState'
            CreateMap<GetAuditEventsRequest<RfqOperationState>, AuditTrailFilter<AuditDataType>>()
                .ForMember(dest => dest.DataTypes, opt => opt.MapFrom(src => new AuditDataType[] { AuditDataType.Rfq }));
        }
    }
}