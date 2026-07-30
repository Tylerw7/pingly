using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pingly_api.Models
{
    public class DeviceSubscription
    {
        public Guid Id { get; set; }

        public Guid TopicId { get; set; }

        public Guid DeviceId { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation

        public Topic Topic { get; set; } = null!;

        public Device Device { get; set; } = null!;

    }
}