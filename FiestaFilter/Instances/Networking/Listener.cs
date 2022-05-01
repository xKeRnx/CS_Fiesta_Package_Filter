using System;
using System.Net;
using System.Net.Sockets;

namespace Filter.Instances.Networking
{
    internal abstract class Listener
    {
        private Socket ListenSocket;
        private IPEndPoint ListenEndPoint;

        public Listener(int Backlog, int Port)
        {
            ListenEndPoint = new IPEndPoint(IPAddress.Any, Port);

            ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try { ListenSocket.Bind(ListenEndPoint); }
            catch { Environment.Exit(0); }
            finally { ListenSocket.Listen(Backlog); }
        }

        public void Accept() { ListenSocket.BeginAccept(new AsyncCallback(Accepted), null); }

        private void Accepted(IAsyncResult AsyncResult)
        {
            Accept();

            try
            {
                Socket AcceptedSocket = ListenSocket.EndAccept(AsyncResult);
                AcceptedSocket.ReceiveTimeout = 2000;
                AcceptedSocket.SendTimeout = 2000;

                IPEndPoint AcceptedEndPoint = (IPEndPoint)AcceptedSocket.RemoteEndPoint;

                NewSocket(AcceptedSocket, AcceptedEndPoint);
            }
            catch { }
        }

        public abstract void NewSocket(Socket AcceptedSocket, IPEndPoint AcceptedEndPoint);
    }
}
