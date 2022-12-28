using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class ProgramList
    {
        [Required]
        public string Programid { get; set; }
        public string ProgramName { get; set; }
        public List<ProgramList> ProgramlistDB { get; set; }
    }

    public class Agencies
    {
        [Required]
        public int Elementno { get; set; }
        public string DisplayText { get; set; }
        public List<Agencies> AgencyList { get; set; }
    }

    public class Term
    {
        [Required]
        public int TermCalendarID { get; set; }
        public string TextTerm { get; set; }
        public List<Term> TermList { get; set; }
    }

    public class Country
    {
        [Required]
        public int UniqueId { get; set; }
        public string DisplayText { get; set; }
        public List<Country> CountryList { get; set; }
    }
}