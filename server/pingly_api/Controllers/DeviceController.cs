using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using pingly_api.DTOs;
using pingly_api.services;

namespace pingly_api.Controllers
{
    [ApiController]
    [Route("api/devices")]
    public class DeviceController : ControllerBase
    {
        private readonly DeviceService _service;

        public DeviceController(DeviceService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<DeviceResponseDto>> Register(
            RegisterDeviceRequestDto request
        )
        {
            return Ok(await _service.RegisterAsync(request));
        }
    }
}