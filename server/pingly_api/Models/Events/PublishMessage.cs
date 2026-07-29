using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pingly_api.Models.Events
{
    public class PublishMessage
    {
        public Guid MessageId { get; init; }
        public Guid TopicId { get; init; }

        public string TopicName { get; init; } = null!;

        public string? Title { get; init; }

        public string Body { get; init; } = null!;

        public DateTime CreatedAt { get; init; }
    }
}