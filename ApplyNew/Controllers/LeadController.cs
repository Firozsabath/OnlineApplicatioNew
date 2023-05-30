using ApplyNew.Models;
using ApplyNew.SFservicesamp;
using ApplyNew.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc.Attributes;
using System.Web.Security;
using System.Threading;
using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace ApplyNew.Controllers
{
    public class LeadController : Controller
    {
        //********************************************

        //***********************************************
        string connectionstringex = System.Configuration.ConfigurationManager.AppSettings["Connectionstringex"];
        string connectioncams = System.Configuration.ConfigurationManager.AppSettings["ConnectionstringCams"];
        DataSet ds = new DataSet();
        int prospectid = 0;
        CAMS_ProfileProgressEntities Progress = new CAMS_ProfileProgressEntities();
        Security External = new Security();
        ActiveCampaignSF ActiveCampaign = new ActiveCampaignSF();
        ACContactFIelds AC = new ACContactFIelds();
        RemotePost Postdata = new RemotePost();
        EmailConfig emailConfig = new EmailConfig();
        GoogleCaptchaService gcs = new GoogleCaptchaService();
        // GET: Lead
        public ActionResult Index(string language)
        {
            //var res = AC.FormFields();
            // var ct = AC.ScriptformissingApplicationtoAC();
            Session["ProspectID"] = null;
            //Session["EmailID"] = null;
            Session["Camsinserted"] = true;
            Session["SFinserted"] = true;

            string qs = Request.ServerVariables["QUERY_STRING"];
            if (qs != null)
            {
                ViewBag.utms = qs;
                if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_source"]))
                {
                    ViewBag.Source_type = Request.QueryString["utm_source"].ToString();
                }

                if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_medium"]))
                {
                    ViewBag.medium = Request.QueryString["utm_medium"].ToString();
                }

                //if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_marketing_channel"]))
                //{
                //    ViewBag.Marketing_channel = Request.QueryString["utm_marketing_channel"].ToString();
                //}

                //if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_communication_channel"]))
                //{
                //    ViewBag.Communication_channel = Request.QueryString["utm_communication_channel"].ToString();
                //}

                if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_campaign"]))
                {
                    ViewBag.Campaign_name = Request.QueryString["utm_campaign"].ToString();
                }
            }

            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);

            DataSet dsview = new DataSet();
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                sqlcon.Open();
                string spName = "OnlineAppDropdownspull";
                SqlCommand cmd = new SqlCommand(spName, sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqldata1 = new SqlDataAdapter(cmd);
                sqldata1.Fill(ds);
                sqlcon.Close();
            }
            List<ProgramList> programlistex = new List<ProgramList>();
            ProgramList Program = new ProgramList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ProgramList pr = new ProgramList();
                pr.Programid = Convert.ToInt16(ds.Tables[0].Rows[i]["ProgramsID"]).ToString();
                pr.ProgramName = ds.Tables[0].Rows[i]["Programs"].ToString();
                programlistex.Add(pr);
            }
            Program.ProgramlistDB = programlistex;

            List<Agencies> Agenclistex = new List<Agencies>();
            Agencies Agency = new Agencies();
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                Agencies Ag = new Agencies();
                Ag.Elementno = Convert.ToInt16(ds.Tables[1].Rows[i]["UniqueId"].ToString());
                Ag.DisplayText = ds.Tables[1].Rows[i]["DisplayText"].ToString();
                Agenclistex.Add(Ag);
            }
            Agency.AgencyList = Agenclistex;

            List<Term> Termlistex = new List<Term>();
            Term Term = new Term();
            for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
            {
                Term Tm = new Term();
                Tm.TermCalendarID = Convert.ToInt16(ds.Tables[2].Rows[i]["TermCalendarID"].ToString());
                Tm.TextTerm = ds.Tables[2].Rows[i]["TextTerm"].ToString();
                Termlistex.Add(Tm);
            }
            Term.TermList = Termlistex;

            List<Country> Countrylistex = new List<Country>();
            Country Country = new Country();
            for (int i = 0; i < ds.Tables[3].Rows.Count; i++)
            {
                Country Cn = new Country();
                Cn.UniqueId = Convert.ToInt16(ds.Tables[3].Rows[i]["UniqueId"].ToString());
                Cn.DisplayText = ds.Tables[3].Rows[i]["DisplayText"].ToString();
                Countrylistex.Add(Cn);
            }
            Country.CountryList = Countrylistex;

            List<Custom_Profileprogress_Result> ProfileProgress = Progress.Custom_Profileprogress(prospectid.ToString()).ToList();
            accountdata data = new accountdata();

            if(Session["EmailID"] != null)
            {
                data.Email = Session["EmailID"].ToString();
                data.verifyEmail = Session["EmailID"].ToString();
            }

            var viewmodel = new LeadViewModel
            {
                Programex = Program,
                Agencylst = Agency,
                Termlst = Term,
                Countrylst = Country,
                ProfileProgress = ProfileProgress,
                Datalist = data
            };

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Index(LeadViewModel Data, FormCollection fm)
        {
            bool Status = false;
            bool Cstatus = true;
            string URL = string.Empty;
            //if (this.IsCaptchaValid("Captcha is not valid"))
            //{
            if (ModelState.IsValid)
            {
                string camsresult = CamsDataInsert(Data, fm, "Update");
                var ACresponse = OraclePost(Data, fm);
                //string camsresult = "success";
                string SaveType = fm["hdnSaveandExit"].ToString();
                if (camsresult == "success")
                {
                    Status = true;
                    ViewBag.Result = "success";
                    if (SaveType == "SaveandExit")
                        URL = Url.Action("SaveAndExit", "Lead");
                    else
                        URL = Url.Action("EducationDoc", "EducationandDocuments");
                }
                else
                    Status = false;
            }
            //}
            //else
            //{                
            //    Cstatus = false;
            //}
            return Json(new { Success = Status, Captcha = Cstatus, newurl = URL }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Verify()
        {
            List<Custom_Profileprogress_Result> ProfileProgress = Progress.Custom_Profileprogress(prospectid.ToString()).ToList();
            string qs = Request.ServerVariables["QUERY_STRING"];
            ViewBag.utms = qs;
            ViewBag.ProfilProgress = ProfileProgress;
            return View();
        }


        [Authorize]
        public ActionResult SaveAndExit(string language)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            if (Session["ProspectID"] != null)
            {
                ViewBag.Prospectid = Session["ProspectID"].ToString();
            }
            if (Session["EmailID"] != null)
            {
                ViewBag.EmailID = Session["EmailID"].ToString();
            }
            if (Session["AppStatus"] != null)
            {
                if (Session["AppStatus"].ToString() == "Completed")
                    return RedirectToAction("LeadComplete", "Lead");

            }
            return View();
        }
        [Authorize]
        public ActionResult LeadComplete(string language)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            return View();
        }

        //[Authorize]
        public ActionResult LeadUpdate(string language)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);

            if (!string.IsNullOrWhiteSpace(Request.QueryString["Incoming_source"]))
            {
                string Incoming_channel = Postdata.Decode(Request.QueryString["Incoming_source"].ToString());
                if (Incoming_channel == "EmailFromExisting")
                {
                    string prospectid = Postdata.Decode(Request.QueryString["v"].ToString());
                    string EmailID = Postdata.Decode(Request.QueryString["x"].ToString());
                    Session["ProspectID"] = prospectid;
                    Session["EmailID"] = EmailID;
                    FormsAuthentication.SetAuthCookie(prospectid, false);
                }

            }
            Session["Camsinserted"] = true;
            if (Session["ProspectID"] != null)
            {
                prospectid = Convert.ToInt32(Session["ProspectID"].ToString());
                ViewBag.hdnprospectid = prospectid;
            }
            else
            {
                return RedirectToAction("Index", "OnlineApply", new { language = "en-US" });
            }

            string qs = Request.ServerVariables["QUERY_STRING"];
            if (qs != null)
            {
                ViewBag.utms = qs;
                if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_source"]))
                {
                    ViewBag.Source_type = Request.QueryString["utm_source"].ToString();
                }

                if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_medium"]))
                {
                    ViewBag.medium = Request.QueryString["utm_medium"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_campaign"]))
                {
                    ViewBag.Campaign_name = Request.QueryString["utm_campaign"].ToString();
                }
            }

            DataSet dsview = new DataSet();
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                sqlcon.Open();
                string spName = "OnlineAppDropdownspull";
                SqlCommand cmd = new SqlCommand(spName, sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqldata1 = new SqlDataAdapter(cmd);
                sqldata1.Fill(ds);

                string ViewName = "OnlineApp_LeadInfo";
                SqlCommand cmd1 = new SqlCommand(ViewName, sqlcon);
                cmd1.Parameters.Add(new SqlParameter("@prospectID", SqlDbType.NVarChar)).Value = prospectid;
                cmd1.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqldataforview = new SqlDataAdapter(cmd1);
                sqldataforview.Fill(dsview);

                sqlcon.Close();
            }
            List<ProgramList> programlistex = new List<ProgramList>();
            ProgramList Program = new ProgramList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ProgramList pr = new ProgramList();
                pr.Programid = Convert.ToInt16(ds.Tables[0].Rows[i]["ProgramsID"]).ToString();
                pr.ProgramName = ds.Tables[0].Rows[i]["Programs"].ToString();
                programlistex.Add(pr);
            }
            Program.ProgramlistDB = programlistex;

            List<Agencies> Agenclistex = new List<Agencies>();
            Agencies Agency = new Agencies();
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                Agencies Ag = new Agencies();
                Ag.Elementno = Convert.ToInt16(ds.Tables[1].Rows[i]["UniqueId"].ToString());
                Ag.DisplayText = ds.Tables[1].Rows[i]["DisplayText"].ToString();
                Agenclistex.Add(Ag);
            }
            Agency.AgencyList = Agenclistex;

            List<Term> Termlistex = new List<Term>();
            Term Term = new Term();
            for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
            {
                Term Tm = new Term();
                Tm.TermCalendarID = Convert.ToInt16(ds.Tables[2].Rows[i]["TermCalendarID"].ToString());
                Tm.TextTerm = ds.Tables[2].Rows[i]["TextTerm"].ToString();
                Termlistex.Add(Tm);
            }
            Term.TermList = Termlistex;

            List<Country> Countrylistex = new List<Country>();
            Country Country = new Country();
            for (int i = 0; i < ds.Tables[3].Rows.Count; i++)
            {
                Country Cn = new Country();
                Cn.UniqueId = Convert.ToInt16(ds.Tables[3].Rows[i]["UniqueId"].ToString());
                Cn.DisplayText = ds.Tables[3].Rows[i]["DisplayText"].ToString();
                Countrylistex.Add(Cn);
            }
            Country.CountryList = Countrylistex;

            List<Custom_Profileprogress_Result> ProfileProgress = Progress.Custom_Profileprogress(prospectid.ToString()).ToList();
            // Custom_Profileprogress_Result ProfileProgress = Progress.Custom_Profileprogress(prospectId);
            accountdata data = new accountdata();
            if (dsview.Tables[0].Rows.Count > 0)
            {
                data.Firstname = dsview.Tables[0].Rows[0]["Firstname"].ToString();
                data.Lastname = dsview.Tables[0].Rows[0]["Lastname"].ToString();
                data.Middlename = dsview.Tables[0].Rows[0]["MiddleInitial"].ToString();
                data.Firstnamear = dsview.Tables[0].Rows[0]["Firstnamear"].ToString();
                data.Lastnamear = dsview.Tables[0].Rows[0]["Lastnamear"].ToString();
                data.Middlenamear = dsview.Tables[0].Rows[0]["Middlenamear"].ToString();
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
                data.AgentCode = dsview.Tables[0].Rows[0]["agentcode"].ToString();
            }

            var viewmodel = new LeadViewModel
            {
                Programex = Program,
                Agencylst = Agency,
                Termlst = Term,
                Countrylst = Country,
                ProfileProgress = ProfileProgress,
                Datalist = data
            };
            return View(viewmodel);
        }
        [HttpPost]
        public ActionResult LeadUpdate(LeadViewModel Data, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                string camsresult = CamsDataInsert(Data, fm, "Update");
                var ACresponse = OraclePost(Data, fm);
                string SaveType = fm["hdnSaveandExit"].ToString();
                if (camsresult == "success")
                {
                    ViewBag.Result = "success";
                    if (SaveType == "SaveandExit")
                        return RedirectToAction("SaveAndExit", "Lead");
                    else
                        return RedirectToAction("EducationDoc", "EducationandDocuments");
                }
                else
                    ViewBag.Result = "unsuccess";
            }
            return View();
        }

        public string CamsDataInsert(LeadViewModel Data, FormCollection fm, string Type)
        {
            if (Type == "Update")
            {
                if (Session["ProspectID"] != null)
                {
                    prospectid = Convert.ToInt32(Session["ProspectID"].ToString());
                }
            }
            string output = string.Empty;
            string Salutation = string.Empty;
            string sourcetext = fm["selectedsource"].ToString();
            string source = fm["FAgency"].ToString();
            string program = fm["selectedprogramID"].ToString();
            string Term = fm["ETerm"].ToString();
            string Gender = fm["hdngender"].ToString();
            string CountryID = fm["selectedcountry"].ToString();
            string GpaGroup = fm["ddlgpatype"].ToString();
            string transfercredits = fm["ddltransferstatus"].ToString();
            //string agentcode = fm["AgentCode"].ToString();
            string prospectType = string.Empty;

            if (Gender == "Male")
            {
                Salutation = "Mr";
            }
            else if (Gender == "Female")
            {
                Salutation = "Ms";
            }
            //prospectid = 0;
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                using (SqlCommand cmd = new SqlCommand("CAMS_GetNextId", sqlcon))
                {
                    if (prospectid == 0)
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Value", SqlDbType.Int, 30);
                        cmd.Parameters["@Value"].Direction = ParameterDirection.Output;
                        sqlcon.Open();
                        cmd.ExecuteNonQuery();
                        sqlcon.Close();
                        prospectid = (int)cmd.Parameters["@Value"].Value;
                        prospectType = "NewID";
                    }
                }

            }
            string password = Passwordformation(Data.Datalist.Firstname, Data.Datalist.phone1);
            if (Session["Camsinserted"].ToString() == "True")
            {
                try
                {
                    using (SqlConnection sqlcon = new SqlConnection(connectioncams))
                    {
                        sqlcon.Open();
                        string spName = "Camsprofilecreation";
                        SqlCommand cmd = new SqlCommand(spName, sqlcon);
                        cmd.Parameters.Add(new SqlParameter("@Firstname", SqlDbType.VarChar)).Value = Data.Datalist.Firstname;
                        cmd.Parameters.Add(new SqlParameter("@Lastname", SqlDbType.VarChar)).Value = Data.Datalist.Lastname;
                        cmd.Parameters.Add(new SqlParameter("@middlename", SqlDbType.VarChar)).Value = Data.Datalist.Middlename == null ? string.Empty : Data.Datalist.Middlename;
                        cmd.Parameters.Add(new SqlParameter("@Firstnamear", SqlDbType.NVarChar)).Value = Data.Datalist.Firstnamear == null ? string.Empty : Data.Datalist.Firstnamear;
                        cmd.Parameters.Add(new SqlParameter("@Lastnamear", SqlDbType.NVarChar)).Value = Data.Datalist.Lastnamear == null ? string.Empty : Data.Datalist.Lastnamear;
                        cmd.Parameters.Add(new SqlParameter("@middlenamear", SqlDbType.NVarChar)).Value = Data.Datalist.Middlenamear == null ? string.Empty : Data.Datalist.Middlenamear;
                        cmd.Parameters.Add(new SqlParameter("@salutation", SqlDbType.VarChar)).Value = Salutation;
                        cmd.Parameters.Add(new SqlParameter("@term", SqlDbType.VarChar)).Value = Term;
                        cmd.Parameters.Add(new SqlParameter("@source", SqlDbType.VarChar)).Value = sourcetext;
                        cmd.Parameters.Add(new SqlParameter("@program", SqlDbType.VarChar)).Value = program;
                        cmd.Parameters.Add(new SqlParameter("@eventid", SqlDbType.VarChar)).Value = source;
                        cmd.Parameters.Add(new SqlParameter("@phone1", SqlDbType.VarChar)).Value = "+" + fm["hdnPhone1"].ToString();
                        cmd.Parameters.Add(new SqlParameter("@phone2", SqlDbType.VarChar)).Value = Data.Datalist.phone2 == null ? string.Empty : Data.Datalist.phone2;
                        cmd.Parameters.Add(new SqlParameter("@email1", SqlDbType.VarChar)).Value = Data.Datalist.Email;
                        cmd.Parameters.Add(new SqlParameter("@prospectid", SqlDbType.Int)).Value = prospectid;
                        cmd.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar)).Value = password;
                        cmd.Parameters.Add(new SqlParameter("@sexcode", SqlDbType.VarChar)).Value = Gender;
                        cmd.Parameters.Add(new SqlParameter("@birthdate", SqlDbType.DateTime)).Value = Data.Datalist.Birdthdate.ToString();
                        cmd.Parameters.Add(new SqlParameter("@CountryID", SqlDbType.VarChar)).Value = CountryID;
                        cmd.Parameters.Add(new SqlParameter("@Gpagroupid", SqlDbType.VarChar)).Value = GpaGroup;
                        cmd.Parameters.Add(new SqlParameter("@Transfercredits", SqlDbType.VarChar)).Value = transfercredits.Equals("Yes") ? 1 : 0;
                        cmd.Parameters.Add(new SqlParameter("@agentCode", SqlDbType.VarChar)).Value = Data.Datalist.AgentCode == null ? string.Empty : Data.Datalist.AgentCode;
                        //optional[0].NeededSpecial.Equals("Yes") ? 1 : 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sqldata1 = new SqlDataAdapter(cmd);
                        sqldata1.Fill(ds);
                        sqlcon.Close();
                        output = "success";
                        Session["Camsinserted"] = false;
                        Session["ProspectID"] = prospectid;
                        Session["EmailID"] = Data.Datalist.Email;

                        if (prospectType == "NewID")
                        {
                            External.SendEmail(Data.Datalist.Email, "Welcome to CUD", Data.Datalist.Firstname, prospectid.ToString(), password, "", "");
                            FormsAuthentication.SetAuthCookie(prospectid.ToString(), false);

                            //var prospect_AgentsDetails = External.getLeadInfo(prospectid.ToString());
                            //if (!String.IsNullOrEmpty(prospect_AgentsDetails.AgentCode))
                            //{
                            //    string recepient = ConfigurationManager.AppSettings["Agent_RecepientEmail"];
                            //    string Subject = "Welcome to CUD";
                            //    var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                            //    var sw = new System.IO.StreamWriter(filename, true);
                            //    sw.WriteLine(DateTime.Now.ToString() + " " + recepient + " " + Subject + " ");                                

                            //    var emstatus = External.SendEmail(recepient, Subject, Data.Datalist.Firstname, prospectid.ToString(), password, "", "");

                            //    sw.WriteLine(DateTime.Now.ToString() + " " + recepient + " " + Subject+" " + emstatus);
                            //    sw.Close();
                            //}
                        }
                    }
                }
                catch (Exception ex)
                {
                    var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                    var sw = new System.IO.StreamWriter(filename, true);
                    sw.WriteLine(DateTime.Now.ToString() + " " + ex.Message + " " + ex.InnerException);
                    sw.Close();
                    output = ex.Message;
                }
            }
            return output;
        }

        public JsonResult GetProgramByTypeId(int Typeid)
        {
            List<ProgramList> progrList = new List<ProgramList>();
            SelectList Colleges;
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                DataSet dshs = new DataSet();
                string SqlQuery = "SELECT dbo.tb_MajorArabic.Major_English + ' / ' + Major_Arabic as Programs, dbo.Programs.ProgramsID,GPAGroupID FROM dbo.tb_MajorArabic INNER JOIN dbo.MajorMinor ON dbo.tb_MajorArabic.MajorMinorID = dbo.MajorMinor.MajorMinorID INNER JOIN dbo.Programs ON dbo.MajorMinor.MajorMinorName = dbo.Programs.Programs where GPAGroupID = '" + Typeid + "' and  dbo.Programs.ActiveFlag =1";
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

        public async Task<JsonResult> GenerateOTP(string emailID, string token)
        {
            var isValid = await gcs.VerifyToken(token);
            if (isValid)
            {
                if (!AC.isOtpLocked(emailID)) {
                    string returndata = string.Empty;
                    string result = string.Empty;
                    var emailIDExists = Checkprospectbyemail(emailID, "");
                    if (string.IsNullOrEmpty(emailIDExists))
                    {
                        var otp = AC.OTPGeneration();
                        var emailBody = emailConfig.CreatebodyforOTP(otp);
                        var emailSend = emailConfig.SendEmail_new(emailID, "OTP for Admissions", emailBody);
                        //var emailSend = emailConfig.SendEmail_new("firoz.sabath@cud.ac.ae", "OTP for Admissions", emailBody);
                        //var emailSend = true;
                        if (emailSend)
                        {
                            OnlineApp_Otp OtpData = new OnlineApp_Otp
                            {
                                Email = emailID,
                                OTP = Convert.ToInt32(otp),
                                SendDate = DateTime.Now,
                                Verified = false,
                            };

                            AC.ManipulateOtpLog(OtpData, "Insert");
                        }
                        return Json(new { result = true, prospectid = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { result = false, prospectid = emailIDExists }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { result = false, prospectid = "Otplimit" }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { result = false, prospectid = "Invalid" }, JsonRequestBehavior.AllowGet);
            }          

        }       

        public JsonResult AuthenticateOTP(string otp, string email, string token)
        {
            //var isValid = await gcs.VerifyToken(token);
            //if (isValid)
            //{
                int receivedOTP = Convert.ToInt32(otp);
                bool result = false;
                string erroMessage = string.Empty;
                string[] optDetails = AC.RetrieveOTP(otp, email);
                int generatedOTP = Convert.ToInt32(optDetails[0]);
                var genretedTime = Convert.ToDateTime(optDetails[1]);

                if (!String.IsNullOrEmpty(optDetails[0]))
                {
                    if (DateTime.UtcNow > genretedTime.AddMinutes(15))
                    {
                        result = true;
                        erroMessage = "OtpExpired";
                    }
                    else if (receivedOTP == generatedOTP)
                    {
                        result = true;
                        OnlineApp_Otp OtpData = new OnlineApp_Otp
                        {
                            Email = email,
                            OTP = Convert.ToInt32(otp),
                            VerifyDate = DateTime.Now
                        };
                        AC.ManipulateOtpLog(OtpData, "Update");
                        Session["EmailID"] = email;
                    }
                }
                else
                {
                    result = false;
                    erroMessage = "OtpMismatch";
                }
                return Json(new { result = result, errMsg = erroMessage }, JsonRequestBehavior.AllowGet);
           // }
            //else
            //{
            //    return Json(new { result = false, errMsg = "Invalid" }, JsonRequestBehavior.AllowGet);
            //}
        }

        //public JsonResult Checkprospectbyemail(string Emailid, string Termid)
        public string Checkprospectbyemail(string Emailid, string Termid)
        {
            string ProspectID = string.Empty;
            string Email = string.Empty;
            string FirstName = string.Empty;
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                DataSet dshs = new DataSet();
                //string SqlQuery = "SELECT top 1 dbo.PreAdmission.ProspectID, dbo.Address.Email1,PreAdmission.StatusID FROM dbo.Prospect_Address INNER JOIN dbo.Address ON dbo.Prospect_Address.AddressID = dbo.Address.AddressID INNER JOIN dbo.PreAdmission ON dbo.Prospect_Address.ProspectID = dbo.PreAdmission.ProspectID WHERE(dbo.Address.Email1 = '" + Emailid + "') and address.AddressTypeID = 287 and PreAdmission.AnticipatedEntryTermID = '" + Termid + "' order by PreAdmission.InsertTime desc";
                string SqlQuery = "SELECT top 1 dbo.PreAdmission.ProspectID, dbo.Address.Email1,PreAdmission.StatusID, dbo.PreAdmission.FirstName+' '+dbo.PreAdmission.LastName as FullName FROM dbo.Prospect_Address INNER JOIN dbo.Address ON dbo.Prospect_Address.AddressID = dbo.Address.AddressID INNER JOIN dbo.PreAdmission ON dbo.Prospect_Address.ProspectID = dbo.PreAdmission.ProspectID WHERE(dbo.Address.Email1 = '" + Emailid + "') and address.AddressTypeID = 287 order by PreAdmission.InsertTime desc";
                SqlCommand cmd = new SqlCommand(SqlQuery, sqlcon);
                SqlDataAdapter sqldata = new SqlDataAdapter(cmd);
                sqldata.Fill(dshs);
                for (int i = 0; i < dshs.Tables[0].Rows.Count; i++)
                {
                    ProspectID = dshs.Tables[0].Rows[0]["ProspectID"].ToString();
                    Email = dshs.Tables[0].Rows[0]["Email1"].ToString();
                    FirstName = dshs.Tables[0].Rows[0]["FullName"].ToString();
                }

                if (!String.IsNullOrEmpty(ProspectID))
                {
                    if (!Postdata.prospectApplicationstatus(ProspectID))
                    {
                        //string s = Server.UrlEncode("1254654");
                        string encryptEmail = Postdata.Encode(Email);
                        string encryptprospectid = Postdata.Encode(ProspectID);
                        string encryptIncomsource = Postdata.Encode("EmailFromExisting");
                        string Url = "https://cudportal.cud.ac.ae/apply/en-US/Lead/LeadUpdate?Incoming_source=" + encryptIncomsource + "&v=" + encryptprospectid + "&x=" + encryptEmail;

                        External.SendEmailRecovery(Email, "Welcome to CUD", FirstName, prospectid.ToString(), Url);
                    }
                    else
                    {
                        ProspectID = "";
                    }
                }

            }
            //return Json(ProspectID, JsonRequestBehavior.AllowGet);
            return ProspectID;
        }

        public string Passwordformation(string em, string ph)
        {
            string pwd = string.Empty;
            pwd = em + ph.Substring(ph.Length - 4);
            return pwd;
        }
        private string OraclePost(LeadViewModel data, FormCollection fm)
        {
            try
            {
                string Source_type = string.Empty;
                string Lead_source = string.Empty;
                string Marketing_channel = string.Empty;
                string Communication_channel = string.Empty;
                string Campaig_name = string.Empty;
                var mapprogram = AC.GetFieldNameForOracle("Program", fm["selectedprogramID"].ToString()).Trim();
                var mapTerm = AC.GetFieldNameForOracle("Term", fm["ETerm"].ToString()).Trim();
                var mapDGtype = AC.GetFieldNameForOracle("DGType", fm["ddlgpatype"].ToString()).Trim();
                var ProspectID = Session["ProspectID"].ToString();
                string qs = fm["hdnqs"].ToString();
                accountdata acdata = new accountdata();
                var token = ConfigurationManager.AppSettings["ActiveCampaignToken"];

                if (qs != null && qs != "")
                {
                    if (!string.IsNullOrWhiteSpace(fm["hdnSource_type"]))
                    {
                        Source_type = fm["hdnSource_type"].ToString();
                    }
                    else
                    {
                        Source_type = "Organic_channel";
                    }
                    if (!string.IsNullOrWhiteSpace(fm["hdnmedium"]))
                    {
                        Communication_channel = fm["hdnmedium"].ToString();
                    }
                    if (!string.IsNullOrWhiteSpace(fm["hdnCampaig_name"]))
                    {
                        Campaig_name = fm["hdnCampaig_name"].ToString();
                    }
                }
                else
                {
                    Source_type = "Organic_channel";
                }

                Eloquacontact Contacttoeloqua = new Eloquacontact
                {
                    contact = new ACContactsample
                    {

                        ID = AC.RetrieveEloquaID(ProspectID),
                        FirstName = data.Datalist.Firstname,
                        Email = data.Datalist.Email,
                        LastName = data.Datalist.Lastname,
                        Phone = fm["hdnPhone1"].ToString(),
                        Program = mapprogram,
                        Enrollement_Term = mapTerm,
                        Nationality = fm["selectedcountryname"].ToString(),
                        ProspectID = ProspectID,
                        Prospect_Date = DateTime.Now.ToString("MM/dd/yyyy"),
                        Transfer_from_another_institution = fm["ddltransferstatus"].ToString(),
                        Hear_about_us = fm["selectedsource"].ToString(),
                        Date_of_Birth = Convert.ToDateTime(data.Datalist.Birdthdate).ToString("MM/dd/yyyy"),
                        Gender = fm["hdngender"].ToString(),
                        I_am_applying_for = mapDGtype,
                        Application_Status = "~Step 3",
                        Utm_Campaign_Name = Campaig_name,
                        Utm_Communication_Channel = Communication_channel,
                        Utm_Source_Type = Source_type,
                        Online_Application = "Yes",
                        AgentOrganization = AC.RetrieveAgentName(data.Datalist.AgentCode)
                    },

                };

                string responseCode = string.Empty;
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        string sampleeloqua = JsonConvert.SerializeObject(Contacttoeloqua).ToString();
                        string eloquaInsert = AC.HttpPost("https://cudint-fryxusjfwk20-fr.integration.ocp.oraclecloud.com:443/ic/api/integration/v1/flows/rest/SYNCHONLINEDATATOELOQUA/2.0/student", sampleeloqua, "first");
                        string eloquaInsertAC = AC.HttpPost("https://cudint-fryxusjfwk20-fr.integration.ocp.oraclecloud.com/ic/api/integration/v1/flows/rest/SYNCHONLINEDATATOACTIVECAMPAIGN/1.0/student", sampleeloqua, "first");

                        if (!String.IsNullOrWhiteSpace(eloquaInsert) && eloquaInsert != "")
                        {
                            var eloquaResponse = JsonConvert.DeserializeObject<JObject>(eloquaInsert);
                            if (eloquaResponse["Eloqua ID"] != null)
                            {
                                string EloquaID = eloquaResponse["Eloqua ID"].ToString();
                                Session["EloquaID"] = EloquaID;
                                var EloquaIDStatus = AC.UpdateEloquaID_toCams(EloquaID, ProspectID);
                            }
                        }
                    }
                    return responseCode;
                }
                catch (Exception Ex)
                {
                    var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                    var sw = new System.IO.StreamWriter(filename, true);
                    sw.WriteLine(DateTime.Now.ToString() + " " + Ex.Message + " " + Ex.InnerException);
                    sw.Close();
                    return $"{Ex.Message} - {Ex.InnerException}";
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
        }

        //[HttpPost]

        //public ActionResult AbandonSession()
        //{
        //    FormsAuthentication.SignOut();
        //    Session.Abandon();  // it will clear the session at the end of request
        //}

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();  // it will clear the session at the end of request
            return RedirectToAction("index", "OnlineApply", new { language = "en-US" });
        }

    }
}