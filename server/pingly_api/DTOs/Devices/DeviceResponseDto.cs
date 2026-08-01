using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pingly_api.DTOs
{
    public class DeviceResponseDto
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}