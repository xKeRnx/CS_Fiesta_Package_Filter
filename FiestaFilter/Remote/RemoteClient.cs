using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Data;
using System.Threading;
using System.Net.Sockets;
using System.Data.SqlClient;

using Filter.Instances.Networking;
using Filter.Manager.Networking;

using Filter.Utilities;

namespace Filter.Remote.Networking
{
    internal class RemoteClient : Client, IDisposable
    {
        public String CharacterName;

        public static Guid GUID = Guid.NewGuid();

        public DateTime LastWhisper = DateTime.MinValue;

        public RemoteClient(Socket AcceptedSocket, IPEndPoint AcceptedEndPoint) : base(AcceptedSocket, AcceptedEndPoint)
        {
            Program.RemoteLoggedIn.Add(GUID, this);

            Receive();
        }

        public override void Connected() { }

        public override void ConnectFailed(SocketException Exception) { }

        public override void Received(Byte[] Buffer)
        {
            try
            {
                if (IsConnected != 0) { return; }

                String ReceivedText = Encoding.ASCII.GetString(Buffer);

                String[] ReceivedTextSplit = ReceivedText.Split('#');

                if(ReceivedTextSplit[0] == "Login")
                {
                    String Username = ReceivedTextSplit[1];
                    String Password = ReceivedTextSplit[2];
                    String Character = ReceivedTextSplit[3];

                    if(Username.Contains(" ")) { Send(Encoding.ASCII.GetBytes("Login#0")); }
                    else if(Username.Length > 20) { Send(Encoding.ASCII.GetBytes("Login#1")); }
                    else if(Password.Contains(" ")) { Send(Encoding.ASCII.GetBytes("Login#2")); }
                    else if(Character.Length > 16) { Send(Encoding.ASCII.GetBytes("Login#3")); }
                    else if(Character.Contains(" ")) { Send(Encoding.ASCII.GetBytes("Login#4")); }
                    else if(Username == "SZEWXC6476FFL" && Password == "dadDFAAR09CVBCKOPMS91")
                    {
                        CharacterName = Character;
                        Send(Encoding.ASCII.GetBytes("Login#7"));
                    }
                }

                else if(CharacterName == String.Empty) { Disconnect(); }
                else if(ReceivedTextSplit[0] == "Ping") { }
                else if(ReceivedTextSplit[0] == "Online")
                {
                    String OnlineString = "Online#";

                    foreach(var MClient in Program.ManagerLoggedIn.Values.Where(Character => Character.CharacterName != String.Empty)) { OnlineString += String.Concat(MClient.CharacterName, "|"); }

                    Send(Encoding.ASCII.GetBytes(OnlineString.TrimEnd('|')));
                }
                else if(ReceivedTextSplit[0] == "Whisper")
                {
                    String Receiver = ReceivedTextSplit[1];
                    String Message = ReceivedTextSplit[2];

                    ManagerClient MClient;

                    if((MClient = Program.ManagerLoggedIn.Values.Where(Character => Character.CharacterName.ToLower() == Receiver.ToLower()).FirstOrDefault()) != null)
                    {
                        if(LastWhisper > DateTime.Now && LastWhisper != DateTime.MinValue)
                        {
                            TimeSpan WaitSpan = LastWhisper.Subtract(DateTime.Now);

                            Send(Encoding.ASCII.GetBytes(String.Format("Whisper#Wait#{0}", WaitSpan.Seconds)));
                        }
                        else
                        {
                            Messager.Whisper(CharacterName, MClient, Message);

                            Send(Encoding.ASCII.GetBytes(String.Format("Whisper#Online#{0}#{1}", Receiver, Message)));

                            LastWhisper = DateTime.Now.AddSeconds(1);
                        }
                    }
                    else { Send(Encoding.ASCII.GetBytes("Whisper#Offline")); }
                }
                else if(ReceivedTextSplit[0] == "World")
                {
                    String Message = ReceivedTextSplit[1];

                    Messager.World(true, Message);

                    Send(Encoding.ASCII.GetBytes("World#OK"));
                }
                else if(ReceivedTextSplit[0] == "Roar")
                {
                    String Message = ReceivedTextSplit[1];

                    Messager.Roar(CharacterName, Message);

                    Send(Encoding.ASCII.GetBytes("Roar#OK"));
                }
                else { Disconnect(); }
            }
            catch
            {
                Send(Encoding.ASCII.GetBytes("UNK#CMD"));
            }
        }

        public override void Disconnected()
        {
            if (Program.RemoteLoggedIn.ContainsKey(GUID)) { Program.RemoteLoggedIn.Remove(GUID); }
        }
    }
}
