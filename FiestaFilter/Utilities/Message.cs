using System;
using System.Data.SqlClient;
using System.Data;

using Filter.Manager.Handlers;
using Filter.Zone.Handlers;

using Filter.Instances.Networking;
using Filter.Manager.Networking;
using Filter.Zone.Networking;


namespace Filter.Utilities
{
    class Messager
    {
        public static void Whisper(ManagerClient ClientSocket, string Message, params Object[] Args)
        {
            string FormattedMessage = string.Format(Message, Args);

            Packet ServerPacket = new Packet(Manager8TypeServer.WhisperMessage);
            ServerPacket.WriteString(Program.Botname, 16);
            ServerPacket.WriteByte(7);
            ServerPacket.WriteString(FormattedMessage, true);

            ClientSocket.SendPacket(ServerPacket);
        }

        public static void Whisper(string Sender, ManagerClient ClientSocket, string Message, params Object[] Args)
        {
            string FormattedMessage = string.Format(Message, Args);

            Packet ServerPacket = new Packet(Manager8TypeServer.WhisperMessage);
            ServerPacket.WriteString(Sender, 16);
            ServerPacket.WriteByte(7);
            ServerPacket.WriteString(FormattedMessage, true);

            ClientSocket.SendPacket(ServerPacket);
        }

        public static void Whisper(bool SendToAll, string Message, params Object[] Args)
        {
            string FormattedMessage = string.Format(Message, Args);

            foreach (ManagerClient ManagerPlayer in Program.ManagerLoggedIn.Values)
            {
                Packet ServerPacket = new Packet(Manager8TypeServer.WhisperMessage);
                ServerPacket.WriteString(Program.Botname, 16);
                ServerPacket.WriteByte(7);
                ServerPacket.WriteString(FormattedMessage, true);

                ManagerPlayer.SendPacket(ServerPacket);
            }
        }
        
        public static void Roar(ManagerClient ClientSocket, string Message, params Object[] Args)
        {
            string FormattedMessage = string.Format(Message, Args);

            Packet ClientPacket = new Packet(Manager25TypeServer.Roar);
            ClientPacket.WriteByte(11);
            ClientPacket.WriteByte(Convert.ToByte(string.Concat(Program.Botname, FormattedMessage).Length + 2));
            ClientPacket.WriteString(string.Concat(Program.Botname, ":", FormattedMessage));//Program.Config.BotName, ">", FormattedMessage));
            ClientPacket.WriteByte(0);

            ClientSocket.SendPacket(ClientPacket);
        }
        public static void Roar(string Sender, string Message, params Object[] Args)
        {
            string FormattedMessage = string.Format(Message, Args);

            foreach (ManagerClient ManagerPlayer in Program.ManagerLoggedIn.Values)
            {
                Packet ServerPacket = new Packet(Manager25TypeServer.Roar);

                Char InvalidSymbol;

                FormattedMessage = FormattedMessage.Replace("?", "?");


                Packet ClientPacket = new Packet(Manager25TypeServer.Roar);
                ClientPacket.WriteByte(11);
                ClientPacket.WriteByte(Convert.ToByte(string.Concat(Sender, FormattedMessage).Length + 2));
                ClientPacket.WriteString(string.Concat(Sender, ":", FormattedMessage));
                ClientPacket.WriteByte(0);

                ManagerPlayer.SendPacket(ClientPacket);
            }

        }

        public static void World(ManagerClient ClientSocket, string Message, params Object[] Args)
        {
            string FormattedMessage = string.Format(Message, Args);

            Packet ServerPacket = new Packet(Manager8TypeServer.WorldMessage);
            ServerPacket.WriteByte(241);
            ServerPacket.WriteByte(Convert.ToByte(FormattedMessage.Length + 1));
            ServerPacket.WriteString(FormattedMessage);
            ServerPacket.WriteByte(32);

            ClientSocket.SendPacket(ServerPacket);
        }

        public static void World(bool SendToAll, string Message, params Object[] Args)
        {
            string FormattedMessage = string.Format(Message, Args);

            foreach (ManagerClient ManagerPlayer in Program.ManagerLoggedIn.Values)
            {
                Packet ServerPacket = new Packet(Manager8TypeServer.WorldMessage);
                ServerPacket.WriteByte(241);
                ServerPacket.WriteByte(Convert.ToByte(FormattedMessage.Length + 1));
                ServerPacket.WriteString(FormattedMessage);
                ServerPacket.WriteByte(32);

                ManagerPlayer.SendPacket(ServerPacket);
            }
        }

        public static void Shout(ZoneClient ClientSocket, string Message, params Object[] Args)
        {
            string FormattedMessage = string.Format(Message, Args);

            Packet ServerPacket = new Packet(Zone8TypeServer.ShoutChat);
            ServerPacket.WriteString(Program.Botname, 16);
            ServerPacket.WriteByte(7);
            ServerPacket.WriteString(FormattedMessage, true);

            ClientSocket.SendPacket(ServerPacket);
        }

        public static void Shout(string Sender, ZoneClient ClientSocket, string Message, params Object[] Args)
        {
            string FormattedMessage = string.Format(Message, Args);

            Packet ServerPacket = new Packet(Zone8TypeServer.ShoutChat);
            ServerPacket.WriteString(Sender, 16);
            ServerPacket.WriteByte(7);
            ServerPacket.WriteString(FormattedMessage, true);

            ClientSocket.SendPacket(ServerPacket);
        }

        public static void Shout(bool SendToAll, ZoneClient ClientSocket, string Message, params Object[] Args)
        {
            string FormattedMessage = string.Format(Message, Args);

            Packet ServerPacket = new Packet(Zone8TypeServer.ShoutChat);
            ServerPacket.WriteString(Program.Botname, 16);
            ServerPacket.WriteByte(7);
            ServerPacket.WriteString(FormattedMessage, true);

            ClientSocket.SendPacket(ServerPacket);
        }

    }
}
