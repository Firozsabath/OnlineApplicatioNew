using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class HighSchoolDetails
    {
        public int HighSchoolID { get; set; }
        public string  Type { get; set; }
        public string Score { get; set; }
        public string Scoreoutof { get; set; }
        public string Highschoolcountry { get; set; }
        public string Customhschool { get; set; }
        public string TransferInstitute { get; set; }
    }
}