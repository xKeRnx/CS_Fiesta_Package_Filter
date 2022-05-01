using System;
using System.Linq;
using System.IO;

using Filter.Instances.Networking;
using Filter.Zone.Networking;

using Filter.Zone.Handlers;

using Filter.Utilities;
using System.Threading;

namespace Filter.Zone.Handlers
{
    internal class Zone6Handle
    {
        //Zone6 - Send SHN Hashes
        [ZonePacketHandler(6, 1)]
        public static void HandleSHNHashes(ZoneClient ClientSocket, Packet ClientPacket)
        {
            byte[] Unwanted = new byte[2];
            if (!ClientPacket.ReadBytes(Unwanted, 2)) 
            { 
                ClientSocket.Disconnect(); 
            }
            else
            {
                string CharacterName;
                string shnhash;
                if (!ClientPacket.ReadString(out CharacterName, 20)) 
                { 
                    ClientSocket.Disconnect(); 
                }
                else if (CharacterName == null)
                {
                    ClientSocket.Disconnect(); 
                }else if (!ClientPacket.ReadString(out shnhash, 1568))
                {
                    ClientSocket.Disconnect();
                }
                else 
                {
                    ClientSocket.CharacterName = CharacterName;
                    ClientSocket.Server.SendPacket(ClientPacket); 
                    
                }
            }

            ClientPacket.Dispose();
        }


        //Zone6 - ZoneToZone Transfer
        [ZonePacketHandler(6, 10)]
        public static void HandleZoneIP(ZoneServer ServerSocket, Packet ServerPacket)
        {
            byte[] Unk00 = new byte[10]; ServerPacket.ReadBytes(Unk00);
            string IPAddress; ServerPacket.ReadString(out IPAddress, 16);
            short Port; ServerPacket.ReadShort(out Port);
            byte[] Unk01 = new byte[2]; ServerPacket.ReadBytes(Unk01);

            ServerPacket.Dispose();

            if (Program.ServerList.Contains(Program.ServerIP)) { ServerSocket.Client.Disconnect(); return; }

            ServerPacket = new Packet(Zone6TypeServer.ZoneTransfer);
            ServerPacket.WriteBytes(Unk00);
            ServerPacket.WriteString(Program.ServerIP, 16);
            ServerPacket.WriteShort(Port);
            ServerPacket.WriteBytes(Unk01);

            ServerSocket.Client.SendPacket(ServerPacket);


            ServerPacket.Dispose();
        }
    }
}
