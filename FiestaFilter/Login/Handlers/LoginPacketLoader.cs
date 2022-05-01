using System;

using Filter.Instances.Handlers;

namespace Filter.Login.Handlers
{
    internal class LoginPacketHandler : PacketHandlerAttribute
    {
        public LoginPacketHandler(byte Header, byte Type) : base(Header, Type) { }
    }
}
