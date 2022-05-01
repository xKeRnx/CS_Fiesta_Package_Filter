using System;
using System.IO;
using System.Text;

using Filter.Login.Handlers;
using Filter.Manager.Handlers;
using Filter.Zone.Handlers;

namespace Filter.Instances.Networking
{
    internal class Packet : IDisposable
    {
        private MemoryStream PacketStream;
        private BinaryWriter PacketWriter;
        private BinaryReader PacketReader;

        public int Remaining
        {
            get { return Convert.ToInt32(PacketStream.Length - PacketStream.Position); }
        }

        public ushort OPCode;
        public byte Header;
        public ushort Type;

        public Packet(byte[] Buffer)
        {
            PacketStream = new MemoryStream(Buffer);
            PacketReader = new BinaryReader(PacketStream);
        }

        public bool SetHeaderAndType()
        {
            ReadUShort(out OPCode);

            try
            {
                Header = Convert.ToByte(OPCode >> 10);
                Type = (ushort)(OPCode & 1023);

                return true;
            }
            catch { return false; }
        }

        public Packet(byte TransferHeader, ushort TransferType)
        {
            PacketStream = new MemoryStream();
            PacketWriter = new BinaryWriter(PacketStream);

            Header = TransferHeader;
            Type = TransferType;

            OPCode = (ushort)((Header << 10) + (Type & 1023));

            WriteUShort(OPCode);
        }

        public Packet(Login2TypeServer Type) : this(2, (ushort)(Type)) { }

        public Packet(Login3TypeClient Type) : this(3, (ushort)(Type)) { }

        public Packet(Login3TypeServer Type) : this(3, (ushort)(Type)) { }
        public Packet(Manager2TypeServer Type) : this(2, (ushort)(Type)) { }

        public Packet(Manager3TypeServer Type) : this(3, (ushort)(Type)) { }

        public Packet(Manager4TypeClient Type) : this(4, (ushort)(Type)) { }

        public Packet(Manager4TypeServer Type) : this(4, (ushort)(Type)) { }

        public Packet(Manager8TypeClient Type) : this(8, (ushort)(Type)) { }

        public Packet(Manager8TypeServer Type) : this(8, (ushort)(Type)) { }

        public Packet(Manager21TypeClient Type) : this(21, (ushort)(Type)) { }

        public Packet(Manager25TypeServer Type) : this(25, (ushort)(Type)) { }

        public Packet(Manager29TypeClient Type) : this(29, (ushort)(Type)) { }

        public Packet(Manager38TypeClient Type) : this(38, (ushort)(Type)) { }

        public Packet(Zone2TypeServer Type) : this(2, (ushort)(Type)) { }

        public Packet(Zone6TypeClient Type) : this(6, (ushort)(Type)) { }

        public Packet(Zone6TypeServer Type) : this(6, (ushort)(Type)) { }

        public Packet(Zone8TypeClient Type) : this(8, (ushort)(Type)) { }

        public Packet(Zone8TypeServer Type) : this(8, (ushort)(Type)) { }

        public Packet(Zone12TypeClient Type) : this (12, (ushort)(Type)) { }
        
        public Packet(Zone15TypeClient Type) : this(15, (ushort)(Type)) { }

        public Packet(Zone15TypeServer Type) : this(15, (ushort)(Type)) { }

        public Packet(Zone19TypeServer Type) : this(19, (ushort)(Type)) { }

        public Packet(Zone26TypeClient Type) : this(26, (ushort)(Type)) { }

        public bool ReadUShort(out ushort Value)
        {
            if (Remaining < 2)
            {
                Value = 0;

                return false;
            }

            Value = PacketReader.ReadUInt16();

            return true;
        }

        public bool ReadShort(out short Value)
        {
            if (Remaining < 2)
            {
                Value = 0;

                return false;
            }

            Value = PacketReader.ReadInt16();

            return true;
        }

        public bool ReadInt(out int Value)
        {
            if (Remaining < 2)
            {
                Value = 0;

                return false;
            }

            Value = PacketReader.ReadInt32();

            return true;
        }

        public bool ReadUInt(out uint Value)
        {
            if (Remaining < 2)
            {
                Value = 0;

                return false;
            }

            Value = PacketReader.ReadUInt32();

            return true;
        }

        public bool ReadString(out string Value)
        {
            byte Length;

            if (!ReadByte(out Length))
            {
                Value = string.Empty;

                return false;
            }

            if (Remaining < Length)
            {
                Value = string.Empty;

                return false;
            }

            return ReadString(out Value, Length);
        }

