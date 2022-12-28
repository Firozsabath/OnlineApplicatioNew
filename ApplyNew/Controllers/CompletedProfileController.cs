using ApplyNew.Models;
using ApplyNew.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApplyNew.Controllers
{
    public class CompletedProfileController : Controller
    {
        // GET: CompletedProfile
        string prospectid = string.Empty;
        int ContactID = 0;
        string connectionstringex = System.Configuration.ConfigurationManager.AppSettings["Connectionstringex"];
        string connectioncams = System.Configuration.ConfigurationManager.AppSettings["ConnectionstringCams"];
        DataSet ds = new DataSet();
        CAMS_CustomTestnames Docsnm = new CAMS_CustomTestnames();
        CAMS_TestScoresEntities Scores = new CAMS_TestScoresEntities();
        CAMS_ProfileProgressEntities Progress = new CAMS_ProfileProgressEntities();
        public ActionResult Index()
        {
            if (Session["ProspectID"] != null)
            {
                prospectid = Session["ProspectID"].ToString();
            }
            else
            {
                return RedirectToAction("index", "OnlineApply");
            }
            // prospectid = "46118";
            List<Cams_test> Cams_testlisted = new List<Cams_test>();
            Cams_test test = new Cams_test();
            List<Country> Countrylistex = new List<Country>();
            Country Countrylt = new Country();
            List<HighschoolType> Typelistex = new List<HighschoolType>();
            HighschoolType HighschoolTypelt = new HighschoolType();
            List<state> statelist = new List<state>();
            state state = new state();
            DataSet dsopt = new DataSet();
            Optional RadioData = new Optional();
            HomeAddress Hadress = new HomeAddress();
            HighSchoolDetails HSData = new HighSchoolDetails();
            CollegeDetails Cdata = new CollegeDetails();
            List<Docsaved> docsavedlist = new List<Docsaved>();
            Guardian guardiandetails = new Guardian();
            DataSet dsview = new DataSet();
            DataSet ds3 = new DataSet();
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                sqlcon.Open();
                string spName = "CAMS_ApplicationPortalTestIDList";
                SqlCommand cmd = new SqlCommand(spName, sqlcon);
                cmd.Parameters.Add(new SqlParameter("@ProspectID", SqlDbType.VarChar)).Value = prospectid;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqldata1 = new SqlDataAdapter(cmd);
                sqldata1.Fill(ds);

                string spName12 = "OnlineAppDropdownspull";
                SqlCommand cmddrop = new SqlCommand(spName12, sqlcon);
                cmddrop.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqldatadrop = new SqlDataAdapter(cmddrop);
                sqldatadrop.Fill(ds3);

                sqlcon.Close();

                DataSet ds2 = new DataSet();
                string spName1 = "select UniqueId,DisplayText from Glossary where UniqueId in (select UniqueID from GlossaryCountry_FK where UniqueID <> 0)select * from Glossary where Category = 2050 order by DisplayText select * from Glossary where Category = '1003' order by DisplayText";
                SqlCommand cmd1 = new SqlCommand(spName1, sqlcon);
                SqlDataAdapter sqldata11 = new SqlDataAdapter(cmd1);
                sqldata11.Fill(ds2);

                string ViewName = "SELECT CONVERT(varchar(10),BirthDate,126) as BirthDate,* FROM  dbo.PreAdmission INNER JOIN dbo.Prospect_Address ON dbo.PreAdmission.ProspectID = dbo.Prospect_Address.ProspectID INNER JOIN dbo.Address ON dbo.Prospect_Address.AddressID = dbo.Address.AddressID INNER JOIN dbo.PreApplication_CUDCustom on dbo.PreApplication_CUDCustom.ProspectID = dbo.PreAdmission.ProspectID where PreAdmission.ProspectID = '" + prospectid + "' and Address.AddressTypeID = 287";
                SqlCommand cmd12 = new SqlCommand(ViewName, sqlcon);
                cmd1.CommandType = CommandType.Text;
                SqlDataAdapter sqldataforview = new SqlDataAdapter(cmd12);
                sqldataforview.Fill(dsview);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Cams_testlisted.Add(new Cams_test
                    {
                        Description = ds.Tables[0].Rows[i]["Description"].ToString(),
                        CAMS_TestRefID = Convert.ToInt16(ds.Tables[0].Rows[i]["CAMS_TestRefID"].ToString()),
                        CAMS_TestScoreRefID = Convert.ToInt16(ds.Tables[1].Rows[i]["CAMS_TestScoreRefID"].ToString()),
                        TestName = ds.Tables[0].Rows[i]["TestName"].ToString()
                    });
                }
                test.Cams_testlist = Cams_testlisted;


                for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                {
                    Country Cn = new Country();
                    Cn.UniqueId = Convert.ToInt16(ds2.Tables[0].Rows[i]["UniqueId"].ToString());
                    Cn.DisplayText = ds2.Tables[0].Rows[i]["DisplayText"].ToString();
                    Countrylistex.Add(Cn);
                }
                Countrylt.CountryList = Countrylistex;

                for (int i = 0; i < ds2.Tables[1].Rows.Count; i++)
                {
                    HighschoolType HT = new HighschoolType();
                    HT.UniqueId = Convert.ToInt16(ds2.Tables[1].Rows[i]["UniqueId"].ToString());
                    HT.DisplayText = ds2.Tables[1].Rows[i]["DisplayText"].ToString();
                    Typelistex.Add(HT);
                }
                HighschoolTypelt.TypeList = Typelistex;

                for (int i = 0; i < ds2.Tables[2].Rows.Count; i++)
                {
                    state st = new state();
                    st.UniqueId = Convert.ToInt16(ds2.Tables[2].Rows[i]["UniqueId"].ToString());
                    st.DisplayText = ds2.Tables[2].Rows[i]["DisplayText"].ToString();
                    statelist.Add(st);
                }
                state.stateList = statelist;

                string QryOptions = "Cams_CustomOptionaldatapull";
                SqlCommand cmdopt = new SqlCommand(QryOptions, sqlcon);
                cmdopt.Parameters.Add(new SqlParameter("@ProspectID", SqlDbType.VarChar)).Value = prospectid;
                cmdopt.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqldataopt = new SqlDataAdapter(cmdopt);
                sqldataopt.Fill(dsopt);

                if (dsopt.Tables[0].Rows.Count > 0)
                {
                    RadioData.NeededHousing = dsopt.Tables[0].Rows[0]["Housing"].ToString();
                    RadioData.NeededSpecial = dsopt.Tables[0].Rows[0]["NeededSpecial"].ToString();
                    RadioData.NeededSpecialNotes = dsopt.Tables[0].Rows[0]["NeededSpecialNotes"].ToString();
                    RadioData.NeededTransportation = dsopt.Tables[0].Rows[0]["NeededTransportation"].ToString();
                    RadioData.NeededVisa = dsopt.Tables[0].Rows[0]["NeededVisa"].ToString();
                    RadioData.TransferCredit = dsopt.Tables[0].Rows[0]["TransferCredit"].ToString();
                    RadioData.TransferFromHigherInstitution = dsopt.Tables[0].Rows[0]["TransferFromHigherInstitution"].ToString();
                    RadioData.TransferToCanada = dsopt.Tables[0].Rows[0]["TransferToCanada"].ToString();
                    RadioData.UsCitizen = dsopt.Tables[0].Rows[0]["USCitizen"].ToString();
                    RadioData.UAEID = dsopt.Tables[0].Rows[0]["NationalID"].ToString();
                    RadioData.Gpagroupdid = dsopt.Tables[0].Rows[0]["GPAGroupID"].ToString();
                    Hadress.age = Convert.ToInt32(dsopt.Tables[0].Rows[0]["Age"]);
                }
                if (dsopt.Tables[1].Rows.Count > 0)
                {
                    Hadress.Address1 = dsopt.Tables[1].Rows[0]["Address1"].ToString();
                    Hadress.Address2 = dsopt.Tables[1].Rows[0]["Address2"].ToString();
                    Hadress.Address3 = dsopt.Tables[1].Rows[0]["Address3"].ToString();
                    Hadress.CountryID = dsopt.Tables[1].Rows[0]["CountryID"].ToString();
                    Hadress.StateID = dsopt.Tables[1].Rows[0]["StateID"].ToString();
                    Hadress.state = dsopt.Tables[1].Rows[0]["County"].ToString();
                }
                if (dsopt.Tables[2].Rows.Count > 0)
                {
                    HSData.HighSchoolID = Convert.ToInt32(dsopt.Tables[2].Rows[0]["HighSchoolID"].ToString());
                    HSData.Score = dsopt.Tables[2].Rows[0]["HSGPA1"].ToString();
                    HSData.Scoreoutof = dsopt.Tables[2].Rows[0]["HSGPA2"].ToString();
                    HSData.Highschoolcountry = dsopt.Tables[2].Rows[0]["CountryID"].ToString();
                    HSData.Type = dsopt.Tables[2].Rows[0]["UserDefGlossary3"].ToString();
                    HSData.Customhschool = dsopt.Tables[2].Rows[0]["CustomHighSchoolName"].ToString();
                }
                if (dsopt.Tables[3].Rows.Count > 0)
                {
                    Cdata.CollegeID = Convert.ToInt32(dsopt.Tables[3].Rows[0]["CollegeID"].ToString());
                    Cdata.Grade = dsopt.Tables[3].Rows[0]["GradePoint"].ToString();
                    Cdata.Gradeoutof = dsopt.Tables[3].Rows[0]["GradePointOf"].ToString();
                    Cdata.CollegeCountry = dsopt.Tables[3].Rows[0]["CountryID"].ToString();
                    Cdata.TransferInstitute = dsopt.Tables[3].Rows[0]["TransferInstitution"].ToString();
                }
                if (dsopt.Tables[4].Rows.Count > 0)
                {
                    for (int i = 0; i < dsopt.Tables[4].Rows.Count; i++)
                    {
                        Docsaved dcs = new Docsaved();
                        dcs.DocNameID = Convert.ToInt32(dsopt.Tables[4].Rows[i]["DocNameID"].ToString());
                        dcs.DocName = dsopt.Tables[4].Rows[i]["DocDescription"].ToString();
                        dcs.FIleName = dsopt.Tables[4].Rows[i]["FileName"].ToString();
                        dcs.ContentType = dsopt.Tables[4].Rows[i]["ContentType"].ToString();
                        docsavedlist.Add(dcs);
                    }
                }
                if (dsopt.Tables[5].Rows.Count > 0)
                {
                    guardiandetails.FirstName = dsopt.Tables[5].Rows[0]["FirstName"].ToString();
                    guardiandetails.LastName = dsopt.Tables[5].Rows[0]["LastName"].ToString();
                    guardiandetails.Phone = dsopt.Tables[5].Rows[0]["Phone1"].ToString();
                    guardiandetails.Relationship = dsopt.Tables[5].Rows[0]["Title"].ToString();
                    guardiandetails.Gemail = dsopt.Tables[5].Rows[0]["Institution"].ToString();
                    guardiandetails.Company = dsopt.Tables[5].Rows[0]["CompanyDetails"].ToString();
                    guardiandetails.Position = dsopt.Tables[5].Rows[0]["JobTitle"].ToString();
                    Session["ContactID"] = dsopt.Tables[5].Rows[0]["ContactID"].ToString();
                }
            }

            List<Agencies> Agenclistex = new List<Agencies>();
            Agencies Agency = new Agencies();
            for (int i = 0; i < ds3.Tables[1].Rows.Count; i++)
            {
                Agencies Ag = new Agencies();
                Ag.Elementno = Convert.ToInt16(ds3.Tables[1].Rows[i]["UniqueId"].ToString());
                Ag.DisplayText = ds3.Tables[1].Rows[i]["DisplayText"].ToString();
                Agenclistex.Add(Ag);
            }
            Agency.AgencyList = Agenclistex;

            List<Term> Termlistex = new List<Term>();
            Term Term = new Term();
            for (int i = 0; i < ds3.Tables[2].Rows.Count; i++)
            {
                Term Tm = new Term();
                Tm.TermCalendarID = Convert.ToInt16(ds3.Tables[2].Rows[i]["TermCalendarID"].ToString());
                Tm.TextTerm = ds3.Tables[2].Rows[i]["TextTerm"].ToString();
                Termlistex.Add(Tm);
            }
            Term.TermList = Termlistex;

            accountdata data = new accountdata();
            if (dsview.Tables[0].Rows.Count > 0)
            {
                data.Firstname = dsview.Tables[0].Rows[0]["Firstname"].ToString();
                data.Lastname = dsview.Tables[0].Rows[0]["Lastname"].ToString();
                data.Middlename = dsview.Tables[0].Rows[0]["MiddleInitial"].ToString();
                data.salutation = dsview.Tables[0].Rows[0]["salutation"].ToString();
                data.phone1 = dsview.Tables[0].Rows[0]["phone1"].ToString();
                data.phone2 = dsview.Tables[0].Rows[0]["phone2"].ToString();
                if (dsview.Tables[0].Rows[0]["BirthDate"].ToString() != "")
                {
                    data.Birdthdate = dsview.Tables[0].Rows[0]["BirthDate"].ToString();
                }
                data.program1 = dsview.Tables[0].Rows[0]["ProgramID"].ToString();
                data.leadsource = dsview.Tables[0].Rows[0]["EventID"].ToString();
                data.CountryID = dsview.Tables[0].Rows[0]["CitizenCountryID"].ToString();
                data.Email = dsview.Tables[0].Rows[0]["Email1"].ToString();
                data.verifyEmail = dsview.Tables[0].Rows[0]["Email1"].ToString();
                data.Gender = dsview.Tables[0].Rows[0]["SexCode"].ToString();
                data.TermcalendarID = dsview.Tables[0].Rows[0]["AnticipatedEntryTermID"].ToString();
                data.Gpagroupdid = dsview.Tables[0].Rows[0]["gpagroupid"].ToString();
                data.Transferstatus = dsview.Tables[0].Rows[0]["TransferCredit"].Equals(1) ? "Yes" : "No";
            }

            List<Custom_DoctNametable> Documents = Docsnm.Custom_DoctNametable.ToList();
            Custom_DoctNametable DocumentNames = new Custom_DoctNametable();
            DocumentNames.DocNamelst = Documents;

            List<MathApptestscores_Result> Scorelist = Scores.MathApptestscores(prospectid).ToList();
            MathApptestscores_Result TestScoreList = new MathApptestscores_Result();
            TestScoreList.Scorelist = Scorelist;

            List<Custom_Profileprogress_Result> ProfileProgress = Progress.Custom_Profileprogress(prospectid).ToList();

            Completeprofiledt viewmodel = new Completeprofiledt()
            {
                Agencylst = Agency,
                Termlst = Term,
                Examlist = test,
                DocumentList = DocumentNames,
                TestScoreList = TestScoreList,
                Countrylist = Countrylt,
                Typelist = HighschoolTypelt,
                ProfileProgress = ProfileProgress,
                statelst = state,
                OptionalData = RadioData,
                HomeAddressFields = Hadress,
                HighSData = HSData,
                CollegeData = Cdata,
                SavedDocList = docsavedlist,
                GuardianData = guardiandetails,
                LeadInfo = data
            };

            return View(viewmodel);
        }

        
        public ActionResult StudentAgentInfo(string ProspectID)
        {           
            prospectid = ProspectID;
            if(String.IsNullOrWhiteSpace(ProspectID))
            {

            }
            List<Cams_test> Cams_testlisted = new List<Cams_test>();
            Cams_test test = new Cams_test();
            List<Country> Countrylistex = new List<Country>();
            Country Countrylt = new Country();
            List<HighschoolType> Typelistex = new List<HighschoolType>();
            HighschoolType HighschoolTypelt = new HighschoolType();
            List<state> statelist = new List<state>();
            state state = new state();
            DataSet dsopt = new DataSet();
            Optional RadioData = new Optional();
            HomeAddress Hadress = new HomeAddress();
            HighSchoolDetails HSData = new HighSchoolDetails();
            CollegeDetails Cdata = new CollegeDetails();
            List<Docsaved> docsavedlist = new List<Docsaved>();
            Guardian guardiandetails = new Guardian();
            DataSet dsview = new DataSet();
            DataSet ds3 = new DataSet();
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(connectioncams))
                {
                    sqlcon.Open();
                    string spName = "CAMS_ApplicationPortalTestIDList";
                    SqlCommand cmd = new SqlCommand(spName, sqlcon);
                    cmd.Parameters.Add(new SqlParameter("@ProspectID", SqlDbType.VarChar)).Value = prospectid;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqldata1 = new SqlDataAdapter(cmd);
                    sqldata1.Fill(ds);

                    string spName12 = "OnlineAppDropdownspull";
                    SqlCommand cmddrop = new SqlCommand(spName12, sqlcon);
                    cmddrop.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqldatadrop = new SqlDataAdapter(cmddrop);
                    sqldatadrop.Fill(ds3);

                    sqlcon.Close();

                    DataSet ds2 = new DataSet();
                    string spName1 = "select UniqueId,DisplayText from Glossary where UniqueId in (select UniqueID from GlossaryCountry_FK where UniqueID <> 0)select * from Glossary where Category = 2050 order by DisplayText select * from Glossary where Category = '1003' order by DisplayText";
                    SqlCommand cmd1 = new SqlCommand(spName1, sqlcon);
                    SqlDataAdapter sqldata11 = new SqlDataAdapter(cmd1);
                    sqldata11.Fill(ds2);

                    string ViewName = "SELECT CONVERT(varchar(10),BirthDate,126) as BirthDate,* FROM  dbo.PreAdmission INNER JOIN dbo.Prospect_Address ON dbo.PreAdmission.ProspectID = dbo.Prospect_Address.ProspectID INNER JOIN dbo.Address ON dbo.Prospect_Address.AddressID = dbo.Address.AddressID INNER JOIN dbo.PreApplication_CUDCustom on dbo.PreApplication_CUDCustom.ProspectID = dbo.PreAdmission.ProspectID where PreAdmission.ProspectID = '" + prospectid + "' and Address.AddressTypeID = 287";
                    SqlCommand cmd12 = new SqlCommand(ViewName, sqlcon);
                    cmd1.CommandType = CommandType.Text;
                    SqlDataAdapter sqldataforview = new SqlDataAdapter(cmd12);
                    sqldataforview.Fill(dsview);

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Cams_testlisted.Add(new Cams_test
                        {
                            Description = ds.Tables[0].Rows[i]["Description"].ToString(),
                            CAMS_TestRefID = Convert.ToInt16(ds.Tables[0].Rows[i]["CAMS_TestRefID"].ToString()),
                            CAMS_TestScoreRefID = Convert.ToInt16(ds.Tables[1].Rows[i]["CAMS_TestScoreRefID"].ToString()),
                            TestName = ds.Tables[0].Rows[i]["TestName"].ToString()
                        });
                    }
                    test.Cams_testlist = Cams_testlisted;


                    for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                    {
                        Country Cn = new Country();
                        Cn.UniqueId = Convert.ToInt16(ds2.Tables[0].Rows[i]["UniqueId"].ToString());
                        Cn.DisplayText = ds2.Tables[0].Rows[i]["DisplayText"].ToString();
                        Countrylistex.Add(Cn);
                    }
                    Countrylt.CountryList = Countrylistex;

                    for (int i = 0; i < ds2.Tables[1].Rows.Count; i++)
                    {
                        HighschoolType HT = new HighschoolType();
                        HT.UniqueId = Convert.ToInt16(ds2.Tables[1].Rows[i]["UniqueId"].ToString());
                        HT.DisplayText = ds2.Tables[1].Rows[i]["DisplayText"].ToString();
                        Typelistex.Add(HT);
                    }
                    HighschoolTypelt.TypeList = Typelistex;

                    for (int i = 0; i < ds2.Tables[2].Rows.Count; i++)
                    {
                        state st = new state();
                        st.UniqueId = Convert.ToInt16(ds2.Tables[2].Rows[i]["UniqueId"].ToString());
                        st.DisplayText = ds2.Tables[2].Rows[i]["DisplayText"].ToString();
                        statelist.Add(st);
                    }
                    state.stateList = statelist;

                    string QryOptions = "Cams_CustomOptionaldatapull";
                    SqlCommand cmdopt = new SqlCommand(QryOptions, sqlcon);
                    cmdopt.Parameters.Add(new SqlParameter("@ProspectID", SqlDbType.VarChar)).Value = prospectid;
                    cmdopt.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqldataopt = new SqlDataAdapter(cmdopt);
                    sqldataopt.Fill(dsopt);

                    if (dsopt.Tables[0].Rows.Count > 0)
                    {
                        RadioData.NeededHousing = dsopt.Tables[0].Rows[0]["Housing"].ToString();
                        RadioData.NeededSpecial = dsopt.Tables[0].Rows[0]["NeededSpecial"].ToString();
                        RadioData.NeededSpecialNotes = dsopt.Tables[0].Rows[0]["NeededSpecialNotes"].ToString();
                        RadioData.NeededTransportation = dsopt.Tables[0].Rows[0]["NeededTransportation"].ToString();
                        RadioData.NeededVisa = dsopt.Tables[0].Rows[0]["NeededVisa"].ToString();
                        RadioData.TransferCredit = dsopt.Tables[0].Rows[0]["TransferCredit"].ToString();
                        RadioData.TransferFromHigherInstitution = dsopt.Tables[0].Rows[0]["TransferFromHigherInstitution"].ToString();
                        RadioData.TransferToCanada = dsopt.Tables[0].Rows[0]["TransferToCanada"].ToString();
                        RadioData.UsCitizen = dsopt.Tables[0].Rows[0]["USCitizen"].ToString();
                        RadioData.UAEID = dsopt.Tables[0].Rows[0]["NationalID"].ToString();
                        RadioData.Gpagroupdid = dsopt.Tables[0].Rows[0]["GPAGroupID"].ToString();
                        Hadress.age = Convert.ToInt32(dsopt.Tables[0].Rows[0]["Age"]);
                    }
                    if (dsopt.Tables[1].Rows.Count > 0)
                    {
                        Hadress.Address1 = dsopt.Tables[1].Rows[0]["Address1"].ToString();
                        Hadress.Address2 = dsopt.Tables[1].Rows[0]["Address2"].ToString();
                        Hadress.Address3 = dsopt.Tables[1].Rows[0]["Address3"].ToString();
                        Hadress.CountryID = dsopt.Tables[1].Rows[0]["CountryID"].ToString();
                        Hadress.StateID = dsopt.Tables[1].Rows[0]["StateID"].ToString();
                        Hadress.state = dsopt.Tables[1].Rows[0]["County"].ToString();
                    }
                    if (dsopt.Tables[2].Rows.Count > 0)
                    {
                        HSData.HighSchoolID = Convert.ToInt32(dsopt.Tables[2].Rows[0]["HighSchoolID"].ToString());
                        HSData.Score = dsopt.Tables[2].Rows[0]["HSGPA1"].ToString();
                        HSData.Scoreoutof = dsopt.Tables[2].Rows[0]["HSGPA2"].ToString();
                        HSData.Highschoolcountry = dsopt.Tables[2].Rows[0]["CountryID"].ToString();
                        HSData.Type = dsopt.Tables[2].Rows[0]["UserDefGlossary3"].ToString();
                        HSData.Customhschool = dsopt.Tables[2].Rows[0]["CustomHighSchoolName"].ToString();
                    }
                    if (dsopt.Tables[3].Rows.Count > 0)
                    {
                        Cdata.CollegeID = Convert.ToInt32(dsopt.Tables[3].Rows[0]["CollegeID"].ToString());
                        Cdata.Grade = dsopt.Tables[3].Rows[0]["GradePoint"].ToString();
                        Cdata.Gradeoutof = dsopt.Tables[3].Rows[0]["GradePointOf"].ToString();
                        Cdata.CollegeCountry = dsopt.Tables[3].Rows[0]["CountryID"].ToString();
                        Cdata.TransferInstitute = dsopt.Tables[3].Rows[0]["TransferInstitution"].ToString();
                    }
                    if (dsopt.Tables[4].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsopt.Tables[4].Rows.Count; i++)
                        {
                            Docsaved dcs = new Docsaved();
                            dcs.DocNameID = Convert.ToInt32(dsopt.Tables[4].Rows[i]["DocNameID"].ToString());
                            dcs.DocName = dsopt.Tables[4].Rows[i]["DocDescription"].ToString();
                            dcs.FIleName = dsopt.Tables[4].Rows[i]["FileName"].ToString();
                            dcs.ContentType = dsopt.Tables[4].Rows[i]["ContentType"].ToString();
                            docsavedlist.Add(dcs);
                        }
                    }
                    if (dsopt.Tables[5].Rows.Count > 0)
                    {
                        guardiandetails.FirstName = dsopt.Tables[5].Rows[0]["FirstName"].ToString();
                        guardiandetails.LastName = dsopt.Tables[5].Rows[0]["LastName"].ToString();
                        guardiandetails.Phone = dsopt.Tables[5].Rows[0]["Phone1"].ToString();
                        guardiandetails.Relationship = dsopt.Tables[5].Rows[0]["Title"].ToString();
                        guardiandetails.Gemail = dsopt.Tables[5].Rows[0]["Institution"].ToString();
                        guardiandetails.Company = dsopt.Tables[5].Rows[0]["CompanyDetails"].ToString();
                        guardiandetails.Position = dsopt.Tables[5].Rows[0]["JobTitle"].ToString();
                        Session["ContactID"] = dsopt.Tables[5].Rows[0]["ContactID"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                var sw = new System.IO.StreamWriter(filename, true);
                sw.WriteLine(DateTime.Now.ToString() + " " + ex.Message + " " + ex.InnerException);
                sw.Close();
                throw;
            }

            List<Agencies> Agenclistex = new List<Agencies>();
            Agencies Agency = new Agencies();
            accountdata data = new accountdata();
            Term Term = new Term();
            try
            {
                for (int i = 0; i < ds3.Tables[1].Rows.Count; i++)
                {
                    Agencies Ag = new Agencies();
                    Ag.Elementno = Convert.ToInt16(ds3.Tables[1].Rows[i]["UniqueId"].ToString());
                    Ag.DisplayText = ds3.Tables[1].Rows[i]["DisplayText"].ToString();
                    Agenclistex.Add(Ag);
                }
                Agency.AgencyList = Agenclistex;

                List<Term> Termlistex = new List<Term>();                
                for (int i = 0; i < ds3.Tables[2].Rows.Count; i++)
                {
                    Term Tm = new Term();
                    Tm.TermCalendarID = Convert.ToInt16(ds3.Tables[2].Rows[i]["TermCalendarID"].ToString());
                    Tm.TextTerm = ds3.Tables[2].Rows[i]["TextTerm"].ToString();
                    Termlistex.Add(Tm);
                }
                Term.TermList = Termlistex;

                if (dsview.Tables[0].Rows.Count > 0)
                {
                    data.Firstname = dsview.Tables[0].Rows[0]["Firstname"].ToString();
                    data.Lastname = dsview.Tables[0].Rows[0]["Lastname"].ToString();
                    data.Middlename = dsview.Tables[0].Rows[0]["MiddleInitial"].ToString();
                    data.salutation = dsview.Tables[0].Rows[0]["salutation"].ToString();
                    data.phone1 = dsview.Tables[0].Rows[0]["phone1"].ToString();
                    data.phone2 = dsview.Tables[0].Rows[0]["phone2"].ToString();
                    if (dsview.Tables[0].Rows[0]["BirthDate"].ToString() != "")
                    {
                        data.Birdthdate = dsview.Tables[0].Rows[0]["BirthDate"].ToString();
                    }
                    data.program1 = dsview.Tables[0].Rows[0]["ProgramID"].ToString();
                    data.leadsource = dsview.Tables[0].Rows[0]["EventID"].ToString();
                    data.CountryID = dsview.Tables[0].Rows[0]["CitizenCountryID"].ToString();
                    data.Email = dsview.Tables[0].Rows[0]["Email1"].ToString();
                    data.verifyEmail = dsview.Tables[0].Rows[0]["Email1"].ToString();
                    data.Gender = dsview.Tables[0].Rows[0]["SexCode"].ToString();
                    data.TermcalendarID = dsview.Tables[0].Rows[0]["AnticipatedEntryTermID"].ToString();
                    data.Gpagroupdid = dsview.Tables[0].Rows[0]["gpagroupid"].ToString();
                    data.Transferstatus = dsview.Tables[0].Rows[0]["TransferCredit"].Equals(1) ? "Yes" : "No";
                }
            }
            catch (Exception ex)
            {
                var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                var sw = new System.IO.StreamWriter(filename, true);
                sw.WriteLine(DateTime.Now.ToString() + " " + ex.Message + " " + ex.InnerException);
                sw.Close();               
                throw;
            }

            List<Custom_DoctNametable> Documents = Docsnm.Custom_DoctNametable.ToList();
            Custom_DoctNametable DocumentNames = new Custom_DoctNametable();
            DocumentNames.DocNamelst = Documents;

            List<MathApptestscores_Result> Scorelist = Scores.MathApptestscores(prospectid).ToList();
            MathApptestscores_Result TestScoreList = new MathApptestscores_Result();
            TestScoreList.Scorelist = Scorelist;

            List<Custom_Profileprogress_Result> ProfileProgress = Progress.Custom_Profileprogress(prospectid).ToList();

            Completeprofiledt viewmodel = new Completeprofiledt()
            {
                Agencylst = Agency,
                Termlst = Term,
                Examlist = test,
                DocumentList = DocumentNames,
                TestScoreList = TestScoreList,
                Countrylist = Countrylt,
                Typelist = HighschoolTypelt,
                ProfileProgress = ProfileProgress,
                statelst = state,
                OptionalData = RadioData,
                HomeAddressFields = Hadress,
                HighSData = HSData,
                CollegeData = Cdata,
                SavedDocList = docsavedlist,
                GuardianData = guardiandetails,
                LeadInfo = data
            };

            return View(viewmodel);
        }

        public JsonResult GetProgramByTypeId(int Typeid)
        {
            List<ProgramList> progrList = new List<ProgramList>();
            SelectList Colleges;
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                DataSet dshs = new DataSet();
                string SqlQuery = "select Programs,ProgramsID from Programs where ISNULL(Programs,'') != '' and GPAGroupID = '" + Typeid + "'";
                SqlCommand cmd = new SqlCommand(SqlQuery, sqlcon);
                SqlDataAdapter sqldata = new SqlDataAdapter(cmd);
                sqldata.Fill(dshs);
                for (int i = 0; i < dshs.Tables[0].Rows.Count; i++)
                {
                    progrList.Add(new ProgramList
                    {
                        ProgramName = dshs.Tables[0].Rows[i]["Programs"].ToString(),
                        Programid = Convert.ToInt16(dshs.Tables[0].Rows[i]["ProgramsID"]).ToString()
                    });
                }
                Colleges = new SelectList(progrList, "Programid", "ProgramName", 0);
            }
            return Json(Colleges, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCollegeByCountryId(int Countryid)
        {
            List<College> CollegeList = new List<College>();
            SelectList Colleges;
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                DataSet dshs = new DataSet();
                string SqlQuery = "select CollegeID,CollegeName from CAMS_Colleges_View where CollegeID > 0 and CountryID = '" + Countryid + "'";
                SqlCommand cmd = new SqlCommand(SqlQuery, sqlcon);
                SqlDataAdapter sqldata = new SqlDataAdapter(cmd);
                sqldata.Fill(dshs);

                for (int i = 0; i < dshs.Tables[0].Rows.Count; i++)
                {
                    CollegeList.Add(new College
                    {
                        CollegeName = dshs.Tables[0].Rows[i]["CollegeName"].ToString(),
                        CollegeID = Convert.ToInt16(dshs.Tables[0].Rows[i]["CollegeID"].ToString())
                    });
                }
                Colleges = new SelectList(CollegeList, "CollegeID", "CollegeName", 0);
            }
            return Json(Colleges, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHighSchoolByCountryId(int Countryid)
        {
            List<Education> HighschoolList = new List<Education>();
            SelectList hgschools;
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                DataSet dshs = new DataSet();
                string SqlQuery = "select HighSchoolID,HighSchoolName from CAMS_HighSchools_View where HighSchoolID > 0 and CountryID = '" + Countryid + "'";
                SqlCommand cmd = new SqlCommand(SqlQuery, sqlcon);
                SqlDataAdapter sqldata = new SqlDataAdapter(cmd);
                sqldata.Fill(dshs);

                for (int i = 0; i < dshs.Tables[0].Rows.Count; i++)
                {
                    HighschoolList.Add(new Education
                    {
                        HighSchoolName = dshs.Tables[0].Rows[i]["HighSchoolName"].ToString(),
                        HighSchoolID = Convert.ToInt16(dshs.Tables[0].Rows[i]["HighSchoolID"].ToString())
                    });
                }
                hgschools = new SelectList(HighschoolList, "HighSchoolID", "HighSchoolName", 0);
            }
            return Json(hgschools, JsonRequestBehavior.AllowGet);
        }
    }
}