using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pingly_api.DTOs.Devices
{
    public class UpdateDeviceRequestDto
    {
        public string ApnsToken { get; set; } = null!;
    }
}