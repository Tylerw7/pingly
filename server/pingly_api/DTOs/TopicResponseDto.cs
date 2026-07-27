using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pingly_api.DTOs
{
    public class TopicResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsPublic { get; set; }

        public DateTime CreatedAt { get; set; }


        public string PublishUrl { get; set; } = null!;
    }
}