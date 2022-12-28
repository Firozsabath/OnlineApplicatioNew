using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApplyNew.Models;
namespace ApplyNew.Models
{
    public class PaymentParameters
    {
        public string Firstname { get; set; }
        public string lastname { get; set; }
        public string middlename { get; set; }
        public string bill_to_email { get; set; }
        public string bill_to_address_line1 { get; set; }
        public string bill_to_address_city { get; set; }
        public string bill_to_address_state { get; set; }
        public string bill_to_address_country { get; set; }
        public string bill_to_address_postal_code { get; set; }
        public string reference_number { get; set; }
        public string getUid { get; set; }
        public string getUTCDateTime { get; set; }
        public string getip { get; set; }
        public string getsessionid { get; set; }
        public string ProfileID { get; set; }
        public string accesskey { get; set; }
        public string locallang { get; set; }
        public string Redirect_url { get; set; }

        public List<Custom_Profileprogress_Result> ProfileProgress { get; set; }
    }
}