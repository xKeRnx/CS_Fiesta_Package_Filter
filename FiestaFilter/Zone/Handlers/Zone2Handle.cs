using System;

using Filter.Instances.Networking;
using Filter.Zone.Networking;

using Filter.Zone.Handlers;

namespace Filter.Zone.Handlers
{
    internal class Zone2Handle
    {
        //Zone2 - Set Xor
        [ZonePacketHandler(2, 7)]
        public static void HandleSetXor(ZoneServer ServerSocket, Packet ServerPacket)
        {
            short Xor;
            
            if (!ServerPacket.ReadShort(out Xor))
            {
                ServerSocket.Disconnect();

                return;
            }

            ServerSocket.Crypto = new FiestaCrypto(Xor);
            ServerSocket.Client.SendCrypto();
            ServerSocket.Client.Receive();

            ServerPacket.Dispose();
        }
    }
}
