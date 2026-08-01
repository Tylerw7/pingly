using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pingly_api.Channels;
using pingly_api.SSE;

namespace pingly_api.services
{
    public class NotificationWorker : BackgroundService
    {
        private readonly NotificationChannels _channel;
        private readonly SseConnectionManager _sseManager;
        private readonly ILogger<NotificationWorker> _logger;

        public NotificationWorker(
            NotificationChannels channel,
            SseConnectionManager sseManager,
            ILogger<NotificationWorker> logger
        )
        {
            _channel = channel;
            _sseManager = sseManager;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var message in _channel.Channel.Reader.ReadAllAsync(stoppingToken))
            {
                _logger.LogInformation(
                    "Processing message {MessageId} for topic {Topic}",
                    message.MessageId,
                    message.TopicName
                    );

                var json =
                    System.Text.Json.JsonSerializer.Serialize(
                        new
                        {
                            id = message.MessageId,
                            title = message.Title,
                            message = message.Body,
                            createdAt = message.CreatedAt,
                        }
                    );

                var sseMessage =
                    new System.Net.ServerSentEvents.SseItem<string>(
                        json,
                        eventType: "message")
                    {
                        ReconnectionInterval =
                            TimeSpan.FromSeconds(5)
                    };

                await _sseManager.BroadcastAsync(
                    message.TopicName,
                    sseMessage
                );    

                // Step 6:
                // Send APNS
            }
        }
    }
}