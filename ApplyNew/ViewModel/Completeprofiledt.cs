using ApplyNew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplyNew.ViewModel
{
    public class Completeprofiledt
    {
        public Agencies Agencylst { get; set; }
        public Term Termlst { get; set; }
        //public Country Countrylst { get; set; }
        public Cams_test Examlist { get; set; }
        public Custom_DoctNametable DocumentList { get; set; }
        public MathApptestscores_Result TestScoreList { get; set; }
        public Country Countrylist { get; set; }
        public HighschoolType Typelist { get; set; }
        public List<Custom_Profileprogress_Result> ProfileProgress { get; set; }
        public state statelst { get; set; }
        public Optional OptionalData { get; set; }
        public HomeAddress HomeAddressFields { get; set; }
        public HighSchoolDetails HighSData { get; set; }
        public CollegeDetails CollegeData { get; set; }
        public int MyProperty { get; set; }
        public List<Docsaved> SavedDocList { get; set; }
        public Guardian GuardianData { get; set; }
        public Employer EmployerData { get; set; }
        public accountdata LeadInfo { get; set; }
    }
}