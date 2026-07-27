using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pingly_api.Models
{
    public class Device
    {
        public Guid Id { get; set; }


        // Apple Push Notification token
        public string ApnsToken { get; set; } = null!;


        public string Platform { get; set; } = "ios";


        public DateTime CreatedAt { get; set; }



        public ICollection<Subscriber> Subscribers { get; set; }
            = new List<Subscriber>();
    }
}