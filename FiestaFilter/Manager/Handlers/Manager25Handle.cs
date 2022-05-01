using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Filter.Instances.Networking;
using Filter.Manager.Networking;

namespace Filter.Manager.Handlers
{
    internal class Manager25Handle
    {
        [ManagerPacketHandler(25, 2)]
        public static void HandleRoar(ManagerServer ServerSocket, Packet ClientPacket)
        {
            byte CharnameLength;
            byte RoarLength;
            string Charname;
            string RoarMsg;
            ClientPacket.ReadByte(out CharnameLength);
            ClientPacket.ReadByte(out RoarLength);
            ClientPacket.ReadString(out Charname, CharnameLength);
            ClientPacket.ReadString(out RoarMsg, RoarLength);

            if (RoarLength- CharnameLength > 70)
            {
               
            }
            else
            {
                ServerSocket.Client.SendPacket(ClientPacket);
            }

            ClientPacket.Dispose();
        }
    }
}
