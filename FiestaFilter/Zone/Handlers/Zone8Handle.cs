using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlClient;
using System.Threading;
using Filter.Instances.Networking;
using Filter.Zone.Networking;

using Filter.Zone.Handlers;

using Filter.Utilities;

namespace Filter.Zone.Handlers
{
    internal class Zone8Handle
    {
        //Zone8 - Stop Walk
        [ZonePacketHandler(8, 18)]
        public static void HandleStopWalk(ZoneClient ClientSocket, Packet ClientPacket)
        {
            ClientPacket.ReadInt(out int newX);
            ClientPacket.ReadInt(out int newY);

            ClientSocket.PosX = newX;
            ClientSocket.PosY = newY;

            ClientSocket.Server.SendPacket(ClientPacket);

            ClientPacket.Dispose();
        }

        //Zone8 - Start Walk
        [ZonePacketHandler(8, 23)]
        public static void HandleStartWalk(ZoneClient ClientSocket, Packet ClientPacket)
        {
            if (ClientSocket.InTrade) { ClientSocket.Disconnect(); }
            else
            {
                ClientPacket.ReadInt(out int oldX);
                ClientPacket.ReadInt(out int oldY);
                ClientPacket.ReadInt(out int newX);
                ClientPacket.ReadInt(out int newY);

                ClientSocket.PosX = newX;
                ClientSocket.PosY = newY;

                ClientSocket.Server.SendPacket(ClientPacket);
            }
            ClientPacket.Dispose();
        }

        //Zone8 - Start Run
        [ZonePacketHandler(8, 25)]
        public static void HandleStartRun(ZoneClient ClientSocket, Packet ClientPacket)
        {
            if (ClientSocket.InTrade) { ClientSocket.Disconnect(); }
            else
            {
                ClientPacket.ReadInt(out int oldX);
                ClientPacket.ReadInt(out int oldY);
                ClientPacket.ReadInt(out int newX);
                ClientPacket.ReadInt(out int newY);

                ClientSocket.PosX = newX;
                ClientSocket.PosY = newY;

                ClientSocket.Server.SendPacket(ClientPacket);
            }
            ClientPacket.Dispose();
        }
    }
}
