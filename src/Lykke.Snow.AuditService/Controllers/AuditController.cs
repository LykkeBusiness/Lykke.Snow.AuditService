using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Client.Model.Request;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Enum.ActionTypes;
using Lykke.Snow.AuditService.Domain.Services;
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
        private readonly IObjectDiffService _objectDiffService;

        public AuditController(IMapper mapper,
            IAuditEventService auditEventService, 
            IObjectDiffService objectDiffService)
        {
            _mapper = mapper;
            _auditEventService = auditEventService;
            _objectDiffService = objectDiffService;
        }

        // Special filters for RFQ
        // Status = Finished & Status = PriceRequested & Status = PriceChanged
        // ActionType = Creation & Edition & StatusChanged
        [HttpGet("rfq")]
        [ProducesResponseType(typeof(PaginatedResponse<IAuditModel<AuditDataType>>), (int)HttpStatusCode.OK)]
        public async Task<PaginatedResponse<IAuditModel<AuditDataType>>> Rfq([FromQuery] GetRfqAuditTrailRequest request, int? skip = null, int? take = null)
        {
            var filter = _mapper.Map<AuditTrailFilter<AuditDataType>>(request);

            var result = await _auditEventService.GetAll(filter, skip, take);
            
            //if(request.ActionType == RfqActionType.StatusChanged)
            //    _objectDiffService.FilterBasedOnJsonDiff(result, )

            return result;
        }
    }
}