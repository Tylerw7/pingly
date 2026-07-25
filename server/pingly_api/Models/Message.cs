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
        public int Id { get; set; }
        public int TopicId { get; set; }

        // Carrier field: NOT stored in DB, DOES appear in JSON as "topic".
        // Set by the handler when returning messages so clients see the
        // topic name alongside the message.
        [NotMapped]
        public string? TopicName { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }
        public required string Body { get; set; }
        public int Priority { get; set; } = 3;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}