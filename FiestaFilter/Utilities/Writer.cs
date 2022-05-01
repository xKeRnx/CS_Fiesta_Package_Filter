using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Filter.Utilities
{
    internal enum ConsoleType
    {
        Login,
        Manager,
        Zone,
        Database,
        Socket,
        SHN,
        Error,
        Filter
    }

    class Writer
    {
        public static void Write(ConsoleType Type, string Message, params object[] Args)
        {
            string arg = string.Format(Message, Args);
            ConsoleColor foregroundColor = Writer.TypeColor(Type);
            Console.ForegroundColor = foregroundColor;
            string write = string.Format("[{0}] || ({1}): {2}", DateTime.Now.ToString("HH:mm:ss"), Type, arg);
            Console.WriteLine(write);
            Console.ForegroundColor = ConsoleColor.White;
            File.AppendAllText("output.txt", $"{write}\n");
        }

        private static ConsoleColor TypeColor(ConsoleType Type)
        {
            switch (Type)
            {
                case ConsoleType.Login:
                    return ConsoleColor.Blue;
                case ConsoleType.Manager:
                    return ConsoleColor.Cyan;
                case ConsoleType.Zone:
                    return ConsoleColor.DarkCyan;
                case ConsoleType.SHN:
                    return ConsoleColor.Magenta;
                case ConsoleType.Socket:
                    return ConsoleColor.DarkGreen;
                case ConsoleType.Database:
                    return ConsoleColor.Green;
                case ConsoleType.Error:
                    return ConsoleColor.Red;
                case ConsoleType.Filter:
                    return ConsoleColor.Yellow;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}