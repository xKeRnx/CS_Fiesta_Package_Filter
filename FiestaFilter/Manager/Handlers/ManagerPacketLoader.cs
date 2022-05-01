using System;

using Filter.Instances.Handlers;

namespace Filter.Manager.Handlers
{
    internal class ManagerPacketHandler : PacketHandlerAttribute
    {
        public ManagerPacketHandler(byte Header, ushort Type) : base(Header, Type) { }
    }
}

