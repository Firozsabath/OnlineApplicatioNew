using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class OnlineApp_Otp
    {
        public long ID { get; set; }
        public string Email { get; set; }
        public int OTP { get; set; }
        public DateTime? SendDate { get; set; }
        public bool Verified { get; set; }
        public DateTime? VerifyDate { get; set; }
    }
}