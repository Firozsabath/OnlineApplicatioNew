using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class CTGamedevelopment
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Email ID")]
        public string EmailID { get; set; }
        [Required]
        [Display(Name = "Confirm Email ID")]
        public string ConfirmEmailID { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Gender { get; set; }
        [Required]
        public string Nationality { get; set; }
        [Required]
        [Display(Name = "Country of Residence")]
        public string CountryofResidence { get; set; }
        [Required]
        [Display(Name = "Emirates / Passport ID")]
        public string EmiratesorPassport { get; set; }

        [Display(Name = "Level of Expertise")]
        public string ExpertLevel { get; set; }
        public string Phone { get; set; }
        public string Enquiry { get; set; }
    }

    public class GenderType
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
   
}