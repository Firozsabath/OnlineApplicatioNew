using ApplyNew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplyNew.ViewModel
{
    public class LeadViewModel
    {
        public ProgramList Programex { get; set; }
        public accountdata Datalist { get; set; }
        public Agencies Agencylst { get; set; }
        public Term Termlst { get; set; }
        public Country Countrylst { get; set; }
        public List<Custom_Profileprogress_Result> ProfileProgress { get; set; }
         
    }
}