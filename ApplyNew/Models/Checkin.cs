using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class Checkin
    {
        [Required]
        public int ProspectID { get; set; }
        [Required]
        public string  EmailID { get; set; }
    }
}