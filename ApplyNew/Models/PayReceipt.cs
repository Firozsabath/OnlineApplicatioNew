using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class PayReceipt
    {
        public string prospectid { get; set; }
        public string response { get; set; }
        public string responsemessage { get; set; }
        public string amount { get; set; }
        public string approvalcode { get; set; }
        public string email { get; set; }
        public string Name { get; set; }
        public string locallang { get; set; }
    }
}