using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.IO;
using System.Linq.Expressions;
using Filter.Instances.Networking;
using Filter.Zone.Networking;

using Filter.Zone.Handlers;

using Filter.Utilities;

namespace Filter.Zone.Handlers
{
    internal class Zone12Handle
    {
        [ZonePacketHandler(12, 105)]
        public static void HandleItemMix(ZoneClient ClientSocket, Packet ClientPacket)
        {
            byte Powder;
            byte FItem;
            byte SItem;
            ClientPacket.ReadByte(out Powder);
            ClientPacket.ReadByte(out FItem);
            ClientPacket.ReadByte(out SItem);

            if (FItem == SItem) 
            {
                if (ClientSocket.CharacterName != string.Empty)
                {
                    //Messager.Roar(Program.Botname, Program.Duplimsg);
                    Messager.World(true, ClientSocket.CharacterName + " " + Program.Duplimsg);
                }
                else 
                {
                    //Messager.Roar(Program.Botname, Program.Duplimsg);
                    Messager.World(true, Program.Duplimsg);
                }
                
            }
            else 
            { 
                ClientSocket.Server.SendPacket(ClientPacket); 
            }

            ClientPacket.Dispose();
        }
    }
}
