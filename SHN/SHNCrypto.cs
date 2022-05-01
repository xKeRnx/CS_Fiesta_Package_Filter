using System;

namespace SHN
{
	public class SHNCrypto
	{
		public SHNCrypto()
		{
		}

		public static void CryptoDefault(byte[] Data)
		{
			byte length = (byte)((int)Data.Length);
			for (int i = (int)Data.Length - 1; i >= 0; i--)
			{
				Data[i] = (byte)(Data[i] ^ length);
				byte num = (byte)((byte)((byte)i & 15) + 85);
				num = (byte)(num ^ (byte)((byte)i * 11));
				length = (byte)((byte)(num ^ length) ^ 170);
			}
		}

		private static byte Method1(byte DL, byte Number)
		{
			return (byte)(DL & Number);
		}

		private static byte Method2(byte DL, byte Number)
		{
			return (byte)(DL + Number);
		}

		private static byte Method3(byte DL, int Counter, byte Number)
		{
			return (byte)(DL ^ (byte)((byte)Counter * Number));
		}

		private static byte Method4(byte DL, byte Number)
		{
			return (byte)(DL ^ Number);
		}
	}
}