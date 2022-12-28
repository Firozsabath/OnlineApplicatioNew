using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class accountdata
    {
       
        public string salutation { get; set; }

        //[Required]
        public string Firstname { get; set; }
        
        public string Middlename { get; set; }

        //[Required]
        public string Lastnamear { get; set; }

        public string Firstnamear { get; set; }

        public string Middlenamear { get; set; }

        //[Required]
        public string Lastname { get; set; }

        //[Required]
        //[DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string phone1 { get; set; }

        public string phone2 { get; set; }

       // [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        public string Birdthdate { get; set; }

        //[Required]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please provide a valid email.")]
        [Compare("Email", ErrorMessage = "The email and confirmation do not match.")]
        public string verifyEmail { get; set; }

        //[Required]
        public string password { get; set; }

        //[Required]
        //[Compare("password", ErrorMessage = "The email and confirmation do not match.")]
        public string ConfirmPassword { get; set; }

        public string program1 { get; set; }
        //[Required(ErrorMessage ="Rquired")]
        public string leadsource { get; set; }
        //[Required(ErrorMessage = "Rquired")]
        public string CountryID { get; set; }      
        public string TermcalendarID { get; set; }
        public string Gpagroupdid { get; set; }
        public string Transferstatus { get; set; }
        public string AgentCode { get; set; }

        public string[] Genders = new[] { "Male", "Female" };
    }
}