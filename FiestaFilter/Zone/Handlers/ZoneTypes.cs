using System;
using System.Reflection;
using System.Collections.Generic;

using Filter.Instances.Handlers;

namespace Filter.Zone.Handlers
{
    internal enum Zone2TypeServer : ushort
    {
        Ping = 4,
        SetXor = 7
    }

    internal enum Zone4TypeServer : ushort
    {
        CharacterInfo = 56,
        CharacterLook = 57,
        CharacterInfoEnd = 72
    }

    internal enum Zone6TypeClient : ushort
    {
        SHNHashes = 1,
        Ready = 3
    }

    internal enum Zone6TypeServer : ushort
    {
        ZoneTransfer = 10
    }

    internal enum Zone8TypeClient : ushort
    {
        NormalChat = 1,
        BeginInteract = 10,
        CloseNPC = 11,
        StopWalk = 18,
        StartWalk = 23,
        StartRun = 25,
        OpenNPC = 29,
        Emote = 32,
        ShoutChat = 30,
    }

    internal enum Zone8TypeServer : ushort
    {
        NormalChat = 2,
        MapNotice = 17,
        ShoutChat = 31
    }

    internal enum Zone9TypeClient : ushort
    {
        SelectObject = 1
    }

    internal enum Zone12TypeClient : ushort
    {
        DropItem = 7,
        BuyItem = 3,
        SellItem = 6,
        UseItem = 21
    }

    internal enum Zone15TypeClient : ushort
    {
        AnswerQuestion = 2
    }

    internal enum Zone15TypeServer : ushort
    {
        Question = 1
    }

    internal enum Zone19TypeServer : ushort
    {        
        TradeStart = 9,
        TradeQuit = 11,
        TradeFinish = 36
    }

    internal enum Zone19TypeClient : ushort
    {
        TradeSend = 1
    }

    internal enum Zone26TypeClient : ushort
    {
        CloseVendor = 4,
        EditButton = 16
    }

    internal class ZoneHandlerLoader : HandlerLoader
    {
        public ZoneHandlerLoader()
        {
            foreach (var PacketHandler in FilterAssembly.FindHandlers<ZonePacketHandler>())
            {
                ZonePacketHandler PacketAttribute = PacketHandler.PacketAttribute;

                MethodInfo PacketMethod = PacketHandler.MethodInfo;

                if (!Handlers.ContainsKey(PacketAttribute.Header)) { Handlers.Add(PacketAttribute.Header, new Dictionary<ushort, MethodInfo>()); }

                if (!Handlers[PacketAttribute.Header].ContainsKey(PacketAttribute.Type)) { Handlers[PacketAttribute.Header].Add(PacketAttribute.Type, PacketMethod); }
            }
        }
    }
}