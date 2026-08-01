using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.ServerSentEvents;
using System.Threading.Tasks;

namespace pingly_api.SSE
{
    public class SseConnectionManager
    {
        private readonly ConcurrentDictionary<
            string,
            ConcurrentDictionary<Guid, SseClient>> _connections = new(StringComparer.OrdinalIgnoreCase);

        public SseClient AddConnection(string topicName)
        {
            var client = new SseClient(topicName);

            var topicConnections =
                _connections.GetOrAdd(topicName,
                _ => new ConcurrentDictionary<Guid, SseClient>());

            topicConnections[client.Id] = client;

            return client;
        }


        public void RemoveConnection(string topicName, Guid clientId)
        {
            if (!_connections.TryGetValue(
                    topicName,
                    out var topicConnections))
            {
                return;
            }

            topicConnections.TryRemove(clientId, out _);

            if (topicConnections.IsEmpty)
            {
                _connections.TryRemove(
                    topicName,
                    out _);
            }
        }

        public async Task BroadcastAsync(string topicName, SseItem<string> message)
        {
            if (!_connections.TryGetValue(
                topicName,
                out var topicConnections
            ))
            {
                return;
            }

            foreach (var client in topicConnections.Values)
            {
                await client.Channel.Writer.WriteAsync(message);
            }
        }

        public async Task BroadcastHeartbeatAsync(string topicName)
        {
            var heartbeat =
                new SseItem<string>(string.Empty,
                eventType: "heartbeat");

            await BroadcastAsync(
                topicName,
                heartbeat
            );
        }


        public IEnumerable<string> GetActiveTopic()
        {
            return _connections.Keys;
        }
    }
}