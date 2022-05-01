using System;
using System.Linq;
using System.Text;

namespace FilterLib
{
    public static class ByteUtils
    {
        public static byte[] HexToBytes(string pValue)
        {
            // FIRST. Use StringBuilder.
            StringBuilder builder = new StringBuilder();

            // SECOND... USE STRINGBUILDER!... and LINQ.
            foreach (char c in pValue.Where(IsHexDigit).Select(Char.ToUpper))
            {
                builder.Append(c);
            }

            // THIRD. If you have an odd number of characters, something is very wrong.
            string hexString = builder.ToString();
            if (hexString.Length % 2 == 1)
            {
                //throw new InvalidOperationException("There is an odd number of hexadecimal digits in this string.");
                // I will just add a zero to the end, who cares (0 padding)
                hexString += '0';
            }

            byte[] Bytes = new byte[hexString.Length / 2];
            // FOURTH. Use the for-loop like a pro :D
            for (int i = 0, j = 0; i < Bytes.Length; i++, j += 2)
            {
                string ByteString = string.Concat(hexString[j], hexString[j + 1]);
                Bytes[i] = HexToByte(ByteString);
            }
            return Bytes;
        }

        public static string BytesToHex(byte[] Bytes, string header = "")
        {
            StringBuilder builder = new StringBuilder(header);
            foreach (byte c in Bytes)
            {
                builder.AppendFormat("{0:X2} ", c);
            }
            return builder.ToString();
        }

        public static bool IsHexDigit(char c)
        {
            if (('0' <= c && c <= '9') ||
                ('A' <= c && c <= 'F') ||
                ('a' <= c && c <= 'f'))
            {
                return true;
            }
            return false;
        }

        private static byte HexToByte(string hex)
        {
            if (hex == null) throw new ArgumentNullException("hex");
            if (hex.Length == 0 || 2 < hex.Length)
            {
                throw new ArgumentOutOfRangeException("hex", "The hexadecimal string must be 1 or 2 characters in length.");
            }
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
        }
    }
}
