using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShineTable
{
    public class ShineColumn : DataColumn
    {
        public string TypeName { get; private set; }

        public ShineColumn(string name, string type, ref int unkColumns)
        {
            if (name.Length < 2)
            {
                name = "Unk_" + unkColumns++;
            }

            this.Caption = name;
            this.ColumnName = name;

            this.TypeName = type;
            this.DataType = GetColumnType();

            if (type.StartsWith("string["))
            {
                int from = type.IndexOf('['), till = type.IndexOf(']');
                string lenStr = type.Substring(from + 1, (till - from) - 1);
                int len = int.Parse(lenStr);
                this.MaxLength = len;
            }

        }


        public Type GetColumnType()
        {
            var typeName = TypeName.ToLower();
            if (typeName.StartsWith("string["))
            {
                return typeof(string); // Char array actually ;p
            }
            switch (typeName)
            {
                case "Byte": return typeof(byte);

                case "word": return typeof(short);

                case "<integer>":
                case "dwrd":
                case "dword": return typeof(int);

                case "qword": return typeof(long);

                case "index": return typeof(string);

                case "<string>":
                case "string": return typeof(string);
                default:
                    Console.WriteLine("Unknown column type found: {0} : {1}", typeName, ColumnName);
                    break;
            }
            return typeof(string); // Just to be sure ?!?! D:
        }
    }
}
