using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace Filter.Instances.Networking
{
    internal abstract class Client : IDisposable
    {
        public Socket ClientSocket;
        private IPEndPoint ClientEndPoint;

        private byte[] ClientBuffer = new byte[4096];

        public string RemoteAddress;
        public int RemotePort;

        public int IsConnected;

        public Client(IPAddress ConnectIP, int ConnectPort)
        {
            RemoteAddress = Convert.ToString(ConnectIP);
            RemotePort = ConnectPort;

            ClientEndPoint = new IPEndPoint(ConnectIP, ConnectPort);

            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.BeginConnect(ClientEndPoint, new AsyncCallback(ConnectionOpened), null);
        }

        private void ConnectionOpened(IAsyncResult AsyncResult)
        {
            try
            {
                ClientSocket.EndConnect(AsyncResult);

                Connected();
            }
            catch (SocketException Exception) { ConnectFailed(Exception); }
        }

        public abstract void Connected();

        public abstract void ConnectFailed(SocketException Exception);

        public Client(Socket AcceptedSocket, IPEndPoint AcceptedEndPoint)
        {
            ClientSocket = AcceptedSocket;
            ClientEndPoint = AcceptedEndPoint;

            RemoteAddress = Convert.ToString(ClientEndPoint.Address);
            RemotePort = ClientEndPoint.Port;
        }

        public void Receive()
        {
            if (IsConnected != 0) { return; }

            ClientBuffer = new byte[1];

            try { ClientSocket.BeginReceive(ClientBuffer, 0, ClientBuffer.Length, SocketFlags.None, new AsyncCallback(ReceivedSize), null); }
            catch { Disconnect(); }
        }

        private void ReceivedSize(IAsyncResult AsyncResult)
        {
            if (IsConnected != 0) { return; }

            int BytesReceived = 0;

            try { BytesReceived = ClientSocket.EndReceive(AsyncResult); }
            catch { Disconnect(); }

            if (BytesReceived > 0)
            {
                if (ClientBuffer[0] == 0)
                {
                    ClientBuffer = new byte[2];

                    try { ClientSocket.BeginReceive(ClientBuffer, 0, 2, SocketFlags.None, new AsyncCallback(ReceivedBigSize), null); }
                    catch { Disconnect(); }
                }
                else
                {
                    ClientBuffer = new byte[ClientBuffer[0]];

                    try { ClientSocket.BeginReceive(ClientBuffer, 0, ClientBuffer.Length, SocketFlags.None, new AsyncCallback(ReceivedPacket), null); }
                    catch { Disconnect(); }
                }
            }
            else { Disconnect(); }
        }

        private void ReceivedBigSize(IAsyncResult AsyncResult)
        {
            if (IsConnected != 0) { return; }

            int BytesReceived = 0;

            try { BytesReceived = ClientSocket.EndReceive(AsyncResult); }
            catch { Disconnect(); }

            if (BytesReceived > 0)
            {
                short ReceivedSize = BitConverter.ToInt16(ClientBuffer, 0);

                ClientBuffer = new byte[ReceivedSize];
                Thread.Sleep(200);

                try { ClientSocket.BeginReceive(ClientBuffer, 0, ReceivedSize, SocketFlags.None, new AsyncCallback(ReceivedPacket), null); }
                catch { Disconnect(); }
            }
            else { Disconnect(); }
        }

        private void ReceivedPacket(IAsyncResult AsyncResult)
        {
            if (IsConnected != 0) { return; }

            int BytesReceived = 0;

            try { BytesReceived = ClientSocket.EndReceive(AsyncResult); }
            catch { Disconnect(); }

            if (BytesReceived > 0)
            {
                Received(ClientBuffer);

                Receive();
            }
            else { Disconnect(); }
        }

        public void Send(byte[] Buffer)
        {
            if (IsConnected != 0) { return; }

            try { ClientSocket.BeginSend(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(Sent), null); }
            catch { Disconnect(); }
        }

        private void Sent(IAsyncResult AsyncResult)
        {
            if (IsConnected != 0) { return; }

            try { ClientSocket.EndSend(AsyncResult); }
            catch { Disconnect(); }
        }

        public void Disconnect()
        {
            if (IsConnected == 0 && Interlocked.CompareExchange(ref IsConnected, 1, 0) == 0)
            {
                Disconnected();

                try { ClientSocket.Dispose(); }
                catch { }
            }
        }

        ~Client() { Dispose(); }

        public void Dispose() { Disconnect(); }

        public abstract void Received(byte[] Buffer);

        public abstract void Disconnected();
    }
}
