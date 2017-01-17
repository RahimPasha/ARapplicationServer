using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARApplicationServer.Models
{
    public class Server
    {
        public int ID { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}