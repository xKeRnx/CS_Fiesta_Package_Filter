using System;
using System.Net;
using System.Timers;
using System.IO;
using System.Reflection;
using System.Net.Sockets;

using Filter.Zone.Handlers;

using Filter.Instances.Networking;
using Filter.Manager.Networking;

using Filter.Zone.Game;

using Filter.Utilities;

using Filter.Manager.Handlers;
using System.Text;

namespace Filter.Zone.Networking
{
    internal class ZoneClient : Client
    {
        public byte ZoneID;

        public ZoneServer Server;

        private FiestaCrypto Crypto;

        public ManagerClient Manager;

        public bool InNPC;

        public bool InTrade;

        public int SlotID;

        public string CharacterName;

        public bool SentMessages;

        public Guid GUID = Guid.NewGuid();

        public int PosX = 0;
        public int PosY = 0;
        public Question Question { get; set; }
        public bool HasQuestion = false;

        public ZoneClient(Socket AcceptedSocket, IPEndPoint AcceptedEndPoint, byte ZID, int ConnectPort) : base(AcceptedSocket, AcceptedEndPoint)
        {
            ZoneID = ZID;
            Program.ZoneLoggedIn.Add(GUID, this);
            Server = new ZoneServer(this, ConnectPort);
        }

        public void SendCrypto()
        {
            if (IsConnected != 0) { return; }

            Crypto = new FiestaCrypto(2);

            Packet ServerPacket = new Packet(Zone2TypeServer.SetXor);
            ServerPacket.WriteShort(2);

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
            else if (Program.ZoneHandlers.HasHandler(ClientPacket.Header, ClientPacket.Type))
            {
                try
                {
                    MethodInfo PacketMethod = Program.ZoneHandlers.GetHandler(ClientPacket.Header, ClientPacket.Type);

                    Action PacketAction = Program.ZoneHandlers.GetAction(PacketMethod, this, ClientPacket);
                    PacketAction();
                }
                catch { SendPacket(ClientPacket); }
            }
            else { 
                Server.SendPacket(ClientPacket); 
            }
        }

        public override void Disconnected()
        {
            if (Program.ZoneLoggedIn.ContainsKey(GUID)) { Program.ZoneLoggedIn.Remove(GUID); }

            Server.Disconnect();
        }
    }
}
