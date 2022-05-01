using System;
using System.Net;
using System.Net.Sockets;

using Filter.Instances.Networking;

using Filter.Utilities;

namespace Filter.Zone.Networking
{
    internal class ZoneListener : Listener
    {
        private byte ZoneID;
        private int BindPort;
        private int ConnectPort;

        public ZoneListener(byte ZID, int BPort, int CPort) : base(50000, BPort)
        {
            ZoneID = ZID;
            BindPort = BPort;
            ConnectPort = CPort;
            Writer.Write(ConsoleType.Socket, "Added Zone listen on {0} with ZoneID {1}", BindPort, ZoneID);
            Accept();
        }

        public override void NewSocket(Socket AcceptedSocket, IPEndPoint AcceptedEndPoint)
        {
            ZoneClient NewZone = new ZoneClient(AcceptedSocket, AcceptedEndPoint, ZoneID, ConnectPort);
        }
    }
}
