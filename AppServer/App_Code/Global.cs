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
        public static List<Messaging> Messages = new List<Messaging>();

        public static string TargetHubAddress = ConfigurationManager.AppSettings["HubAddress"].ToString();
        public static string ServerName = ConfigurationManager.AppSettings["Name"].ToString();
        public static string Identifier = ConfigurationManager.AppSettings["Identifier"].ToString();
        public static string ServerID = ConfigurationManager.AppSettings["ID"].ToString();
        public static string Registered = ConfigurationManager.AppSettings["Registered"].ToString();

        public static string TargetsFolder = ConfigurationManager.AppSettings["TargetsFolder"].ToString();

        public static string IncomingTargetName = ConfigurationManager.AppSettings["IncomingTargetName"].ToString();
        public static string OutgoingTargetName = ConfigurationManager.AppSettings["OutgoingTargetName"].ToString();
        public static string IncomingDatabase = ConfigurationManager.AppSettings["IncomingFolder"].ToString();
        public static string OutgoingDatabase = ConfigurationManager.AppSettings["OutgoingFolder"].ToString();
        public static string ChatFolder = ConfigurationManager.AppSettings["ChatFolder"].ToString();
        //TODO: address of the server that is hosting the application should be retrived. 
        public static string ServerAddress = "http://localhost:7204"; //getServerAddress();

        private static string getServerAddress()
        {
            string strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(strHostName);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            return ipHostInfo.HostName;
        }
            
    }
}