using System;
using System.Net;
using System.Net.Sockets;

using Filter.Instances.Networking;

using Filter.Utilities;

namespace Filter.Manager.Networking
{
    internal class ManagerListener : Listener
    {
        private int BindPort;
        private int ConnectPort;

        public ManagerListener(int BPort, int CPort)
            : base(50000, BPort)
        {
            BindPort = BPort;
            ConnectPort = CPort;
            Writer.Write(ConsoleType.Socket, "Added Manager listen on {0}", BindPort);
            Accept();
        }

        public override void NewSocket(Socket AcceptedSocket, IPEndPoint AcceptedEndPoint)
        {
            ManagerClient NewManager = new ManagerClient(AcceptedSocket, AcceptedEndPoint, ConnectPort);
        }
    }
}
