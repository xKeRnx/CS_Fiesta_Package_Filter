using System;

using Filter.Instances.Networking;
using Filter.Manager.Networking;

namespace Filter.Manager.Handlers
{
    internal class Manager2Handle
    {
        //Manager2 - Set Xor
        [ManagerPacketHandler(2, 7)]
        public static void HandleSetXor(ManagerServer ServerSocket, Packet ServerPacket)
        {
            short Xor;

            if (!ServerPacket.ReadShort(out Xor)) { ServerSocket.Disconnect(); }
            else
            {
                ServerSocket.Crypto = new FiestaCrypto(Xor);

                ServerSocket.Client.SendCrypto();
                ServerSocket.Client.Receive();
            }

            ServerPacket.Dispose();
        }
    }
}
