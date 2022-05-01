using System;
using System.Net;
using System.Net.Sockets;

using Filter.Instances.Networking;

using Filter.Utilities;

namespace Filter.Login.Networking
{
    internal class LoginListener : Listener
    {
        private int BindPort;
        private int ConnectPort;

        public LoginListener(int BPort, int CPort) : base(50000, BPort)
        {
            BindPort = BPort;
            ConnectPort = CPort;
            Writer.Write(ConsoleType.Socket, "Added Login listen on {0}", BindPort);
            Accept();
        }

        public override void NewSocket(Socket AcceptedSocket, IPEndPoint AcceptedEndPoint)
        {
            LoginClient NewLogin = new LoginClient(AcceptedSocket, AcceptedEndPoint, ConnectPort);
        }
    }
}
