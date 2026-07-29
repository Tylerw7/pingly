using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pingly_api.DTOs
{
    public class PublishMessageRequestDto
    {
        public string? Title { get; set; }

        public string Body { get; set; } = null!;
    }
}