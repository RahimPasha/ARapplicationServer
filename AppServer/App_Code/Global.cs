using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;

namespace ARApplicationServer.App_Code
{
    public static class Global
    {
        public static List<Messaging> Messages = new List<Messaging>();

        public static string TargetHubAddress = ConfigurationManager.AppSettings["HubAddress"].ToString();
        public static string ServerName;
        public static string IncomingTargetName;
        public static string OutgoingTargetName;
        public static string ServerID;
        public static string TargetsFolder;
        public static string Registered;
        public static string Identifier;
        public static string incomingDatabase;
        public static string outgoingDatabase;
        public static string ChatFolder;
        public static string serverConfigFile = "Config" + "/" + "Config.xml";
        public static System.IO.FileInfo Dfile;
        public static List<string> Tags = new List<string>();
        public static XmlDocument xDoc = new XmlDocument(); // reading XML documents
            
    }
}