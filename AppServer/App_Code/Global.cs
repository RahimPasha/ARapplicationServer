using ARApplicationServer.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ARApplicationServer.App_Code
{
    public static class Global
    {
        static Global()
        {
            
            ActiveServerInfo = 1;
            Refresh();
            /*using (DAL db = new DAL())
            {
                MyServer = db.Servers.Where(s => s.ID == ActiveServerInfo).FirstOrDefault();
                TargetHubAddress = MyServer.HubAddress;
                ServerName = MyServer.Name;
                Identifier = MyServer.Identifier;
                HubID = MyServer.HubID.ToString();
                Registered = MyServer.Registered;
                TargetsFolder = MyServer.TargetsFolder;
                OutgoingTargetName = MyServer.OutgoingFolder;
                IncomingDatabase = MyServer.IncomingFolder;
                OutgoingDatabase = MyServer.OutgoingTargetName;
                ChatFolder = MyServer.ChatFolder;
                ServerAddress = MyServer.Address;
            }*/
        }
        public static string HubID;
        public static string Registered;
        public static string TargetsFolder;
        public static string OutgoingTargetName;
        public static string IncomingDatabase;
        public static string OutgoingDatabase;
        public static string ChatFolder;
        public static string ServerAddress;
        public static List<Messaging> Messages= new List<Messaging>();
        public static int ActiveServerInfo;
        public static Server MyServer = new Server();
        public static string TargetHubAddress;
        public static string ServerName;
        public static string Identifier;

        internal static void Refresh()
        {
            using (DAL db = new DAL())
            {
                MyServer = db.Servers.Where(s => s.ID == ActiveServerInfo).FirstOrDefault();
                TargetHubAddress = MyServer.HubAddress;
                ServerName = MyServer.Name;
                Identifier = MyServer.Identifier;
                HubID = MyServer.HubID.ToString();
                Registered = MyServer.Registered;
                TargetsFolder = MyServer.TargetsFolder;
                OutgoingTargetName = MyServer.OutgoingTargetName;
                IncomingDatabase = MyServer.IncomingFolder;
                OutgoingDatabase = MyServer.OutgoingFolder;
                ChatFolder = MyServer.ChatFolder;
                ServerAddress = MyServer.Address;
            }
        }
            
    }
}