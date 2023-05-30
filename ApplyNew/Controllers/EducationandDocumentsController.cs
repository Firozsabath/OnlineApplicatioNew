using ApplyNew.Models;
using ApplyNew.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace ApplyNew.Controllers
{
    public class EducationandDocumentsController : Controller
    {
        // GET: EducationandDocuments
        string prospectid = string.Empty;
        int ContactID = 0;
        string connectionstringex = System.Configuration.ConfigurationManager.AppSettings["Connectionstringex"];
        string connectioncams = System.Configuration.ConfigurationManager.AppSettings["ConnectionstringCams"];
        DataSet ds = new DataSet();
        CAMS_CustomTestnames Docsnm = new CAMS_CustomTestnames();
        CAMS_TestScoresEntities Scores = new CAMS_TestScoresEntities();
        CAMS_ProfileProgressEntities Progress = new CAMS_ProfileProgressEntities();
        ActiveCampaignSF ActiveCampaign = new ActiveCampaignSF();
        ACContactFIelds AC = new ACContactFIelds();

        [Authorize]
        public ActionResult EducationDoc(string language)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);

            if (Session["ProspectID"] != null)
            {
                prospectid = Session["ProspectID"].ToString();
                ViewBag.hdnprospectid = prospectid;
            }
            //else
            //{
            //    return RedirectToAction("index", "OnlineApply");
            //}
            //prospectid = "46022";
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
            TransferInstitutedtls trdata = new TransferInstitutedtls();
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                sqlcon.Open();
                string spName = "CAMS_ApplicationPortalTestIDList";
                SqlCommand cmd = new SqlCommand(spName, sqlcon);
                cmd.Parameters.Add(new SqlParameter("@ProspectID", SqlDbType.VarChar)).Value = prospectid;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqldata1 = new SqlDataAdapter(cmd);
                sqldata1.Fill(ds);
                sqlcon.Close();

                DataSet ds2 = new DataSet();
                string spName1 = "select UniqueId,DisplayText from Glossary where UniqueId in (select UniqueID from GlossaryCountry_FK where UniqueID <> 0)select * from Glossary where Category = 1094 and USERProtect = 0 order by DisplayText select * from Glossary where Category = '1003' order by DisplayText";
                SqlCommand cmd1 = new SqlCommand(spName1, sqlcon);
                SqlDataAdapter sqldata11 = new SqlDataAdapter(cmd1);
                sqldata11.Fill(ds2);

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
                    RadioData.UAEIDExpDate = dsopt.Tables[0].Rows[0]["ExpiryDate"].ToString();
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
                if (dsopt.Tables[6].Rows.Count > 0)
                {
                    trdata.TransferInstitute = dsopt.Tables[6].Rows[0]["TransferInstitution"].ToString();
                    trdata.Trcountry = dsopt.Tables[6].Rows[0]["TransferCountry"].ToString();
                    trdata.TrProgram = dsopt.Tables[6].Rows[0]["TransferProgram"].ToString();
                }
            }

            List<Custom_DoctNametable> Documents = Docsnm.Custom_DoctNametable.Where(x => x.DegreeType == RadioData.Gpagroupdid && x.TransferType == RadioData.TransferCredit).ToList();
            //List<Custom_DoctNametable> Documents = Docsnm.Custom_DoctNametable.ToList();
            Custom_DoctNametable DocumentNames = new Custom_DoctNametable();
            DocumentNames.DocNamelst = Documents;

            List<MathApptestscores_Result> Scorelist = Scores.MathApptestscores(prospectid).ToList();
            MathApptestscores_Result TestScoreList = new MathApptestscores_Result();
            TestScoreList.Scorelist = Scorelist;

            List<Custom_Profileprogress_Result> ProfileProgress = Progress.Custom_Profileprogress(prospectid).ToList();

            TestdocViewModel viewmodel = new TestdocViewModel()
            {
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
                Trdata = trdata
            };

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult InsertCamstestscores(List<ScoreDetails> Scoredetailsed, ACEdupageDetails AcDetails, List<Optional> optional, List<HomeAddress> Addressfields, List<HighSchoolDetails> HighSchoolDetail, List<CollegeDetails> Collegedetail, List<Guardian> Guardiandetail, List<TransferInstitutedtls> TransferDtls)
        {
            if (Session["ProspectID"] != null)
            {
                prospectid = Session["ProspectID"].ToString();
            }
            if (Session["ContactID"] != null)
            {
                ContactID = Convert.ToInt32(Session["ContactID"].ToString());
            }
            //var highschoolName = fm["hdnHighSchoolName"].ToString();
            //prospectid = "46022";  
            if (Scoredetailsed != null)
            {
                if (Scoredetailsed[0].CAMS_TestRefID != 0)
                {
                    XDocument StudentScores = new XDocument(new XDeclaration("1.0", "UTF - 8", "yes"),
                     new XElement("StudentScores",
                                from OrderDet in Scoredetailsed
                                select new XElement("OrderDetails",
                                new XElement("CAMS_TestID", OrderDet.CAMS_TestID),
                                new XElement("CAMS_TestRefID", OrderDet.CAMS_TestRefID),
                                new XElement("Score", OrderDet.Score),
                                new XElement("TestDate", OrderDet.TestDate.ToString() == "1/1/0001" ? string.Empty : OrderDet.TestDate.ToString()),
                                new XElement("TestName", OrderDet.TestName)
                                )));
                    string xml = StudentScores.ToString();

                    using (SqlConnection sqlcon = new SqlConnection(connectioncams))
                    {
                        sqlcon.Open();
                        string spName = "Custom_TestScoreInsert";
                        SqlCommand cmd = new SqlCommand(spName, sqlcon);
                        cmd.Parameters.Add(new SqlParameter("@Charges", SqlDbType.VarChar)).Value = xml;
                        cmd.Parameters.Add(new SqlParameter("@prospectid", SqlDbType.VarChar)).Value = prospectid;
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sqldata1 = new SqlDataAdapter(cmd);
                        sqldata1.Fill(ds);
                        sqlcon.Close();
                    }
                }
            }
            if (Guardiandetail != null)
            {
                if (ContactID == 0)
                {
                    using (SqlConnection sqlcon = new SqlConnection(connectioncams))
                    {
                        using (SqlCommand cmd = new SqlCommand("CAMS_GetNextId", sqlcon))
                        {

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Value", SqlDbType.Int, 30);
                            cmd.Parameters["@Value"].Direction = ParameterDirection.Output;
                            sqlcon.Open();
                            cmd.ExecuteNonQuery();
                            sqlcon.Close();
                            ContactID = (int)cmd.Parameters["@Value"].Value;
                            Session["ContactID"] = ContactID;
                        }
                    }
                }
                using (SqlConnection sqlcon = new SqlConnection(connectioncams))
                {
                    sqlcon.Open();
                    string spName = "Custom_Guardian_EmployerInsert";
                    SqlCommand cmd = new SqlCommand(spName, sqlcon);
                    cmd.Parameters.Add(new SqlParameter("@prospectid", SqlDbType.VarChar)).Value = prospectid;
                    cmd.Parameters.Add(new SqlParameter("@firstaname", SqlDbType.VarChar)).Value = Guardiandetail[0].FirstName == null ? string.Empty : Guardiandetail[0].FirstName;
                    cmd.Parameters.Add(new SqlParameter("@lastname", SqlDbType.VarChar)).Value = Guardiandetail[0].LastName == null ? string.Empty : Guardiandetail[0].LastName;
                    cmd.Parameters.Add(new SqlParameter("@relationship", SqlDbType.VarChar)).Value = Guardiandetail[0].Relationship == null ? string.Empty : Guardiandetail[0].Relationship;
                    cmd.Parameters.Add(new SqlParameter("@phone", SqlDbType.VarChar)).Value = Guardiandetail[0].Phone == null ? string.Empty : Guardiandetail[0].Phone;
                    cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.VarChar)).Value = Guardiandetail[0].Gemail == null ? string.Empty : Guardiandetail[0].Gemail;
                    cmd.Parameters.Add(new SqlParameter("@contactid", SqlDbType.VarChar)).Value = ContactID;
                    cmd.Parameters.Add(new SqlParameter("@Employer", SqlDbType.VarChar)).Value = Guardiandetail[0].Company == null ? string.Empty : Guardiandetail[0].Company;
                    cmd.Parameters.Add(new SqlParameter("@Position", SqlDbType.VarChar)).Value = Guardiandetail[0].Position == null ? string.Empty : Guardiandetail[0].Position;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqldata1 = new SqlDataAdapter(cmd);
                    sqldata1.Fill(ds);
                    sqlcon.Close();
                }
            }
            var OptionalDetails = Cams_OptionalDetailsInsert(optional, Addressfields);
            var HighScool = InsertHighSchoolDetail(HighSchoolDetail, Collegedetail, TransferDtls);
            var ActiveCampaignPush = ActiveCampaignPost(optional, AcDetails, Guardiandetail);
            if (OptionalDetails == "Success" && HighScool == "Success")
            {
                //return RedirectToAction("CudPayment", "OnlinePayment");
                //This is done for Redirectaction wont go with ajax call.
                return Json(new { ok = true, newurl = Url.Action("CudPayment", "OnlinePayment") });
            }

            return View();
        }
        //[HttpPost]
        public string Cams_OptionalDetailsInsert(List<Optional> optional, List<HomeAddress> Addressfields)
        {
            string status = string.Empty;

            if (Session["ProspectID"] != null)
            {
                prospectid = Session["ProspectID"].ToString();
            }
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                try
                {
                    sqlcon.Open();
                    string spName = "Cams_OptionalvalueUpdate";
                    SqlCommand cmd = new SqlCommand(spName, sqlcon);
                    cmd.Parameters.Add(new SqlParameter("@prospectid", SqlDbType.VarChar)).Value = prospectid;
                    cmd.Parameters.Add(new SqlParameter("@needspecial", SqlDbType.VarChar)).Value = optional[0].NeededSpecial.Equals("Yes") ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@needVisa", SqlDbType.VarChar)).Value = optional[0].NeededVisa.Equals("Yes") ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@neededhousing", SqlDbType.VarChar)).Value = optional[0].NeededHousing.Equals("Yes") ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@neededspecialnotes", SqlDbType.VarChar)).Value = optional[0].NeededSpecialNotes == null ? string.Empty : optional[0].NeededSpecialNotes;
                    cmd.Parameters.Add(new SqlParameter("@TransferFromHigherInstitution", SqlDbType.VarChar)).Value = optional[0].TransferFromHigherInstitution.Equals("Yes") ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@TransferCredit", SqlDbType.VarChar)).Value = optional[0].TransferCredit.Equals("Yes") ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@neededtransportation", SqlDbType.VarChar)).Value = optional[0].NeededTransportation.Equals("Yes") ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@Housing", SqlDbType.VarChar)).Value = optional[0].NeededHousing.Equals("Yes") ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@Transfertocanada", SqlDbType.VarChar)).Value = optional[0].TransferToCanada.Equals("Yes") ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@USCitizen", SqlDbType.VarChar)).Value = optional[0].UsCitizen;
                    cmd.Parameters.Add(new SqlParameter("@UAEID", SqlDbType.VarChar)).Value = optional[0].UAEID == null ? string.Empty : optional[0].UAEID;
                    cmd.Parameters.Add(new SqlParameter("@UAEIDExpDate", SqlDbType.VarChar)).Value = optional[0].UAEIDExpDate == null ? string.Empty : optional[0].UAEIDExpDate.ToString();
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqldata1 = new SqlDataAdapter(cmd);
                    sqldata1.Fill(ds);



                    string spName1 = "Cams_HomeAddressInsert";
                    SqlCommand cmd1 = new SqlCommand(spName1, sqlcon);
                    cmd1.Parameters.Add(new SqlParameter("@prospectid", SqlDbType.VarChar)).Value = prospectid;
                    cmd1.Parameters.Add(new SqlParameter("@address1", SqlDbType.VarChar)).Value = Addressfields[0].Address1 == null ? string.Empty : Addressfields[0].Address1;
                    cmd1.Parameters.Add(new SqlParameter("@address2", SqlDbType.VarChar)).Value = Addressfields[0].Address2 == null ? string.Empty : Addressfields[0].Address2;
                    cmd1.Parameters.Add(new SqlParameter("@address3", SqlDbType.VarChar)).Value = Addressfields[0].Address3 == null ? string.Empty : Addressfields[0].Address3;
                    cmd1.Parameters.Add(new SqlParameter("@stateID", SqlDbType.VarChar)).Value = Addressfields[0].StateID == null ? string.Empty : Addressfields[0].StateID;
                    cmd1.Parameters.Add(new SqlParameter("@state", SqlDbType.VarChar)).Value = Addressfields[0].state == null ? string.Empty : Addressfields[0].state;
                    cmd1.Parameters.Add(new SqlParameter("@CountryID", SqlDbType.VarChar)).Value = Addressfields[0].CountryID == null ? string.Empty : Addressfields[0].CountryID;
                    cmd1.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqldata11 = new SqlDataAdapter(cmd1);
                    sqldata11.Fill(ds);
                    sqlcon.Close();
                    status = "Success";
                }
                catch (Exception Ex)
                {
                    status = "Failed";
                    Console.WriteLine(Ex.Message);
                }
            }
            return status;
        }

        //[HttpPost]
        // public ActionResult InsertHighSchoolDetail(List<HighSchoolDetails> HighSchoolDetail)
        public string InsertHighSchoolDetail(List<HighSchoolDetails> HighSchoolDetail, List<CollegeDetails> Collegdetail, List<TransferInstitutedtls> TransferDtls)
        {
            string status = string.Empty;
            if (Session["ProspectID"] != null)
            {
                prospectid = Session["ProspectID"].ToString();
            }
            else
            {

            }
            
            //prospectid = "46022";            

            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                try
                {
                    sqlcon.Open();
                    string spName = "Custom_CamsEducationIsert";
                    SqlCommand cmd = new SqlCommand(spName, sqlcon);
                    cmd.Parameters.Add(new SqlParameter("@prospectid", SqlDbType.VarChar)).Value = prospectid;
                    cmd.Parameters.Add(new SqlParameter("@highschool", SqlDbType.VarChar)).Value = HighSchoolDetail[0].HighSchoolID.ToString() == null ? string.Empty : HighSchoolDetail[0].HighSchoolID.ToString();
                    cmd.Parameters.Add(new SqlParameter("@hsScore", SqlDbType.VarChar)).Value = HighSchoolDetail[0].Score == null ? string.Empty : HighSchoolDetail[0].Score;
                    cmd.Parameters.Add(new SqlParameter("@hsScoreoutof", SqlDbType.VarChar)).Value = HighSchoolDetail[0].Scoreoutof == null ? string.Empty : HighSchoolDetail[0].Scoreoutof;
                    cmd.Parameters.Add(new SqlParameter("@hsType", SqlDbType.VarChar)).Value = HighSchoolDetail[0].Type == null ? string.Empty : HighSchoolDetail[0].Type;
                    cmd.Parameters.Add(new SqlParameter("@Customhighschool", SqlDbType.VarChar)).Value = HighSchoolDetail[0].Customhschool == null ? string.Empty : HighSchoolDetail[0].Customhschool;
                    cmd.Parameters.Add(new SqlParameter("@college", SqlDbType.VarChar)).Value = Collegdetail[0].CollegeID.ToString() == null ? string.Empty : Collegdetail[0].CollegeID.ToString();
                    cmd.Parameters.Add(new SqlParameter("@clScore", SqlDbType.VarChar)).Value = Collegdetail[0].Grade == null ? string.Empty : Collegdetail[0].Grade;
                    cmd.Parameters.Add(new SqlParameter("@clScoreoutof", SqlDbType.VarChar)).Value = Collegdetail[0].Gradeoutof == null ? string.Empty : Collegdetail[0].Gradeoutof;
                    cmd.Parameters.Add(new SqlParameter("@TransferInst", SqlDbType.VarChar)).Value = TransferDtls[0].TransferInstitute == null ? string.Empty : TransferDtls[0].TransferInstitute;
                    cmd.Parameters.Add(new SqlParameter("@Transfercountry", SqlDbType.VarChar)).Value = TransferDtls[0].Trcountry == null ? string.Empty : TransferDtls[0].Trcountry;
                    cmd.Parameters.Add(new SqlParameter("@Transferprogram", SqlDbType.VarChar)).Value = TransferDtls[0].TrProgram == null ? string.Empty : TransferDtls[0].TrProgram;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqldata1 = new SqlDataAdapter(cmd);
                    sqldata1.Fill(ds);
                    sqlcon.Close();
                    status = "Success";
                }
                catch (Exception Ex)
                {
                    status = "Failed";
                    Console.WriteLine(Ex.Message);
                }
            }

            return status;
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

        [HttpPost]
        public JsonResult Fileupload(HttpPostedFileBase uploadedFile)
        {
            bool flag = true;
            string responseMessage = string.Empty;
            string fname = string.Empty;
            byte[] bytes;
            if (Session["ProspectID"] != null)
            {
                prospectid = Session["ProspectID"].ToString();
            }
            else {
                return Json(new { success = false, responseMessage = "sessionexpr" }, JsonRequestBehavior.AllowGet);
            }
            //prospectid = "46021";
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase File = Request.Files[0];
                using (BinaryReader br = new BinaryReader(File.InputStream))
                {
                    bytes = br.ReadBytes(File.ContentLength);
                }
                string FileName = Path.GetFileName(File.FileName);
                string Extention = Path.GetExtension(FileName);
                string ContentType = File.ContentType;
                string Docid = Request.Form["Docid"];
                string DocName = Request.Form["DocName"];
                string[] DocNamearray = DocName.Split('/');
                string DocNameeng = DocNamearray[0];
                if (Extention.ToLower() == ".jpg" || Extention.ToLower() == ".jpeg" || Extention.ToLower() == ".doc" ||
                    Extention.ToLower() == ".docx" || Extention.ToLower() == ".pdf" || Extention.ToLower() == ".gif" || Extention.ToLower() == ".png")
                {
                    try
                    {
                        using (SqlConnection sqlcon = new SqlConnection(connectioncams))
                        {
                            sqlcon.Open();
                            string spName = "CustomCams_DocUpload";
                            SqlCommand cmd = new SqlCommand(spName, sqlcon);
                            cmd.Parameters.Add(new SqlParameter("@ProspectID", SqlDbType.VarChar)).Value = prospectid;
                            cmd.Parameters.Add(new SqlParameter("@DocName", SqlDbType.VarChar)).Value = DocNameeng;
                            cmd.Parameters.Add(new SqlParameter("@DocNameID", SqlDbType.VarChar)).Value = Docid;
                            cmd.Parameters.Add(new SqlParameter("@FileName", SqlDbType.VarChar)).Value = FileName;
                            cmd.Parameters.Add(new SqlParameter("@insertUser", SqlDbType.VarChar)).Value = "OnlineApp";
                            cmd.Parameters.Add(new SqlParameter("@Contenttype", SqlDbType.VarChar)).Value = ContentType;
                            cmd.Parameters.Add(new SqlParameter("@file", SqlDbType.Image)).Value = bytes;
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter sqldata1 = new SqlDataAdapter(cmd);
                            sqldata1.Fill(ds);
                            sqlcon.Close();
                        }
                        flag = true;
                        responseMessage = "Upload Successful.";
                        fname = FileName;
                    }
                    catch (Exception ex)
                    {
                        flag = false;
                        responseMessage = "Upload Failed with error: " + ex.Message;
                    }
                }
                else
                {
                    flag = false;
                    responseMessage = "File is invalid.";
                }
            }
            else
            {
                flag = false;
                responseMessage = "No File to upload.";
            }
            return Json(new { success = flag, responseMessage = responseMessage, Filename = fname }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteUploadedFile(string DocNameID)
        {
            string responseMessage = string.Empty;
            bool flag = true;
            if (Session["ProspectID"] != null)
            {
                prospectid = Session["ProspectID"].ToString();
            }
            if (DocNameID != "")
            {
                try
                {
                    using (SqlConnection sqlcon = new SqlConnection(connectioncams))
                    {
                        sqlcon.Open();
                        string spName = "Custom_RemoveDocuploaded";
                        SqlCommand cmd = new SqlCommand(spName, sqlcon);
                        cmd.Parameters.Add(new SqlParameter("@ProspectID", SqlDbType.VarChar)).Value = prospectid;
                        cmd.Parameters.Add(new SqlParameter("@DocNameID", SqlDbType.VarChar)).Value = DocNameID;
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sqldata1 = new SqlDataAdapter(cmd);
                        sqldata1.Fill(ds);
                        sqlcon.Close();
                    }
                    flag = true;
                    responseMessage = "Deleted Successfully.";
                }
                catch (Exception ex)
                {
                    flag = false;
                    responseMessage = "Delete Failed with error: " + ex.Message;
                }
            }
            return Json(new { success = flag, responseMessage = responseMessage }, JsonRequestBehavior.AllowGet);
        }

        private string ActiveCampaignPost(List<Optional> optional, ACEdupageDetails AceduDetails, List<Guardian> Guardiandetail)
        {
            var email = Session["EmailID"].ToString();
            //var Eloquaid = Session["EloquaID"].ToString();
            //var Eloquaid = "550";
            if (Session["ProspectID"] != null)
            {
                prospectid = Session["ProspectID"].ToString();
            }
            accountdata acdata = new accountdata();
            var token = ConfigurationManager.AppSettings["ActiveCampaignToken"];
            int curriculumnid = 0;
            if (AceduDetails.HighSchoolCurriculum == null) { curriculumnid = 0; } else { curriculumnid = Convert.ToInt16(AceduDetails.HighSchoolCurriculum); }
            

            List<ACCustomFields> customfields = new List<ACCustomFields>();
            customfields.Add(new ACCustomFields(){field = "53",value = optional[0].NeededTransportation});
            customfields.Add(new ACCustomFields() { field = "52", value = optional[0].NeededHousing });
            customfields.Add(new ACCustomFields() { field = "54", value = optional[0].NeededSpecial });
            customfields.Add(new ACCustomFields() { field = "55", value = optional[0].TransferToCanada });
            customfields.Add(new ACCustomFields() { field = "56", value = optional[0].UsCitizen });
            customfields.Add(new ACCustomFields() { field = "57", value = optional[0].NeededVisa });
            customfields.Add(new ACCustomFields() { field = "59", value = Guardiandetail[0].Company == null ? string.Empty : Guardiandetail[0].Company });
            customfields.Add(new ACCustomFields() { field = "60", value = Guardiandetail[0].Position == null ? string.Empty : Guardiandetail[0].Position });
            customfields.Add(new ACCustomFields() { field = "61", value = Guardiandetail[0].FirstName == null ? string.Empty : Guardiandetail[0].FirstName });
            customfields.Add(new ACCustomFields() { field = "62", value = Guardiandetail[0].Gemail == null ? string.Empty : Guardiandetail[0].Gemail });
            customfields.Add(new ACCustomFields() { field = "63", value = Guardiandetail[0].Phone == null ? string.Empty : Guardiandetail[0].Phone });
            customfields.Add(new ACCustomFields() { field = "29", value = AceduDetails.CollegeCountryName == null ? string.Empty : AceduDetails.CollegeCountryName });
            customfields.Add(new ACCustomFields() { field = "26", value = AceduDetails.CollegeName == null ? string.Empty : AceduDetails.CollegeName });
            customfields.Add(new ACCustomFields() { field = "27", value = AceduDetails.HighSchoolCountryName == null ? string.Empty : AceduDetails.HighSchoolCountryName });
            customfields.Add(new ACCustomFields() { field = "22", value = AceduDetails.HighschoolName == null ? string.Empty : AceduDetails.HighschoolName });
            customfields.Add(new ACCustomFields() { field = "24", value =  AC.GetFieldNameForOracle("Curriculum", AceduDetails.HighSchoolCurriculum).Trim() });
            customfields.Add(new ACCustomFields() { field = "15", value = "~Step 6 - Pending Payment" });
            customfields.Add(new ACCustomFields() { field = "23", value = AceduDetails.HighSchoolGrade == null ? string.Empty : AceduDetails.HighSchoolGrade });
            customfields.Add(new ACCustomFields() { field = "69", value = AceduDetails.TransferCollegeName == null ? string.Empty : AceduDetails.TransferCollegeName });
            customfields.Add(new ACCustomFields() { field = "81", value = AceduDetails.ResidenceCountryName == null ? string.Empty : AceduDetails.ResidenceCountryName });
            customfields.Add(new ACCustomFields() { field = "82", value = AceduDetails.ResdienceStateName == null ? string.Empty : AceduDetails.ResdienceStateName });

            Eloquacontactsecondary contactsec = new Eloquacontactsecondary
            {
                contact = new ACContactsecondaryeloqua
                {
                    Email = email,
                    ID = AC.RetrieveEloquaID(prospectid),
                    Transportation = optional[0].NeededTransportation,
                    NeededHousing = optional[0].NeededHousing,
                    NeededSpecial = optional[0].NeededSpecial,
                    TransferToCanada = optional[0].TransferToCanada,
                    UAE_resident = optional[0].UsCitizen,
                    NeededVisa = optional[0].NeededVisa,
                    Employercompany = Guardiandetail[0].Company == null ? string.Empty : Guardiandetail[0].Company,
                    Position = Guardiandetail[0].Position == null ? string.Empty : Guardiandetail[0].Position,
                    Guardiandetail_FirstName = Guardiandetail[0].FirstName == null ? string.Empty : Guardiandetail[0].FirstName,
                    Guardiandetail_email = Guardiandetail[0].Gemail == null ? string.Empty : Guardiandetail[0].Gemail,
                    Guardiandetail_Phone = Guardiandetail[0].Phone == null ? string.Empty : Guardiandetail[0].Phone,
                    CollegeCountryName = AceduDetails.CollegeCountryName == null ? string.Empty : AceduDetails.CollegeCountryName,
                    CollegeName = AceduDetails.CollegeName == null ? string.Empty : AceduDetails.CollegeName,
                    HighSchoolCountryName = AceduDetails.HighSchoolCountryName == null ? string.Empty : AceduDetails.HighSchoolCountryName,
                    HighschoolName = AceduDetails.HighschoolName == null ? string.Empty : AceduDetails.HighschoolName,
                    HSCurriculum = ActiveCampaign.HSCurriculum(curriculumnid),
                    HighSchoolGrade = AceduDetails.HighSchoolGrade == null ? string.Empty : AceduDetails.HighSchoolGrade,
                    University_you_currently_study_in = AceduDetails.TransferCollegeName == null ? string.Empty : AceduDetails.TransferCollegeName,
                    ResidenceCountryName = AceduDetails.ResidenceCountryName == null ? string.Empty : AceduDetails.ResidenceCountryName,
                    ResdienceStateName = AceduDetails.ResdienceStateName == null ? string.Empty : AceduDetails.ResdienceStateName,
                    Application_Status = "~Step 6 - Pending Payment"
                }
            };            

            ACViewModelsecondary contactview = new ACViewModelsecondary
            {
                contact = new ACContactsecondary
                {                    
                    email = email,                   
                    fieldValues = customfields
                },
            };
            string responseCode = string.Empty;
            if (email != null)
            {                
                try
                {              
                    string strContactListeloqua = JsonConvert.SerializeObject(contactsec).ToString();                    
                    string eloquaInsert = AC.HttpPost("https://cudint-fryxusjfwk20-fr.integration.ocp.oraclecloud.com:443/ic/api/integration/v1/flows/rest/SYNCHONLINEDATATOELOQUAUPDATE/1.0/student", strContactListeloqua, "second");
                    string eloquaInsertAC = AC.HttpPost("https://cudint-fryxusjfwk20-fr.integration.ocp.oraclecloud.com:443/ic/api/integration/v1/flows/rest/SYNCHONLINEDATATOACTIVECAMPAIGNU/1.0/student", strContactListeloqua, "first");
                }
                catch (Exception Ex)
                {
                    return $"{Ex.Message} - {Ex.InnerException}";
                }

            }
            return responseCode;
        }      

    }
}