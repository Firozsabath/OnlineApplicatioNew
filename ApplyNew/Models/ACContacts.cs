using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class ACContact
    {
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public List<ACCustomFields> fieldValues { get; set; }
    }

    public class ACContactsample
    {
        public string ID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Program { get; set; }
        public string Enrollement_Term { get; set; }
        public string Nationality { get; set; }
        public string ProspectID { get; set; }
        public string Prospect_Date { get; set; }
        public string AgentOrganization { get; set; }
        public string Transfer_from_another_institution { get; set; }
        public string Hear_about_us { get; set; }
        public string Date_of_Birth { get; set; }
        public string Gender { get; set; }
        public string I_am_applying_for { get; set; }
        public string Application_Status { get; set; }
        public string Online_Application { get; set; }
        public string Utm_Source_Type { get; set; }
        public string Utm_Campaign_Name { get; set; }        
        public string Utm_Communication_Channel { get; set; }
        
    }

    public class ACContactsecondaryeloqua
    {
        public string Email { get; set; }
        public string ID { get; set; }
        public string Transportation { get; set; }
        public string NeededHousing { get; set; }
        public string NeededSpecial { get; set; }
        public string TransferToCanada { get; set; }
        public string UAE_resident { get; set; }
        public string NeededVisa { get; set; }
        public string Employercompany { get; set; }
        public string Position { get; set; }
        public string Guardiandetail_FirstName { get; set; }
        public string Guardiandetail_email { get; set; }
        public string Guardiandetail_Phone { get; set; }
        public string CollegeCountryName { get; set; }
        public string CollegeName { get; set; }
        public string HighSchoolCountryName { get; set; }
        public string HighschoolName { get; set; }
        public string HSCurriculum { get; set; }
        public string HighSchoolGrade { get; set; }
        public string University_you_currently_study_in { get; set; }
        public string ResidenceCountryName { get; set; }
        public string ResdienceStateName { get; set; }
        public string AppFeePaid { get; set; }
        public string Application_Status { get; set; }
    }

    public class ACContactsecondary
    {
        public string email { get; set; }       
        public List<ACCustomFields> fieldValues { get; set; }
    }

    public class ACEdupageDetails
    {
        public string HighschoolName { get; set; }
        public string HighSchoolCountryName { get; set; }
        public string HighSchoolGrade { get; set; }
        public string HighSchoolCurriculum { get; set; }
        public string CollegeName { get; set; }
        public string TransferCollegeName { get; set; }
        public string CollegeCountryName { get; set; }
        public string ResidenceCountryName { get; set; }
        public string ResdienceStateName { get; set; }
    }

    public class ACCustomFields
    {
        public string field { get; set; }
        public string value { get; set; }
    }

    public class ACJsonFieldvalues
    {
        public string contact { get; set; }
        public string field { get; set; }
        public string value { get; set; }
        public DateTime? cdate { get; set; }
        public DateTime? udate { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public string id { get; set; }
        public string owner { get; set; }
    }

    public class EloquaResponse
    {
        public int Eloqua_ID { get; set; }
    }   
}