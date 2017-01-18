using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ARApplicationServer.Models
{
    public class User
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
    }
}