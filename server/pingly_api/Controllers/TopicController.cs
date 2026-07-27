using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;



namespace pingly_api.Controllers
{
    [ApiController]
    [Route("topics")]
    public class TopicController : ControllerBase
    {
        private static readonly Regex TopicNameRegex = new(
        @"^[a-zA-Z0-9_-]{1,64}$", RegexOptions.Compiled);

        private const int MaxBodyBytes = 8 * 1024;

        //private readonly AppDbContext _db;
        private readonly Channel<string> _channel;


        public TopicController(Channel<string> channel)
        {
            //_db = db;
            _channel = channel;
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "New status ok." });
        }

        [HttpPost("messages")]
        public async Task<IActionResult> message(string message)
        {
            await _channel.Writer.WriteAsync(message);
            return Accepted(new
            {
                qued = message
            });
        }

    }
}