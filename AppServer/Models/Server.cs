using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARApplicationServer.Models
{
    public class Server
    {
        public int ID { get; set; }
        public int HubID { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string RootFolder { get; set; }
        public string HubAddress { get; set; }
        public string Registered { get; set; }
        public string TargetsFolder { get; set; }
        public string IncomingFolder { get; set; }
        public string OutgoingFolder { get; set; }
        public string OutgoingTargetName { get; set; }
        public string ChatFolder { get; set; }
    }
}