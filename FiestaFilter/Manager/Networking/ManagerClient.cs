using System;
using System.Net;
using System.Timers;
using System.Reflection;
using System.Net.Sockets;
using System.Xml;

using Filter.Manager.Handlers;

using Filter.Instances.Networking;

using Filter.Utilities;

namespace Filter.Manager.Networking
{
    internal class ManagerClient : Client
    {
        public ManagerServer Server;

        private FiestaCrypto Crypto;

        private Timer PingTimer;

        public Guid GUID = Guid.NewGuid();

        public int AccountID = -1;
        public int CharacterID;
        public string CharacterName;
        public int AdminLevel;

        private int Port;

        public DateTime LastRoar = DateTime.MinValue;

        public bool SentMessages;

        public ManagerClient(Socket AcceptedSocket, IPEndPoint AcceptedEndPoint, int TrasnferPort)
            : base(AcceptedSocket, AcceptedEndPoint)
        {
            Port = TrasnferPort;

            Program.ManagerLoggedIn.Add(GUID, this);

            Server = new ManagerServer(this, Port);

            StartPing();
        }

        public void SendCrypto()
        {
            if (IsConnected != 0) { return; }

            Crypto = new FiestaCrypto(2);

            Packet ServerPacket = new Packet(Manager2TypeServer.SetXor);
            ServerPacket.WriteUShort(2);

            SendPacket(ServerPacket);
        }

        public void StartPing()
        {
            PingTimer = new Timer();
            PingTimer.AutoReset = true;
            PingTimer.Elapsed += PingTimer_Elapsed;
            PingTimer.Interval = 2000;
            PingTimer.Start();
        }

        private void PingTimer_Elapsed(Object Sender, ElapsedEventArgs Args)
        {
            Packet ServerPacket = new Packet(Manager2TypeServer.Ping);

            SendPacket(ServerPacket);
        }

        public void SendPacket(Packet ServerPacket)
        {
            if (IsConnected != 0) { return; }

            byte[] PacketBuffer;

            ServerPacket.ToArray(out PacketBuffer);
            ServerPacket.Dispose();

            Send(PacketBuffer);
        }

        public override void Connected() { }

        public override void ConnectFailed(SocketException Exception) { }

        public override void Received(byte[] Buffer)
        {
            if (IsConnected != 0) { return; }
            if (Crypto != null) { Crypto.Crypt(Buffer, 0, Buffer.Length); }
            Packet ClientPacket = new Packet(Buffer);

            if (!ClientPacket.SetHeaderAndType()) { }
            else if (Program.ManagerHandlers.HasHandler(ClientPacket.Header, ClientPacket.Type))
            {
                try
                {
                    MethodInfo PacketMethod = Program.ManagerHandlers.GetHandler(ClientPacket.Header, ClientPacket.Type);

                    Action PacketAction = Program.ManagerHandlers.GetAction(PacketMethod, this, ClientPacket);
                    PacketAction();
                }
                catch { SendPacket(ClientPacket); }
            }
            else { Server.SendPacket(ClientPacket); }
        }

        public void SendConnectMessages() { }

        public void SendDisconnectMessages() { }

        public override void Disconnected()
        {
            if (Program.ManagerLoggedIn.ContainsKey(GUID)) { Program.ManagerLoggedIn.Remove(GUID); }

            PingTimer.Stop();
            PingTimer.Dispose();

            Server.Disconnect();
        }
    }

}
