using System;

using Filter.Login.Networking;
using Filter.Instances.Networking;

namespace Filter.Login.Handlers
{
    internal class Login2Handle
    {
        //Login2 - SetXor
        [LoginPacketHandler(2, 7)]
        public static void HandleSetXor(LoginServer ServerSocket, Packet ServerPacket)
        {
            if (ServerSocket.IsConnected != 0)
            {
                ServerPacket.Dispose();

                return;
            }

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
