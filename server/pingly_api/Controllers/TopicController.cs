using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using pingly_api.DTOs;
using pingly_api.services;



namespace pingly_api.Controllers
{
    [ApiController]
    [Route("topics")]
    public class TopicsController : ControllerBase
    {
        private readonly TopicService _service;


        public TopicsController(
            TopicService service)
        {
            _service = service;
        }



        [HttpPost]
        public async Task<ActionResult<TopicResponseDto>> Create(
            CreateTopicRequestDto request)
        {
            var topic =
                await _service.CreateAsync(request);


            return Ok(topic);
        }



        [HttpGet("{name}")]
        public async Task<ActionResult> Get(string name)
        {
            var topic =
                await _service.GetAsync(name);


            if (topic == null)
                return NotFound();


            return Ok(topic);
        }



        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            return Ok(
                await _service.GetAllAsync());
        }



        [HttpDelete("{name}")]
        public async Task<ActionResult> Delete(string name)
        {
            var deleted =
                await _service.DeleteAsync(name);


            if (!deleted)
                return NotFound();


            return NoContent();
        }
}
}