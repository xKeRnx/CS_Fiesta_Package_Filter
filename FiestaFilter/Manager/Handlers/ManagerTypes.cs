using System;
using System.Reflection;
using System.Collections.Generic;

using Filter.Instances.Handlers;

namespace Filter.Manager.Handlers
{
    internal enum Manager2TypeServer : ushort
    {
        Ping = 4,
        SetXor = 7
    }

    internal enum Manager3TypeServer : ushort
    {
        Transfer = 15
    }

    internal enum Manager4TypeClient : ushort
    {
        SelectCharacter = 1
    }

    internal enum Manager4TypeServer : ushort
    {
        SendIP = 3
    }

    internal enum Manager8TypeClient : ushort
    {
        WhisperMessage = 12,
        PartyMessage = 20
    }

    internal enum Manager8TypeServer : ushort
    {
        WhisperMessage = 13,
        WhisperMessageTo = 15,
        WorldMessage = 17
    }

    internal enum Manager21TypeClient : ushort
    {
        RemoveFriend = 5
    }

    public enum Manager15TypeServer : ushort
    {
        Question = 1,
    }

    internal enum Manager22TypeServer : ushort
    {
        KingdomQuestList = 29
    }

    internal enum Manager25TypeServer : ushort
    {
        Roar = 2
    }

    internal enum Manager29TypeClient : ushort
    {
        GuildMessage = 115
    }

    internal enum Manager38TypeClient : ushort
    {
        AcademyPort = 31,
        AcademyMessage = 104
    }

    internal class ManagerHandlerLoader : HandlerLoader
    {
        public ManagerHandlerLoader()
        {
            foreach (var PacketHandler in FilterAssembly.FindHandlers<ManagerPacketHandler>())
            {
                ManagerPacketHandler PacketAttribute = PacketHandler.PacketAttribute;

                MethodInfo PacketMethod = PacketHandler.MethodInfo;

                if (!Handlers.ContainsKey(PacketAttribute.Header)) { Handlers.Add(PacketAttribute.Header, new Dictionary<ushort, MethodInfo>()); }

                if (!Handlers[PacketAttribute.Header].ContainsKey(PacketAttribute.Type)) { Handlers[PacketAttribute.Header].Add(PacketAttribute.Type, PacketMethod); }
            }
        }
    }
}