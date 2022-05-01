using System;
using System.Linq;
using System.Data;
using System.Timers;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Threading;

using Filter.Remote.Networking;
using Filter.Login.Networking;
using Filter.Manager.Networking;
using Filter.Zone.Networking;

using Filter.Login.Handlers;
using Filter.Manager.Handlers;
using Filter.Zone.Handlers;


using Filter.Utilities;
using Timer = System.Timers.Timer;

namespace Filter
{
    internal class Zonep
    {
        public byte ZoneID { get; set; }
        public int BindPort { get; set; }
        public int ConnectPort { get; set; }
        public bool NormalProtect { get; set; }
        public bool NormalLog { get; set; }
        public bool ShoutProtect { get; set; }
        public bool ShoutLog { get; set; }
    }

    internal class Program
    {


        public static Dictionary<string, int> LoginToManagerTransfer;

        public static Dictionary<Guid, ManagerClient> ManagerLoggedIn;
        public static Dictionary<Guid, ZoneClient> ZoneLoggedIn;
        public static Dictionary<Guid, RemoteClient> RemoteLoggedIn;

        public static LoginHandlerLoader LoginHandlers;
        public static ManagerHandlerLoader ManagerHandlers;
        public static ZoneHandlerLoader ZoneHandlers;

        public static Timer TitleTimer;

        public static byte ZoneCount;
        public static int ZoneStdPort;
        public static int ManagerBind;
        public static int ManagerConnect;
        public static int LoginBind;
        public static int LoginConnect;
        public static string ServerIP;
        public static string LoginYear;
        public static string LoginVersion;
        public static string LoginHash;

        public static string Servername;
        public static string Botname;
        public static string Duplimsg;

        public static List<string> ServerList;
        public static List<Zonep> Zones = new List<Zonep>();

