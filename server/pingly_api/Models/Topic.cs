using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace pingly_api.Models
{
    public class Topic
    {
        public Guid Id { get; set; }


        // The public topic address
        // Example:
        // server-alerts-x92kd8
        public string Name { get; set; } = null!;


        public string? Description { get; set; }


        // Allows users to subscribe
        // without special permission
        public bool IsPublic { get; set; } = true;


        public DateTime CreatedAt { get; set; }



        // Navigation properties

        public ICollection<Message> Messages { get; set; }
            = new List<Message>();


        public ICollection<Subscriber> Subscribers { get; set; }
            = new List<Subscriber>();
    }
}