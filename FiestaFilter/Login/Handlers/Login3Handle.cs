using System;
using System.Net;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

using Filter.Login.Networking;
using Filter.Instances.Networking;

using Filter.Utilities;
using FilterLib;

namespace Filter.Login.Handlers
{
    internal enum LoginResponse : byte
    {
        DatabaseError = 67, //DB Error.
        AuthFailed = 68, //Authentication Failed.
        CheckIdPw = 69, //Please check ID or Password.
        IdBlocked = 71, //The ID has been blocked.
        WorldMaintenance = 72, //The World Servers are down for maintenance.
        TimeOut = 73, //Authentication timed out. Please try to log in again.
        LoginFailed = 74, //Login failed.
        Agreement = 75, //Please accept the agreement to continue.
    }

    internal class Login3Handle
    {
        //Login3 - Client Version
        [LoginPacketHandler(3, 101)]
        public static void HandleVersion(LoginClient ClientSocket, Packet ClientPacket)
        {
            string Year;
            string Version;

            if (!ClientPacket.ReadString(out Year, 4))
            {

                SendResponse(ClientSocket, LoginResponse.LoginFailed);

                ClientSocket.Disconnect();
            }else if (!ClientPacket.ReadString(out Version, 10))
            {

                SendResponse(ClientSocket, LoginResponse.LoginFailed);

                ClientSocket.Disconnect();
            }
            else if (Year != Program.LoginYear || Version != Program.LoginVersion)
            {
                SendResponse(ClientSocket, LoginResponse.LoginFailed);

                ClientSocket.Disconnect();
            }
            else { ClientSocket.Server.SendPacket(ClientPacket); }

            ClientPacket.Dispose();
        }

        //Login3 - ClientHash
        [LoginPacketHandler(3, 4)]
        public static void HanldeHash(LoginClient ClientSocket, Packet ClientPacket)
        {
            byte unwanted;
            string hash;

            ClientPacket.ReadByte(out unwanted);
            if (!ClientPacket.ReadString(out hash, 28))
            {
                SendResponse(ClientSocket, LoginResponse.IdBlocked);

                ClientSocket.Disconnect();
            }
            else if (hash != Program.LoginHash)
            {
                Writer.Write(ConsoleType.Login, "Blocked invalid client from {0}", ClientSocket.RemoteAddress);

                SendResponse(ClientSocket, LoginResponse.TimeOut);

                ClientSocket.Disconnect();
            } else { ClientSocket.Server.SendPacket(ClientPacket); }

            ClientPacket.Dispose();
        }

        //Login3 - World Select
        [LoginPacketHandler(3, 11)]
        public static void HandleWorldSelect(LoginClient ClientSocket, Packet ClientPacket)
        {
            if (!ClientPacket.ReadByte(out byte WorldID)) { ClientSocket.Disconnect(); }
            else { ClientSocket.Server.SendPacket(ClientPacket); }

            ClientPacket.Dispose();
        }


        //Login3 - Login To WM Transfer
        [LoginPacketHandler(3, 12)]
        public static void HandleTransfer(LoginServer ServerSocket, Packet ServerPacket)
        {
            if (ServerSocket.IsConnected == 0)
            {
                byte WorldStatus; ServerPacket.ReadByte(out WorldStatus);
                string IPAddress; ServerPacket.ReadString(out IPAddress, 16);
                short Port; ServerPacket.ReadShort(out Port);
                string TransferKey; ServerPacket.ReadString(out TransferKey, 64);

                ServerPacket.Dispose();

                if (Program.ServerList.Contains(Program.ServerIP)) { ServerSocket.Client.Disconnect(); return; }

                ServerPacket = new Packet(Login3TypeServer.Transfer);
                ServerPacket.WriteByte(WorldStatus);
                ServerPacket.WriteString(Program.ServerIP, 16);
                ServerPacket.WriteShort(Port);
                ServerPacket.WriteString(TransferKey);

                Program.LoginToManagerTransfer.Add(TransferKey, ServerSocket.Client.AccountID);

                ServerSocket.Client.SendPacket(ServerPacket);
            }

            ServerPacket.Dispose();
        }

      

        private static void SendResponse(LoginClient ClientSocket, LoginResponse Response)
        {
            Packet ServerPacket = new Packet(Login3TypeServer.LoginError);
            ServerPacket.WriteShort(Convert.ToInt16(Response));

            ClientSocket.SendPacket(ServerPacket);
        }
    }
}
