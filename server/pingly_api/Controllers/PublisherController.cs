using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pingly_api.Channels;
using pingly_api.DTOs;
using pingly_api.services;

namespace pingly_api.Controllers
{
    [ApiController]
    [Route("api")]
    public class PublisherController : ControllerBase
    {
        private readonly TopicService _topicService;
        private readonly NotificationChannels _channel;

        public PublisherController(TopicService topicService, NotificationChannels channel)
        {
            _topicService = topicService;
            _channel = channel;

        }

        [HttpPost("{topicName}")]
        public async Task<IActionResult> Publish(string topicName, PublishMessageRequestDto request)
        {
            var success = await _topicService.PublishAsync(
                topicName,
                request,
                _channel
            );

            if (!success) return NotFound();

            return Accepted();
        }

    }
}