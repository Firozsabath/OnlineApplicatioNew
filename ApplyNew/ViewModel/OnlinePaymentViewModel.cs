using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApplyNew.Models;

namespace ApplyNew.ViewModel
{
    public class OnlinePaymentViewModel
    {
        public Security Security { get; set; }
        public List<Custom_Profileprogress_Result> ProfileProgress { get; set; }
    }
}