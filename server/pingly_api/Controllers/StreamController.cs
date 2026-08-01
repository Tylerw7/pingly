using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.ServerSentEvents;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pingly_api.Models;
using pingly_api.services;
using pingly_api.SSE;


// Purpose of this end point
//--------------------------
// StreamController
//        │
//        ▼
// Does topic exist?
//        │
//        ▼
// Create SseClient
//        │
//        ▼
// Register connection
//        │
//        ▼
// Keep HTTP connection open



namespace pingly_api.Controllers
{
    [ApiController]
    [Route("stream")]
    public class StreamController : ControllerBase
    {
        private readonly SseConnectionManager _connectionManager;
        private readonly TopicService _topicService;

        public StreamController(SseConnectionManager connectionManager, TopicService topicService)
        {
            _connectionManager = connectionManager;
            _topicService = topicService;
        }

        [HttpGet("{topicName}")]
        public async Task<IResult> Stream(
            string topicName,
            CancellationToken cancellationToken
        )
        {
            var topic = await _topicService.GetAsync(topicName);

            if (topic == null) return TypedResults.NotFound();

            var client = _connectionManager.AddConnection(topic.Name);

            async IAsyncEnumerable<SseItem<string>> GetEvents()
            {
                try
                {
                    await foreach (
                        var message in client.Channel.Reader.ReadAllAsync(
                            cancellationToken
                        )
                    )
                    {
                        yield return message;
                    }
                }
                finally
                {
                    _connectionManager.RemoveConnection(
                    topic.Name,
                    client.Id);
                }
            }

            return TypedResults.ServerSentEvents(GetEvents());
        }
    }
}