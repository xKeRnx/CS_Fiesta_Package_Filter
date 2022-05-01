using System;
using System.Net;
using System.Reflection;
using System.Net.Sockets;

using Filter.Login.Handlers;

using Filter.Instances.Networking;

namespace Filter.Login.Networking
{
    internal class LoginClient : Client, IDisposable
    {
        public LoginServer Server;

        private FiestaCrypto Crypto;

        public int AccountID = -1;

        public LoginClient(Socket AcceptedSocket, IPEndPoint AcceptedEndPoint, int Port)
            : base(AcceptedSocket, AcceptedEndPoint)
        {
            Server = new LoginServer(this, Port);
        }

        public void SendCrypto()
        {
            if (IsConnected != 0) { return; }

            Crypto = new FiestaCrypto(2);

            Packet ServerPacket = new Packet(Login2TypeServer.SetXor);
            ServerPacket.WriteShort(2);

            SendPacket(ServerPacket);
        }

        public void SendPacket(Packet ServerPacket)
        {
            if (IsConnected != 0) { return; }

            ServerPacket.ToArray(out byte[] PacketBuffer);
            ServerPacket.Dispose();

            Send(PacketBuffer);
        }

        public override void Connected() { }

        public override void ConnectFailed(SocketException Exception) { }

        public override void Received(byte[] Buffer)
        {
            if (IsConnected != 0) { return; }
            else if (Crypto != null) { Crypto.Crypt(Buffer, 0, Buffer.Length); }

            Packet ClientPacket = new Packet(Buffer);

            if (!ClientPacket.SetHeaderAndType()) { }
            else if (Program.LoginHandlers.HasHandler(ClientPacket.Header, ClientPacket.Type))
            {
                try
                {
                    MethodInfo PacketMethod = Program.LoginHandlers.GetHandler(ClientPacket.Header, ClientPacket.Type);

                    Action PacketAction = Program.LoginHandlers.GetAction(PacketMethod, this, ClientPacket);
                    PacketAction();
                }
                catch { SendPacket(ClientPacket); }
            }
            else { Server.SendPacket(ClientPacket); }
        }

        public override void Disconnected()
        {
            Server.Disconnect();
        }
    }

}
