// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Client.Model.Request;
using Lykke.Snow.AuditService.Client.Model.Request.Rfq;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Enum.ActionTypes;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.AuditService.Settings;
using Lykke.Snow.Common.Startup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Snow.AuditService.Controllers
{
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
        public async Task<PaginatedResponse<IAuditModel<AuditDataType>>> Rfq([FromQuery] GetRfqAuditEventsRequest request, int? skip = null, int? take = null)
        {
            var filter = _mapper.Map<AuditTrailFilter<AuditDataType>>(request);

            JsonDiffFilter jsonDiffFilter = null!;

            if (request.ActionType == AuditEventType.Edition && request.RefinedEditActionType == RfqRefinedEditActionType.StatusChanged)
                jsonDiffFilter = new JsonDiffFilter(nameof(request.State));

            var result = await _auditEventService.GetAll(filter, jsonDiffFilter, skip, take);
            
            return result;
        }

        [HttpGet("rfq/csv")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Csv([FromQuery] ExportAuditEventsToCsvRequest request)
        {
            var result = await _auditEventService.GetAll(request.Filter ?? new AuditTrailFilter<AuditDataType>());

            this.TrySetCsvSettings(_auditServiceSettings.CsvExportSettings.Delimiter, _auditServiceSettings.CsvExportSettings.ShouldOutputHeader);

            return Ok(result.Contents);
        }
    }
}