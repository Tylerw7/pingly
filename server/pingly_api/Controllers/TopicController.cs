using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pingly_api.Data;

namespace pingly_api.Controllers
{
    [ApiController]
    [Route("topics")]
    public class TopicController : ControllerBase
    {

        private readonly AppDbContext _db;

        public TopicController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "New status ok." });
        }
    }
}