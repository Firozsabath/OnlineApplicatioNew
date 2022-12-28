using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class Education
    {
        public int HighSchoolID { get; set; }
        public string HighSchoolName { get; set; }
        public List<Education> HighSchoollist { get; set; }
    }

    public class HighschoolType
    {
        public int UniqueId { get; set; }
        public string DisplayText { get; set; }
        public List<HighschoolType> TypeList { get; set; }
    }

    public class College
    {
        public int CollegeID { get; set; }
        public string CollegeName { get; set; }
        public List<College> CollegeList { get; set; }
    }

    public class state
    {
        public int UniqueId { get; set; }
        public string DisplayText { get; set; }
        public List<state> stateList { get; set; }
    }
}