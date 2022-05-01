using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Filter.Instances.Networking;
using Filter.Manager.Networking;

namespace Filter.Manager.Handlers
{
    internal class Manager4Handle
    {
        //Manager4 - Send ZoneData
        [ManagerPacketHandler(4, 3)]
        public static void HandleZoneIP(ManagerServer ServerSocket, Packet ServerPacket)
        {
            string IPAddress;
            short Port;

            ServerPacket.ReadString(out IPAddress, 16);
            ServerPacket.ReadShort(out Port);

            ServerPacket.Dispose();

            if (Program.ServerList.Contains(Program.ServerIP)) { ServerSocket.Client.Disconnect(); return; }

            ServerPacket = new Packet(Manager4TypeServer.SendIP);
            ServerPacket.WriteString(Program.ServerIP, 16);
            ServerPacket.WriteShort(Port);

            ServerSocket.Client.SendPacket(ServerPacket);


            ServerPacket.Dispose();
        }
    }
}

