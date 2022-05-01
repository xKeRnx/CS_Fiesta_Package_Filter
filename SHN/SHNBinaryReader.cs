using System;
using System.IO;
using System.Text;

namespace SHN
{
	public class SHNBinaryReader : BinaryReader
	{
		private Encoding SHNEncoding;

		private byte[] Buffer = new byte[256];

		public SHNBinaryReader(Stream S, Encoding SE) : base(S)
		{
			this.SHNEncoding = SE;
		}

		private string PReadString(uint Bytes)
		{
			string empty = string.Empty;
			if (Bytes > 256)
			{
				empty = this.ReadString(Bytes - 256);
			}
			this.Read(this.Buffer, 0, (int)Bytes);
			return string.Concat(empty, this.SHNEncoding.GetString(this.Buffer, 0, (int)Bytes));
		}

		public string ReadString(int Bytes)
		{
			if (Bytes <= 0)
			{
				return string.Empty;
			}
			return this.ReadString((uint)Bytes);
		}

		public string ReadString(uint Bytes)
		{
			return this.PReadString(Bytes).TrimEnd(new char[1]);
		}

		public override string ReadString()
		{
			int num = 0;
			for (byte i = this.ReadByte(); i != 0; i = this.ReadByte())
			{
				int num1 = num;
				num = num1 + 1;
				this.Buffer[num1] = i;
				if (num >= 256)
				{
					break;
				}
			}
			string str = this.SHNEncoding.GetString(this.Buffer, 0, num);
			if (num == 256)
			{
				str = string.Concat(str, this.ReadString());
			}
			return str;
		}
	}
}