        static void Main()
        {

            var MyIni = new IniFile("Settings.ini");

            if (!MyIni.KeyExists("ServerIP", "KeRnFilter2020"))
            {
                MyIni.Write("ServerIP", "82.165.67.50", "KeRnFilter2020");
            }
            if (!MyIni.KeyExists("Servername", "KeRnFilter2020"))
            {
                MyIni.Write("Servername", "KeRnOnline", "KeRnFilter2020");
            }
            if (!MyIni.KeyExists("Botname", "KeRnFilter2020"))
            {
                MyIni.Write("Botname", "KeRnBot", "KeRnFilter2020");
            }
            if (!MyIni.KeyExists("Duplimsg", "KeRnFilter2020"))
            {
                MyIni.Write("Duplimsg", "don't try to duplicate Items. Idiot!", "KeRnFilter2020");
            }
            if (!MyIni.KeyExists("LoginYear", "KeRnFilter2020"))
            {
                MyIni.Write("LoginYear", "2015", "KeRnFilter2020");
            }
            if (!MyIni.KeyExists("LoginVersion", "KeRnFilter2020"))
            {
                MyIni.Write("LoginVersion", "1116141627", "KeRnFilter2020");
            }
            if (!MyIni.KeyExists("LoginHash", "KeRnFilter2020"))
            {
                MyIni.Write("LoginHash", "33B543B0CA6E7C41E5D1D0651307", "KeRnFilter2020");
            }
            if (!MyIni.KeyExists("LoginBind", "KeRnFilter2020"))
            {
                MyIni.Write("LoginBind", "9010", "KeRnFilter2020");
            }
            if (!MyIni.KeyExists("LoginConnect", "KeRnFilter2020"))
            {
                MyIni.Write("LoginConnect", "9010", "KeRnFilter2020");
            }
            if (!MyIni.KeyExists("WMLoginBind", "KeRnFilter2020"))
            {
                MyIni.Write("WMLoginBind", "9110", "KeRnFilter2020");
            }
            if (!MyIni.KeyExists("WMLoginConnect", "KeRnFilter2020"))
            {
                MyIni.Write("WMLoginConnect", "9110", "KeRnFilter2020");
            }
            if (!MyIni.KeyExists("ZoneCount", "KeRnFilter2020"))
            {
                MyIni.Write("ZoneCount", "6", "KeRnFilter2020");
            }
            ZoneCount = Convert.ToByte(MyIni.Read("ZoneCount", "KeRnFilter2020"));
            ZoneStdPort = 9210;
            for (int Counter = 1; Counter < ZoneCount + 1; Counter++)
            {
                if (!MyIni.KeyExists(string.Format("Zone{0}_ID", Counter), "KeRnFilter2020"))
                {
                    MyIni.Write(string.Format("Zone{0}_ID", Counter), (Counter-1).ToString(), "KeRnFilter2020");
                }
                if (!MyIni.KeyExists(string.Format("Zone{0}_BindPort", Counter), "KeRnFilter2020"))
                {
                    MyIni.Write(string.Format("Zone{0}_BindPort", Counter), ZoneStdPort.ToString(), "KeRnFilter2020");
                }
                if (!MyIni.KeyExists(string.Format("Zone{0}_ConnectPort", Counter), "KeRnFilter2020"))
                {
                    MyIni.Write(string.Format("Zone{0}_ConnectPort", Counter), ZoneStdPort.ToString(), "KeRnFilter2020");
                }
                ZoneStdPort = ZoneStdPort + 2;
            }

            LoginYear = MyIni.Read("LoginYear", "KeRnFilter2020");
            LoginVersion = MyIni.Read("LoginVersion", "KeRnFilter2020");
            LoginHash = MyIni.Read("LoginHash", "KeRnFilter2020");

            LoginBind = Convert.ToInt32(MyIni.Read("LoginBind", "KeRnFilter2020"));
            LoginConnect = Convert.ToInt32(MyIni.Read("LoginConnect", "KeRnFilter2020"));

            ManagerBind = Convert.ToInt32(MyIni.Read("WMLoginBind", "KeRnFilter2020"));
            ManagerConnect = Convert.ToInt32(MyIni.Read("WMLoginConnect", "KeRnFilter2020"));
            ServerIP = MyIni.Read("ServerIP", "KeRnFilter2020");

            Servername = MyIni.Read("Servername", "KeRnFilter2020");
            Botname = MyIni.Read("Botname", "KeRnFilter2020");
            Duplimsg = MyIni.Read("Duplimsg", "KeRnFilter2020");

            for (int Counter = 1; Counter < ZoneCount + 1; Counter++)
            {
                Zonep NewZone = new Zonep()
                {
                    ZoneID = Convert.ToByte(MyIni.Read(string.Format("Zone{0}_ID", Counter), "KeRnFilter2020")),
                    BindPort = Convert.ToInt32(MyIni.Read(string.Format("Zone{0}_BindPort", Counter), "KeRnFilter2020")),
                    ConnectPort = Convert.ToInt32(MyIni.Read(string.Format("Zone{0}_ConnectPort", Counter), "KeRnFilter2020")),
                };

                Zones.Add(NewZone);
            }



            LoginToManagerTransfer = new Dictionary<string, int>();
            ManagerLoggedIn = new Dictionary<Guid, ManagerClient>();
            ZoneLoggedIn = new Dictionary<Guid, ZoneClient>();
            ServerList = new List<string>();
            


            Console.Title = string.Format("Fiesta Filter (c) KeRn 2020 :: Manager: {0} :: RAM usage: {1} MB", ManagerLoggedIn.Count, (Process.GetCurrentProcess().PrivateMemorySize64 / 1024f) / 1024f);

            LoginHandlers = new LoginHandlerLoader();
            ManagerHandlers = new ManagerHandlerLoader();
            ZoneHandlers = new ZoneHandlerLoader();

            TitleTimer = new Timer();
            TitleTimer.AutoReset = true;
            TitleTimer.Elapsed += TitleTimer_Elapsed;
            TitleTimer.Interval = 1000;
            TitleTimer.Start();

            LoginListener LoginListen = new LoginListener(LoginBind, LoginConnect);
            ManagerListener ManagerListen = new ManagerListener(ManagerBind, ManagerConnect);
            foreach (Zonep Z in Zones) { ZoneListener ZoneListen = new ZoneListener(Z.ZoneID, Z.BindPort, Z.ConnectPort); }

            Writer.Write(ConsoleType.Filter, "Filter succesfully started - (c) KeRn");
            Console.ReadLine();
        }

        private static void TitleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.Title = string.Format("Fiesta Filter (c) KeRn 2020 :: Manager: {0} :: RAM usage: {1} MB", ManagerLoggedIn.Count, (Process.GetCurrentProcess().PrivateMemorySize64 / 1024f) / 1024f);
        }

    }
}
