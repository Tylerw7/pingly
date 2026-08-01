using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pingly_api.DTOs.Devices
{
    public class SubscribeDeviceRequestDto
    {
        public string TopicName { get; set; } = null!;
    }
}