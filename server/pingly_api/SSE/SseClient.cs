using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.ServerSentEvents;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace pingly_api.SSE
{
    public class SseClient
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string TopicName { get; }

        public Channel<SseItem<string>> Channel { get; }

        public SseClient(string topicName)
        {
            TopicName = topicName;

            Channel = System.Threading.Channels.Channel
                .CreateUnbounded<SseItem<string>>();
        }
    }
}