        public bool ReadString(out string Value, int ReadLength)
        {
            if (Remaining < ReadLength)
            {
                Value = string.Empty;

                return false;
            }

            byte[] Buffer = new byte[ReadLength];

            if(!ReadBytes(Buffer))
            {
                Value = string.Empty;

                return false;
            }

            int Length = 0;

            if (Buffer[ReadLength - 1] != 0) { Length = ReadLength; }
            else
            {
                while (Buffer[Length] != 0x00 && Length < ReadLength) { Length++; }
            }

            if (Length > 0)
            {
                Value = Encoding.Default.GetString(Buffer, 0, Length);

                return true;
            }

            Value = string.Empty;

            return false;
        }

        public bool ReadByte(out byte Value)
        {
            if (Remaining < 1)
            {
                Value = 0;

                return false;
            }

            Value = PacketReader.ReadByte();

            return true;
        }

        public bool ReadBytes(byte[] Buffer)
        {
            if (Remaining < Buffer.Length) { return false; }

            PacketStream.Read(Buffer, 0, Buffer.Length);

            return true;
        }

        public bool ReadBytes(byte[] Buffer, int Length)
        {
            if (Remaining < Length)
            {
                return false;
            }

            Buffer = PacketReader.ReadBytes(Length);

            return true;
        }
        public void WriteShort(short Value) { PacketWriter.Write(Value); }

        public void WriteInt16(Int16 Value) { PacketWriter.Write(Value); }
        public void WriteUShort(ushort Value) { PacketWriter.Write(Value); }
        public void WriteInt(int Value) { PacketWriter.Write(Value); }
        public void WriteUInt(uint Value) { PacketWriter.Write(Value); }
        public void WriteString(string Value) { PacketWriter.Write(Encoding.Default.GetBytes(Value)); }
        public void WriteString(string Value, bool WriteLength)
        {
            WriteByte(Convert.ToByte(Value.Length));
            WriteBytes(Encoding.Default.GetBytes(Value));
        }
        public void WriteString(string Value, int FillLength)
        {
            byte[] Buffer = Encoding.Default.GetBytes(Value);

            WriteBytes(Buffer);

            for (int Counter = 0; Counter < FillLength - Buffer.Length; Counter++) { WriteByte(0); }
        }
        public void WriteByte(byte Value) { PacketWriter.Write(Value); }
        public void WriteBytes(byte[] Value) { PacketWriter.Write(Value); }
        public void Fill(int pLength, byte pValue)
        {
            for (int i = 0; i < pLength; ++i)
            {
                WriteByte(pValue);
            }
        }
        public void ToArray(FiestaCrypto Crypto, out byte[] PacketBuffer)
        {
            byte[] BuildBuffer;
            byte[] StreamBuffer = PacketStream.ToArray();

            Crypto.Crypt(StreamBuffer, 0, StreamBuffer.Length);

            if (StreamBuffer.Length <= 0xff)
            {
                BuildBuffer = new byte[StreamBuffer.Length + 1];

                Buffer.BlockCopy(StreamBuffer, 0, BuildBuffer, 1, StreamBuffer.Length);

                BuildBuffer[0] = Convert.ToByte(StreamBuffer.Length);
            }
            else
            {
                BuildBuffer = new byte[StreamBuffer.Length + 3];

                Buffer.BlockCopy(StreamBuffer, 0, BuildBuffer, 3, StreamBuffer.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToUInt16(StreamBuffer.Length)), 0, BuildBuffer, 1, 2);
            }

            PacketBuffer = BuildBuffer;
        }

        public void ToArray(out byte[] PacketBuffer)
        {
            byte[] BuildBuffer;
            byte[] StreamBuffer = PacketStream.ToArray();

            if (StreamBuffer.Length <= 0xff)
            {
                BuildBuffer = new byte[StreamBuffer.Length + 1];

                Buffer.BlockCopy(StreamBuffer, 0, BuildBuffer, 1, StreamBuffer.Length);

                BuildBuffer[0] = Convert.ToByte(StreamBuffer.Length);
            }
            else
            {
                BuildBuffer = new byte[StreamBuffer.Length + 3];

                Buffer.BlockCopy(StreamBuffer, 0, BuildBuffer, 3, StreamBuffer.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToUInt16(StreamBuffer.Length)), 0, BuildBuffer, 1, 2);
            }

            PacketBuffer = BuildBuffer;
        }

        ~Packet() { Dispose(); }

        public void Dispose()
        {
            try { PacketReader.Dispose(); }
            catch { }

            try { PacketWriter.Dispose(); }
            catch { }

            try { PacketStream.Dispose(); }
            catch { }
        }
    }
}
