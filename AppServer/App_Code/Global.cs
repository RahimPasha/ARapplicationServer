using ARApplicationServer.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;

namespace ARApplicationServer.App_Code
{
    public static class Global
    {
        static Global()
        {
            Messages = new List<Messaging>();
            ActiveServerInfo = 1;
            dba = new DAL();
            MyServer = dba.ServerInfo.Where(s => s.ID == ActiveServerInfo).FirstOrDefault();
            TargetHubAddress = MyServer.HubAddress;
            ServerName = MyServer.Name;
            Identifier = MyServer.Name;
            ServerID = MyServer.Name;
            Registered = MyServer.Name;
            TargetsFolder = MyServer.Name;
            OutgoingTargetName = MyServer.Name;
            IncomingDatabase = MyServer.Name;
            OutgoingDatabase = MyServer.Name;
            ChatFolder = MyServer.Name;
            ServerAddress = MyServer.Name;
        }
        public static List<Messaging> Messages;
        public static int ActiveServerInfo;
        public static DAL dba;
        public static Server MyServer;
        public static string TargetHubAddress;
        public static string ServerName = ConfigurationManager.AppSettings["Name"].ToString();
        public static string Identifier = ConfigurationManager.AppSettings["Identifier"].ToString();

        internal static void Refresh()
        {
            MyServer = dba.ServerInfo.Where(s => s.ID == ActiveServerInfo).FirstOrDefault();
            TargetHubAddress = MyServer.HubAddress;
            ServerName = MyServer.Name;
            Identifier = MyServer.Name;
            ServerID = MyServer.Name;
            Registered = MyServer.Name;
            TargetsFolder = MyServer.Name;
            OutgoingTargetName = MyServer.Name;
            IncomingDatabase = MyServer.Name;
            OutgoingDatabase = MyServer.Name;
            ChatFolder = MyServer.Name;
            ServerAddress = MyServer.Name;
        }

        public static string ServerID = ConfigurationManager.AppSettings["ID"].ToString();
        public static string Registered = ConfigurationManager.AppSettings["Registered"].ToString();

        public static string TargetsFolder = ConfigurationManager.AppSettings["TargetsFolder"].ToString();
        public static string OutgoingTargetName = ConfigurationManager.AppSettings["OutgoingTargetName"].ToString();
        public static string IncomingDatabase = ConfigurationManager.AppSettings["IncomingFolder"].ToString();
        public static string OutgoingDatabase = ConfigurationManager.AppSettings["OutgoingFolder"].ToString();
        public static string ChatFolder = ConfigurationManager.AppSettings["ChatFolder"].ToString();
        public static string ServerAddress = ConfigurationManager.AppSettings["ServerAddress"].ToString();

        //private static string getServerAddress()
        //{
        //    string strHostName = System.Net.Dns.GetHostName();
        //    IPHostEntry ipHostInfo = Dns.GetHostEntry(strHostName);
        //    IPAddress ipAddress = ipHostInfo.AddressList[0];
        //    return ipHostInfo.HostName;
        //}
            
    }
}