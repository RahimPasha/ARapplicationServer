using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ARApplicationServer.Models
{
    public class Target
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public string XmlFilePath { get; set; }
        public string DatFilePath { get; set; }
        public string ChatFilePath { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}