using System;
using System.Net;
using System.Net.Sockets;
using Filter.Instances.Networking;
using Filter.Utilities;

namespace Filter.Remote.Networking
{
    internal class RemoteListener : Listener
    {
        private Int32 BindPort;

        public RemoteListener(Int32 BPort) : base(50000, BPort)
        {
            BindPort = BPort;
            Writer.Write(ConsoleType.Socket, "Added RemoteTool listen on {0}", BindPort);
            Accept();
        }

        public override void NewSocket(Socket AcceptedSocket, IPEndPoint AcceptedEndPoint) { RemoteClient NewRemote = new RemoteClient(AcceptedSocket, AcceptedEndPoint); }
    }
}
