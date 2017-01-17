using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ARApplicationServer.Models
{
    public class Tag
    {
        public int ID { get; set; }
        public int TargetID { get; set; }
        public string tag { get; set; }
        [ForeignKey("TargetID")]
        public virtual Target Target { get; set; }
    }
}