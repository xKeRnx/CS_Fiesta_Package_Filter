using System;
using System.Net;
using System.Timers;
using System.Reflection;
using System.Net.Sockets;

using Filter.Zone.Handlers;

using Filter.Instances.Networking;
using Filter.Manager.Networking;

using Filter.Utilities;

namespace Filter.Zone.Networking
{
    internal class ZoneServer : Client, IDisposable
    {
        public ZoneClient Client;

        public FiestaCrypto Crypto;

        public ZoneServer(ZoneClient TransferClient, int Port) : base(IPAddress.Parse("127.0.0.1"), Port) { Client = TransferClient; }

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

            if (!ServerPacket.SetHeaderAndType()) { Disconnect(); }
            else if (Program.ZoneHandlers.HasHandler(ServerPacket.Header, ServerPacket.Type))
            {
                try
                {
                    MethodInfo PacketMethod = Program.ZoneHandlers.GetHandler(ServerPacket.Header, ServerPacket.Type);

                    Action PacketAction = Program.ZoneHandlers.GetAction(PacketMethod, this, ServerPacket);
                    PacketAction();
                }
                catch { SendPacket(ServerPacket); }
            }
            else { Client.SendPacket(ServerPacket); }
        }

        public override void Disconnected() { }
    }
}
