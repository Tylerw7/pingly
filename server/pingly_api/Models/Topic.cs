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
        public int Id { get; set; }

        [MaxLength(64)]
        public required string Name { get; set; }

        [JsonIgnore]
        public string? OwnerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property — EF Core uses this to model the one-to-many
        // relationship. Not serialized to JSON (would cause infinite loops).
        [JsonIgnore]
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}