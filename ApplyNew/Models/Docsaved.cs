using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class Docsaved
    {
        public int DocNameID { get; set; }
        public string DocName { get; set; }
        public string  ContentType { get; set; }
        public string FIleName { get; set; }
    }
}