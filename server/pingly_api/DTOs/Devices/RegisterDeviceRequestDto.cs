using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pingly_api.DTOs
{
    public class RegisterDeviceRequestDto
    {
        public string ApnsToken { get; set; } = null!;

    }
}