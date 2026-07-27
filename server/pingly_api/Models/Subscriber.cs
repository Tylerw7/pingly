using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pingly_api.Models
{
    public class Subscriber
    {
        public Guid Id { get; set; }


        public Guid TopicId { get; set; }


        // Used for browsers
        public string? BrowserId { get; set; }


        // Used for mobile devices
        public Guid? DeviceId { get; set; }


        public DateTime CreatedAt { get; set; }



        public Topic Topic { get; set; } = null!;


        public Device? Device { get; set; }
    }
}