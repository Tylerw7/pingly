using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace pingly_api.Models
{
    public class Message
    {
        public Guid Id { get; set; }


        public Guid TopicId { get; set; }


        public string? Title { get; set; }


        public string Body { get; set; } = null!;


        // 0 = normal
        // 1 = important
        // 2 = urgent
        public int Priority { get; set; } = 0;


        public DateTime CreatedAt { get; set; }



        public Topic Topic { get; set; } = null!;
    }
}