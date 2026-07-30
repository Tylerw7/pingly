using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pingly_api.Channels;

namespace pingly_api.services
{
    public class NotificationWorker : BackgroundService
    {
        private readonly NotificationChannels _channel;
        private readonly ILogger<NotificationWorker> _logger;

        public NotificationWorker(
            NotificationChannels channel,
            ILogger<NotificationWorker> logger
        )
        {
            _channel = channel;
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

                // Step 5:
                // Broadcast SSE

                // Step 6:
                // Send APNS
            }
        }
    }
}