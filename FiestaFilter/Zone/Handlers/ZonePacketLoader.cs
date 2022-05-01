using System;

using Filter.Instances.Handlers;

namespace Filter.Zone.Handlers
{
    internal class ZonePacketHandler : PacketHandlerAttribute
    {
        public ZonePacketHandler(byte Header, ushort Type) : base(Header, Type) { }
    }
}

