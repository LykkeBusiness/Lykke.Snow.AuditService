// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;


using AutoMapper;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Client.Model;
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

            CreateMap<AuditDataTypeContract, AuditDataType>()
                .ReverseMap();

            CreateMap<AuditObjectState, AuditObjectStateEntity>()
                .ReverseMap();
            
            CreateMap<JsonDiffFilter, JsonDiffFilterContract>()
                .ReverseMap();

            CreateMap<GetAuditEventsRequest, AuditTrailFilter<AuditDataType>>();
            
            CreateMap<KeyValuePair<AuditDataTypeContract, List<JsonDiffFilterContract>>, KeyValuePair<AuditDataType, List<JsonDiffFilter>>>()
                .ConstructUsing(x => new KeyValuePair<AuditDataType, List<JsonDiffFilter>>(Map(x.Key), x.Value.Select(x => Map(x)).ToList()));
        }
        private AuditDataType Map(AuditDataTypeContract auditDataTypeContract)
        {
            return (AuditDataType)auditDataTypeContract;
        }
        
        private JsonDiffFilter Map(JsonDiffFilterContract jsonDiffFilterContract)
        {
            return new JsonDiffFilter(jsonDiffFilterContract.PropertyName, jsonDiffFilterContract.Value);
        }
    }
    
}