// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Snow.AuditService.Controllers
{
    [Route("api/[controller]")]
    public class IsAliveController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(
                new IsAliveResponse
                {
                    Version = PlatformServices.Default.Application.ApplicationVersion,
                    Env = Environment.GetEnvironmentVariable("ENV_INFO"),
#if DEBUG
                    IsDebug = true,
#else
                    IsDebug = false,
#endif
                    Name = Program.ApiName
                }
            );
        }
    }
}