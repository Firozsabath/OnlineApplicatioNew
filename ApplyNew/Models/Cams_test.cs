using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class Cams_test
    {
        public int CAMS_TestRefID { get; set; }
        public string TestName { get; set; }
        public string Description { get; set; }
        public int CAMS_TestScoreRefID { get; set; }
        public List<Cams_test> Cams_testlist { get; set; }
    }
}