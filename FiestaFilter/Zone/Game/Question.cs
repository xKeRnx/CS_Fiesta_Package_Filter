using System.Collections.Generic;

using Filter.Zone.Handlers;
using Filter.Zone.Networking;
using Filter.Instances.Networking;
using Filter.Utilities;

namespace Filter.Zone.Game
{
    internal delegate void QuestionCallback(ZoneClient ClientSocket, byte answer);

    internal class Question
    {
        public string Text { get; private set; }
        public QuestionCallback Function { get; private set; }
        public List<string> Answers { get; private set; }
        public object Object { get; set; }

        public Question(string pText, QuestionCallback pFunction, object obj = null)
        {
            Text = pText;
            Function = pFunction;
            Answers = new List<string>();
            Object = obj;
        }

        public void Add(params string[] text)
        {
            Answers.AddRange(text);
        }

        public void Send(ZoneClient ClientSocket, ushort distance = 1000)
        {
            SendQuestion(ClientSocket, this, distance);
        }

        public static void SendQuestion(ZoneClient ClientSocket, Question question, ushort range)
        {
            using (var packet = new Packet(Zone15TypeServer.Question))
            {
                packet.WriteString(question.Text, 129);
                packet.WriteUShort(0);     // object ID 0 for unk?
                packet.WriteUInt((uint)ClientSocket.PosX);
                packet.WriteUInt((uint)ClientSocket.PosY);
                packet.WriteUShort(range);        // autoclose dst
                packet.WriteByte((byte)question.Answers.Count);
                for (byte i = 0; i < question.Answers.Count; ++i)
                {
                    packet.WriteByte(i);
                    packet.WriteString(question.Answers[i], 32);
                }
                ClientSocket.SendPacket(packet);

                ClientSocket.Question = question;
                ClientSocket.HasQuestion = true;
            }
        }
    }
}
