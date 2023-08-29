// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Client.Model;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Snow.AuditService.Controllers
{
    /// <summary>
    /// Main controller for Audit events. 
    /// A new action per each audit type is required.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/audit")]
    public class AuditController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuditEventService _auditEventService;

        public AuditController(IMapper mapper,
            IAuditEventService auditEventService)
        {
            _mapper = mapper;
            _auditEventService = auditEventService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(PaginatedResponse<IAuditModel<AuditDataType>>), (int)HttpStatusCode.OK)]
        public async Task<PaginatedResponse<IAuditModel<AuditDataType>>> GetAll([FromBody] GetAuditEventsRequest request, int? skip = null, int? take = null)
        {
            var filter = _mapper.Map<AuditTrailFilter<AuditDataType>>(request);
            
            var domainFilters = _mapper.Map<IDictionary<AuditDataType, List<JsonDiffFilter>>>(request.DomainFilters);

            var result = await _auditEventService.GetAll(filter, domainFilters, skip, take);
            
            return result;
        }
    }
}