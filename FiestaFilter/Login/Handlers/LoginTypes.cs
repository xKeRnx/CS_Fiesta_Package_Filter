using System;
using System.Reflection;
using System.Collections.Generic;

using Filter.Instances.Handlers;

namespace Filter.Login.Handlers
{
    internal enum Login2TypeServer : ushort
    {
        SetXor = 7
    }

    internal enum Login3TypeClient : ushort
    {
        Version = 1,
        WorldSelect = 11,
        Token = 32
    }

    internal enum Login3TypeServer : ushort
    {
        LoginError = 9,
        WorldListNew = 10,
        Transfer = 12,
        WorldListResend = 28
    }

    internal class LoginHandlerLoader : HandlerLoader
    {
        public LoginHandlerLoader()
        {
            foreach (var PacketHandler in FilterAssembly.FindHandlers<LoginPacketHandler>())
            {
                LoginPacketHandler PacketAttribute = PacketHandler.PacketAttribute;

                MethodInfo PacketMethod = PacketHandler.MethodInfo;

                if (!Handlers.ContainsKey(PacketAttribute.Header)) { Handlers.Add(PacketAttribute.Header, new Dictionary<ushort, MethodInfo>()); }

                if (!Handlers[PacketAttribute.Header].ContainsKey(PacketAttribute.Type)) { Handlers[PacketAttribute.Header].Add(PacketAttribute.Type, PacketMethod); }
            }
        }
    }
}
