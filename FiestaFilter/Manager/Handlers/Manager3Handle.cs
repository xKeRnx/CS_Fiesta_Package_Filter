using System;
using System.Linq;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using Filter.Instances.Networking;
using Filter.Manager.Networking;

namespace Filter.Manager.Handlers
{
    internal class Manager3Handle
    {
        //ManagerCrash Block
        [ManagerPacketHandler(3, 86)]
        public static void HandleCrashPack1(ManagerClient ClientSocket, Packet ClientPacket)
        {
            String Msg;
            ClientPacket.ReadString(out Msg);
            DateTime dt = DateTime.Now;
            String IP = ClientSocket.RemoteAddress;
            File.AppendAllText(string.Format("{0}\\WM Crash {1}-{2}.txt", AppDomain.CurrentDomain.BaseDirectory, dt.Day, dt.Month), string.Format("[{0}] {1}\n", DateTime.Now.ToString("HH:mm:ss"), IP));
            if (Msg.Length == 0)
            {
                ClientSocket.Disconnect();
            }
            
            ClientPacket.Dispose();
        }

        //Manager3 - Login To Manager Transfer
        [ManagerPacketHandler(3, 15)]
        public static void HandleTransfer(ManagerClient ClientSocket, Packet ClientPacket)
        {
            byte[] Unwanted = new byte[256];

            if (!ClientPacket.ReadBytes(Unwanted, 256)) { ClientSocket.Disconnect(); }
            else
            {
                string TransferKey;

                if (!ClientPacket.ReadString(out TransferKey, 64)) { ClientSocket.Disconnect(); }
                else if (!Program.LoginToManagerTransfer.ContainsKey(TransferKey)) { ClientSocket.Disconnect(); }
                else
                {
                    ClientSocket.AccountID = Program.LoginToManagerTransfer[TransferKey];

                    ClientSocket.Server.SendPacket(ClientPacket);
                }
            }

            ClientPacket.Dispose();
        }
    }
}
