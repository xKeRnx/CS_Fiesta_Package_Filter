using System;
using System.IO;
using System.Text;

namespace SHN
{
	public class SHNBinaryWriter : BinaryWriter
	{
		private Encoding SHNEncoding;

		public SHNBinaryWriter(Stream S, Encoding SE) : base(S)
		{
			this.SHNEncoding = SE;
		}

		public void WriteString(string Text, int Length)
		{
			if (Length == -1)
			{
				this.Write(this.SHNEncoding.GetBytes(Text));
				this.Write((byte)0);
				return;
			}
		    byte[] Bytes = this.SHNEncoding.GetBytes(Text);
		    byte[] numArray = new byte[Length];
			Array.Copy(Bytes, numArray, Math.Min(Length, (int)Bytes.Length));
			this.Write(numArray);
		}
	}
}