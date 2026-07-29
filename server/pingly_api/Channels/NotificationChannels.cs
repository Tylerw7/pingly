using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using pingly_api.Models.Events;

namespace pingly_api.Channels
{
    public class NotificationChannels
    {
        public Channel<PublishMessage> Channel { get; }
        public NotificationChannels()
        {
            Channel = System.Threading.Channels.Channel.CreateUnbounded<PublishMessage>();
        }
    }
}