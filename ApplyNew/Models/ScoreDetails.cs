using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class ScoreDetails
    {
        public int CAMS_TestID { get; set; }
        public int CAMS_TestRefID { get; set; }
        // public int OwnerID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TestDate { get; set; }

        public decimal Score { get; set; }

        public string TestName { get; set; }
    }
}