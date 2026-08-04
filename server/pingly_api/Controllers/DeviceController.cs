using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using pingly_api.DTOs;
using pingly_api.DTOs.Devices;
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


        [HttpPut("{deviceId:guid}")]
        public async Task<IActionResult> Update(
                Guid deviceId,
                UpdateDeviceRequestDto request)
        {
            var updated = await _service.UpdateTokenAsync(deviceId, request);

            if (!updated)
                return NotFound();

            return NoContent();
        }


        [HttpPost("{deviceId:guid}/subscriptions")]
        public async Task<IActionResult> Subscribe(
            Guid deviceId,
            SubscribeDeviceRequestDto request)
        {
            var success = await _service.SubscribeAsync(
                deviceId,
                request.TopicName);

            if (!success)
                return NotFound();

            return NoContent();
        }


        [HttpDelete("{deviceId:guid}/subscriptions/{topicName}")]
        public async Task<IActionResult> Unsubscribe(
            Guid deviceId,
            string topicName)
        {
            var success = await _service.UnsubscribeAsync(
                deviceId,
                topicName);

            if (!success)
                return NotFound();

            return NoContent();
        }
        

        [HttpGet("{deviceId:guid}/subscriptions")]
        public async Task<ActionResult<List<string>>> GetSubscriptions(
            Guid deviceId)
        {
            return Ok(await _service.GetSubscriptionsAsync(deviceId));
        }
    }
}