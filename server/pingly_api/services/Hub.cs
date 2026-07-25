using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace pingly_api.services
{

    public interface IHub
    {
        Subscription Subscribe(string topic);
        void Publish(string topic, byte[] payload);
    }

    public class Hub : IHub
    {
        // Nested map: topic name → { subscriber ID → subscription }.
        // Outer dict: many topics, safe for concurrent add/remove.
        // Inner dict-as-set: many subscribers per topic, keyed by ID for
        //   O(1) Unsubscribe.
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Subscription>> _topics = new();

        public Subscription Subscribe(string topic)
        {
            // GetOrAdd: returns the existing set for this topic, or atomically
            // creates + inserts + returns a fresh one. This is how "topics
            // spring into existence on first use" is implemented.
            var subs = _topics.GetOrAdd(topic, _ => new ConcurrentDictionary<Guid, Subscription>());
            var sub = new Subscription(topic, this);
            subs[sub.Id] = sub;
            return sub;
        }

        public void Publish(string topic, byte[] payload)
        {
            // If nobody's subscribed, nothing to fan out.
            // (The message still got saved to the DB by the handler — this is
            // just the live-delivery path.)
            if (!_topics.TryGetValue(topic, out var subs)) return;

            foreach (var sub in subs.Values)
            {
                // TryWrite is non-blocking:
                //   - room in the channel → writes, returns true
                //   - channel full (slow subscriber) → drops, returns false
                // Either way, we move on. One slow client can't hold up
                // anyone else or block the publisher.
                sub.Writer.TryWrite(payload);
            }
        }

        // Called only by Subscription.Dispose(). `internal` restricts callers
        // to this assembly, so external code can't accidentally unsubscribe
        // someone else.
        internal void Unsubscribe(string topic, Guid subscriberId)
        {
            if (_topics.TryGetValue(topic, out var subs))
            {
                subs.TryRemove(subscriberId, out _);
                // Deliberately NOT removing the empty topic entry — doing so
                // races with concurrent Subscribes. An empty ConcurrentDictionary
                // costs ~200 bytes; negligible.
            }
        }
    }

    public class Subscription : IDisposable
    {
        public Guid Id { get; } = Guid.NewGuid();

        private readonly Channel<byte[]> _channel;
        private readonly string _topic;
        private readonly Hub _hub;
        private bool _disposed;

        // Writer is internal — only the hub pushes into it.
        // Reader is public — the SSE handler pulls from it.
        // Splitting reader/writer visibility enforces flow direction at the
        // type level; no accidental publishing from a handler.
        internal ChannelWriter<byte[]> Writer => _channel.Writer;
        public ChannelReader<byte[]> Reader => _channel.Reader;

        internal Subscription(string topic, Hub hub)
        {
            _topic = topic;
            _hub = hub;

            // Bounded channel: fixed size, backpressure policy required.
            _channel = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(16)
            {
                // When full: silently drop new writes. Alternatives are
                // DropOldest / DropNewest / Wait — DropWrite matches the
                // "best-effort live delivery" contract, and history is
                // recoverable via the /messages endpoint.
                FullMode = BoundedChannelFullMode.DropWrite,

                // Optimization hints: exactly one SSE handler reads from
                // this channel; many threads may write to us concurrently.
                SingleReader = true,
                SingleWriter = false,
            });
        }

        public void Dispose()
        {
            // Idempotent — `using var` and explicit Dispose can both fire.
            if (_disposed) return;
            _disposed = true;

            // Wake up any awaiting ReadAsync so the SSE handler unwinds
            // cleanly instead of hanging forever.
            _channel.Writer.TryComplete();

            _hub.Unsubscribe(_topic, Id);
        }
    }
}