using System;
using System.Net;
using System.Reflection;
using System.Net.Sockets;

using Filter.Instances.Networking;

namespace Filter.Login.Networking
{
    internal class LoginServer : Client, IDisposable
    {
        public LoginClient Client;

        public FiestaCrypto Crypto;

        public LoginServer(LoginClient TransferClient, int Port) : base(IPAddress.Parse("127.0.0.1"), Port) { Client = TransferClient; }

        public void SendPacket(Packet ClientPacket)
        {
            if (IsConnected != 0) { return; }

            byte[] PacketBuffer;

            ClientPacket.ToArray(Crypto, out PacketBuffer);
            ClientPacket.Dispose();

            Send(PacketBuffer);
        }

        public override void Connected() { Receive(); }

        public override void ConnectFailed(SocketException Exception)
        {
            Disconnect();

            Client.Disconnect();
        }

        public override void Received(byte[] Buffer)
        {
            if (IsConnected != 0) { return; }

            Packet ServerPacket = new Packet(Buffer);

            if (!ServerPacket.SetHeaderAndType()) { }
            else if (Program.LoginHandlers.HasHandler(ServerPacket.Header, ServerPacket.Type))
            {
                try
                {
                    MethodInfo PacketMethod = Program.LoginHandlers.GetHandler(ServerPacket.Header, ServerPacket.Type);

                    Action PacketAction = Program.LoginHandlers.GetAction(PacketMethod, this, ServerPacket);
                    PacketAction();
                }
                catch { SendPacket(ServerPacket); }
            }
            else { Client.SendPacket(ServerPacket); }
        }

        public override void Disconnected() { Client.Disconnect(); }
    }
}
