// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Client.Model;
using Lykke.Snow.AuditService.Client.Model.Rfq;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.AuditService.Settings;
using MarginTrading.Backend.Contracts.Rfq;
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
        private readonly AuditServiceSettings _auditServiceSettings;

        public AuditController(IMapper mapper,
            IAuditEventService auditEventService,
            AuditServiceSettings auditServiceSettings)
        {
            _mapper = mapper;
            _auditEventService = auditEventService;
            _auditServiceSettings = auditServiceSettings;
        }

        [HttpGet("rfq")]
        [ProducesResponseType(typeof(PaginatedResponse<IAuditModel<AuditDataType>>), (int)HttpStatusCode.OK)]
        public async Task<PaginatedResponse<IAuditModel<AuditDataType>>> Rfq(
            [FromQuery] GetAuditEventsRequest<RfqOperationState> request, 
            [FromQuery] RfqRefinedEditActionTypeContract RefinedEditType, 
            int? skip = null, int? take = null)
        {
            var filter = _mapper.Map<AuditTrailFilter<AuditDataType>>(request);

            var jsonDiffFilters = new List<JsonDiffFilter>();

            if (request.ActionType == AuditEventType.Edition && RefinedEditType == RfqRefinedEditActionTypeContract.StatusChanged)
                jsonDiffFilters.Add(new JsonDiffFilter(nameof(RfqContract.State)));

            var result = await _auditEventService.GetAll(filter, jsonDiffFilters, skip, take);
            
            return result;
        }
    }
}