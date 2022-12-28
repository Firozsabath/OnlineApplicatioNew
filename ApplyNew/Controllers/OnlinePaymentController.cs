using ApplyNew.Models;
using ApplyNew.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace ApplyNew.Controllers
{
    public class OnlinePaymentController : Controller
    {
        // GET: OnlinePayment
        string prospectid = string.Empty;
        string OrderNumber = string.Empty;
        string connectionstringex = ConfigurationManager.AppSettings["Connectionstringex"];
        string connectioncams = ConfigurationManager.AppSettings["ConnectionstringCams"];
        CAMS_ProfileProgressEntities Progress = new CAMS_ProfileProgressEntities();
        ACContactFIelds AC = new ACContactFIelds();
        [Authorize]
        public ActionResult CudPayment(string language)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);

            if (Session["ProspectID"] != null)
            {
                prospectid = Session["ProspectID"].ToString();
                ViewBag.hdnprospectid = prospectid;
            }
            else
            {
                return RedirectToAction("index", "OnlineApply");
            }
            //prospectid = "46022";
            PaymentParameters paymentparas = new PaymentParameters();
            List<Custom_Profileprogress_Result> ProfileProgress = Progress.Custom_Profileprogress(prospectid.ToString()).ToList();
            SqlConnection con = new SqlConnection(connectioncams);
            string Qry = "select * from PreAdmission where ProspectID = '" + prospectid + "' select * from Address where AddressID in(select top 1 AddressID from Prospect_Address where ProspectID = '" + prospectid + "')";
            SqlCommand cmd = new SqlCommand(Qry, con);
            SqlDataAdapter cad = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            cad.Fill(ds);

            string profile_id = ConfigurationManager.AppSettings["profile_id"];
            string access_key = ConfigurationManager.AppSettings["access_key"];

            if (ds.Tables[0].Rows.Count > 0)
            {

                paymentparas.middlename = ds.Tables[0].Rows[0]["MiddleInitial"].ToString();
                if (ds.Tables[0].Rows[0]["FirstName"].ToString() != "")
                { paymentparas.lastname = ds.Tables[0].Rows[0]["LastName"].ToString(); }
                else { paymentparas.lastname = "NAME"; }

                if (ds.Tables[0].Rows[0]["FirstName"].ToString() != "")
                { paymentparas.Firstname = ds.Tables[0].Rows[0]["FirstName"].ToString(); }
                else
                { paymentparas.Firstname = "NOREAL"; }

            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                if (ds.Tables[1].Rows[0]["Email1"].ToString() != "")
                {
                    paymentparas.bill_to_email = ds.Tables[1].Rows[0]["Email1"].ToString();
                }
                else { paymentparas.bill_to_email = "null@cybersource.com"; }
                if (ds.Tables[1].Rows[0]["Address1"].ToString() != "")
                {
                    paymentparas.bill_to_address_line1 = ds.Tables[1].Rows[0]["Address1"].ToString();
                }
                else
                {
                    paymentparas.bill_to_address_line1 = "1295 Charleston Road";
                }
                if (ds.Tables[1].Rows[0]["City"].ToString() != "")
                {
                    paymentparas.bill_to_address_city = ds.Tables[1].Rows[0]["City"].ToString();
                }
                else { paymentparas.bill_to_address_city = "Dubai"; }

                paymentparas.bill_to_address_state = "Dubai";

                paymentparas.bill_to_address_country = "AE";

                if (ds.Tables[1].Rows[0]["ZipCode"].ToString() != "")
                {
                    paymentparas.bill_to_address_postal_code = ds.Tables[1].Rows[0]["ZipCode"].ToString();
                }
                paymentparas.bill_to_address_postal_code = "00000";
            }
            //OrderNumber = FnOrderNM(paymentparas.Firstname, paymentparas.lastname, paymentparas.middlename);
            //paymentparas.reference_number = "P" + prospectid + "-" + OrderNumber;
            //paymentparas.getUid = getUUID();
            paymentparas.locallang = language;
            paymentparas.getip = getip();
            paymentparas.getUTCDateTime = getUTCDateTime();
            paymentparas.getsessionid = getsessionid();
            paymentparas.ProfileID = profile_id;
            paymentparas.accesskey = access_key;
            paymentparas.ProfileProgress = ProfileProgress;

            ViewData["amount"] = string.Format("{0:#.00}", Convert.ToDecimal(getamount(prospectid)));

            return View(paymentparas);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Demo(FormCollection form)
        {
            string prospect = string.Empty;
            if (Session["ProspectID"] != null)
            {
                prospect = Session["ProspectID"].ToString();
            }
            else
            {
                prospect = form["hdnProspectID"].ToString();
            }
            if(String.IsNullOrWhiteSpace(prospect))
            {
                return RedirectToAction("index", "OnlineApply");
            }
            string RedirectUrl  = ConfigurationManager.AppSettings["RedirectUrl"];                      
            Security Security = new Security();
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            RemotePost Repost = new RemotePost();
            Repost.Url = RedirectUrl;
            OrderNumber = FnOrderNM(form["bill_to_forename"].ToString(), form["bill_to_surname"].ToString(), "");
            string ReferenceNum = "P" + prospect + "-" + OrderNumber;
            string transaction_uuid = getUUID();
            SqlConnection con = new SqlConnection(connectionstringex);
            string Qry = "Insert into StudentportalCheckoutLog(StudentID,Amount,merchantID,Checkouttime,Source,TermID)values('" + prospect + "','" + form["amount"].ToString() + "','" + ReferenceNum + "','" + DateTime.Now.ToString() + "','OnlineApp','')";
            SqlCommand cmd = new SqlCommand(Qry, con);
            SqlDataAdapter cad = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            cad.Fill(ds);

            bool ProspectStatus = Repost.prospectApplicationstatus(prospect);
            if (ProspectStatus)
                return RedirectToAction("Index", "CompletedProfile");

            foreach (var key in form.AllKeys)
            {
                parameters.Add(key, Request.Params[key]);
                Repost.Add(key, Request.Params[key]);
            }
            parameters.Add("transaction_uuid", transaction_uuid);
            parameters.Add("reference_number", ReferenceNum);
            string Securitykey = Security.sign(parameters);
            Repost.Add("transaction_uuid", transaction_uuid);
            Repost.Add("reference_number", ReferenceNum);
            Repost.Add("signature", Securitykey);
            Repost.Post();

            return null;
        }

        //[Authorize]
        //public ActionResult Checkout()
        //{
        //    if (Session["ProspectID"] != null)
        //    {
        //        prospectid = Session["ProspectID"].ToString();
        //    }
        //    Security Security = new Security();            
        //    List<Custom_Profileprogress_Result> ProfileProgress = Progress.Custom_Profileprogress(prospectid.ToString()).ToList();
        //    var viewmodel = new OnlinePaymentViewModel
        //    {
        //        Security = Security,
        //        ProfileProgress = ProfileProgress
        //    };
        //    return View(viewmodel);
        //}

        [HttpPost]
        public ActionResult CudReceipt(string language)
        {            
            string lang = string.Empty;
            if (Request.Form["req_locale"] != null)
            {
                lang = Request.Form["req_locale"].ToString();
            }

            if (language == "en-OP")
            {
                if (lang != null)
                {
                    language = lang;
                }
            }

            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            PayReceipt PaymentPara = new PayReceipt();
            Security Security = new Security();
            foreach (var key in Request.Form.AllKeys)
            {
                //Response.Write("<span>" + key + "</span><input type=\"text\" name=\"" + key + "\" size=\"50\" value=\"" + Request.Params[key] + "\" readonly=\"true\"/><br/>");
                parameters.Add(key, Request.Params[key]);
            }

            if (Request.Form["auth_response"] != null)
            {
                PaymentPara.response = Request.Form["auth_response"].ToString();
                PaymentPara.responsemessage = Request.Form["message"].ToString();
                PaymentPara.amount = Request.Form["req_amount"].ToString();
                PaymentPara.approvalcode = Request.Form["req_reference_number"].ToString();
                PaymentPara.email = Request.Form["req_bill_to_email"].ToString();
                PaymentPara.Name = Request.Form["req_bill_to_forename"].ToString();
                string[] array = PaymentPara.approvalcode.Split('-');
                PaymentPara.prospectid = array[0].Remove(0, 1);
            }
            PaymentPara.locallang = language;
            if (Request.Form["message"] != null)
            {
                PaymentPara.responsemessage = Request.Form["message"].ToString();
            }

            if (PaymentPara.response == "00")
            {
                try
                {
                    SqlConnection con = new SqlConnection(connectioncams);
                    using (con)
                    {
                        con.Open();
                        SqlCommand sql_cmnd = new SqlCommand("InsertOnlineAppFee", con);
                        sql_cmnd.CommandType = CommandType.StoredProcedure;
                        sql_cmnd.Parameters.AddWithValue("@prospectid", SqlDbType.NVarChar).Value = PaymentPara.prospectid;
                        sql_cmnd.Parameters.AddWithValue("@amount", SqlDbType.Money).Value = PaymentPara.amount;
                        sql_cmnd.Parameters.AddWithValue("@approvalcode", SqlDbType.NVarChar).Value = PaymentPara.approvalcode;
                        sql_cmnd.ExecuteNonQuery();
                        con.Close();
                        Session["Paidstatus"] = "Yes";
                    }
                    ActiveCampaignPost(PaymentPara.email,PaymentPara.prospectid);
                    Security.SendEmail(PaymentPara.email, "CUD receipt", PaymentPara.Name, PaymentPara.prospectid, "", PaymentPara.approvalcode, PaymentPara.amount);

                    //Below codes should be uncommented for email sending process to agents

                    //var prospect_AgentsDetails = Security.getLeadInfo(PaymentPara.prospectid);
                    //if(!String.IsNullOrEmpty(prospect_AgentsDetails.AgentCode))
                    //{
                    //    string recepient = ConfigurationManager.AppSettings["Agent_RecepientEmail"]; ;
                    //    string Subject = "Test Agent Prospect Email";
                    //    Security.SendEmailAgentProspect(recepient, Subject, prospect_AgentsDetails);
                    //}
                    
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
            //PaymentPara.response = "00";
            return View(PaymentPara);
        }

        public ActionResult CudPaymentCorporateTraning(string language)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            string applicantID = string.Empty;
            if (Session["ApplicantID"] != null)
            {
                applicantID = Session["ApplicantID"].ToString();
                ViewBag.hdnprospectid = applicantID;
            }
            PaymentParameters paymentparas = new PaymentParameters();
            SqlConnection con = new SqlConnection(connectionstringex);
            string Qry = "select * from CTGamedevelopment where ID = '" + applicantID + "'";
            SqlCommand cmd = new SqlCommand(Qry, con);
            SqlDataAdapter cad = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            cad.Fill(ds);

            string profile_id = ConfigurationManager.AppSettings["profile_id"];
            string access_key = ConfigurationManager.AppSettings["access_key"];
            string RedirectUrl_API = ConfigurationManager.AppSettings["RedirectUrl_API"];

            if (ds.Tables[0].Rows.Count > 0)
            {
                paymentparas.middlename = "";
                if (ds.Tables[0].Rows[0]["LastName"].ToString() != "")
                { paymentparas.lastname = ds.Tables[0].Rows[0]["LastName"].ToString(); }
                else { paymentparas.lastname = "NAME"; }

                if (ds.Tables[0].Rows[0]["FirstName"].ToString() != "")
                { paymentparas.Firstname = ds.Tables[0].Rows[0]["FirstName"].ToString(); }
                else
                { paymentparas.Firstname = "NOREAL"; }
                if (ds.Tables[0].Rows[0]["EmailID"].ToString() != "")
                {
                    paymentparas.bill_to_email = ds.Tables[0].Rows[0]["EmailID"].ToString();
                }
                paymentparas.bill_to_address_line1 = "1295 Charleston Road";
                paymentparas.bill_to_address_city = "Dubai";
                paymentparas.bill_to_address_state = "Dubai";
                paymentparas.bill_to_address_country = "AE";
                paymentparas.bill_to_address_postal_code = "00000";
            }
           
            paymentparas.locallang = language;
            paymentparas.getip = getip();
            paymentparas.getUTCDateTime = getUTCDateTime();
            paymentparas.getsessionid = getsessionid();
            //paymentparas.ProfileID = profile_id;
            //paymentparas.accesskey = access_key;
            //paymentparas.ProfileProgress = ProfileProgress;
            paymentparas.Redirect_url = RedirectUrl_API;
            string amt = ConfigurationManager.AppSettings["CTPaymentAmt"];
            ViewData["amount"] = string.Format("{0:#.00}", Convert.ToDecimal(amt));


            return View(paymentparas);
        }

        [HttpPost]
        //[Authorize]
        public ActionResult DemoCorporate(FormCollection form)
        {           
            string prospect = string.Empty;
            if (Session["ApplicantID"] != null)
            {
                prospect = Session["ApplicantID"].ToString();
            }
            else
            {
                prospect = form["ApplicantID"].ToString();
            }
            if (String.IsNullOrWhiteSpace(prospect))
            {
                return RedirectToAction("index", "OnlineApply");
            }
            string RedirectUrl = ConfigurationManager.AppSettings["CUD_PaymentAPI"];

            Security Security = new Security();
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            RemotePost Repost = new RemotePost();
            Repost.Url = RedirectUrl;
            OrderNumber = FnOrderNM(form["bill_to_forename"].ToString(), form["bill_to_surname"].ToString(), "");
            string ReferenceNum = "CT-GDP" + "-" + OrderNumber;         

           
            foreach (var key in form.AllKeys)
            {
                parameters.Add(key, Request.Params[key]);
                Repost.Add(key, Request.Params[key]);
            }          
            Repost.Add("reference_number", ReferenceNum);          
            Repost.Post();

            return null;
        }

        [HttpPost]
        //[AllowCrossSiteJson]
        public async Task<JsonResult> BinancePay(string amount, string fname, string lname, string prospect, string EmailID)
        {
            string binanceUrl = ConfigurationManager.AppSettings["CryptoAPIUrl"];
            OrderNumber = FnOrderNM(fname, lname, "");
            string ReferenceNum = "P" + prospect + OrderNumber;
            string converrate = ConfigurationManager.AppSettings["CryptoConversionRate"];
            string successUrl = ConfigurationManager.AppSettings["CryptoReturnUrl"];
            string failureUrl = ConfigurationManager.AppSettings["CryptoCancelUrl"];
            var amt = Convert.ToDecimal(amount) / Convert.ToDecimal(converrate);
            BinanceExternalData ExternalData = new BinanceExternalData
            {
                amount = Math.Round(amt, 2),
                referenceNumber = ReferenceNum,
                goodsName = "Prospect",
                goodsDetail = "Prospect Transaction",
                failureUrl = failureUrl,
                successUrl = successUrl
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(ExternalData), Encoding.UTF8, "application/json");
            BinanceResponse binRes = new BinanceResponse();
            using (var httpclient = new HttpClient())
            {
                using (var res = await httpclient.PostAsync(binanceUrl, content))
                {
                    string apiResponse = await res.Content.ReadAsStringAsync();
                    binRes = JsonConvert.DeserializeObject<BinanceResponse>(apiResponse);

                    if (res.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        using (SqlConnection con = new SqlConnection(connectionstringex))
                        {
                            string Qry = "Insert into StudentportalCheckoutLog(StudentID,Amount,merchantID,Checkouttime,Source,TermID)values('" + prospect + "','" + amount + "','" + ReferenceNum + "','" + DateTime.Now.ToString() + "','OnlineApp-Binance','')";
                            string Qry1 = "Insert into BinancePaymentTransaction(ProspectID,BinanceReference,Amount,CreatedDate,Source,CUDReference,EmailID)values('" + prospect + "','" + binRes.data.prepayId + "','" + amount + "','" + DateTime.Now.ToString() + "','OnlineApp-Binance','" + ReferenceNum + "','" + EmailID + "')";
                            con.Open();
                            SqlCommand cmd = new SqlCommand(Qry, con);
                            cmd.ExecuteNonQuery();
                            con.Close();
                            con.Open();
                            SqlCommand cmd1 = new SqlCommand(Qry1, con);
                            cmd1.ExecuteNonQuery();
                            con.Close();
                        }
                        return Json(new { success = true, redirectUrl = binRes.data.universalUrl }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { success = true, redirectUrl = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Binancesuccessrequest()
        {
            return View();
        }

        public ActionResult Binancecancelrequest()
        {
            return View();
        }

        public ActionResult CudReceiptCorporateTraining()
        {

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            PayReceipt PaymentPara = new PayReceipt();
            Security Security = new Security();
            CTGamedevelopment GDP = new CTGamedevelopment();
            foreach (var key in Request.Form.AllKeys)
            {
                //Response.Write("<span>" + key + "</span><input type=\"text\" name=\"" + key + "\" size=\"50\" value=\"" + Request.Params[key] + "\" readonly=\"true\"/><br/>");
                parameters.Add(key, Request.Params[key]);
            }

            if (Request.Form["auth_response"] != null)
            {
                PaymentPara.response = Request.Form["auth_response"].ToString();
                PaymentPara.responsemessage = Request.Form["message"].ToString();
                PaymentPara.amount = Request.Form["req_amount"].ToString();
                PaymentPara.approvalcode = Request.Form["req_reference_number"].ToString();
                PaymentPara.email = Request.Form["req_bill_to_email"].ToString();
                PaymentPara.Name = Request.Form["req_bill_to_forename"].ToString();
                string[] array = PaymentPara.approvalcode.Split('-');
                PaymentPara.prospectid = array[0].Remove(0, 1);
            }
            //PaymentPara.locallang = language;
            if (Request.Form["message"] != null)
            {
                PaymentPara.responsemessage = Request.Form["message"].ToString();
            }

            if (PaymentPara.response == "00")
            {
                try
                {                    
                    Security.SendEmailCTCourses("Receipt", "CUD Receipt", GDP, PaymentPara);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
            //PaymentPara.response = "00";
            return View(PaymentPara);
        }

        public string getamount(string prospectid)
        {
            SqlConnection con = new SqlConnection(connectioncams);
            string amount = string.Empty;
            string Qry = "select * from CAMSGradAppPortalConfig gp left join PreAdmission pr on gp.ProgramID = pr.ProgramID where pr.ProspectID = '" + prospectid + "'";
            SqlCommand cmd = new SqlCommand(Qry, con);
            SqlDataAdapter cad = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            cad.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                amount = ds.Tables[0].Rows[0]["AppFeeAmount"].ToString();
            }

            return amount;
        }

        public string FnOrderNM(string fname, string lname, string mname)
        {
            string Ordernumber = string.Empty;

            if (fname != "")
            {
                fname = fname.Substring(0, 1);
            }
            if (lname != "")
            {
                lname = lname.Substring(0, 1);
            }
            if (mname != "")
            {
                mname = mname.Substring(0, 1);
            }
            Ordernumber = fname + lname + DateTime.Now.ToString("MMddyyymmss");
            return Ordernumber;
        }

        public String getUUID()
        {
            return System.Guid.NewGuid().ToString();
        }

        public String getUTCDateTime()
        {
            DateTime time = DateTime.Now.ToUniversalTime();
            return time.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", new CultureInfo("en-US"));
        }
        public string getip()
        {
            string ipaddress;
            return ipaddress = Request.ServerVariables["REMOTE_ADDR"];
        }
        public string getsessionid()
        {

            HttpSessionState sessionValue = System.Web.HttpContext.Current.Session;
            return sessionValue.SessionID.ToString();
        }

        private string ActiveCampaignPost(string emailid,string prospectID)
        {
            var email = emailid;
            //var token = ConfigurationManager.AppSettings["ActiveCampaignToken"];
            //List<ACCustomFields> customfields = new List<ACCustomFields>();
            //customfields.Add(new ACCustomFields() { field = "75", value = "Yes" });
            //customfields.Add(new ACCustomFields() { field = "15", value = "Review / Paid" });
            //ACViewModelsecondary contactview = new ACViewModelsecondary
            //{
            //    contact = new ACContactsecondary
            //    {
            //        email = email,
            //        fieldValues = customfields
            //    },
            //};

            Eloquacontactsecondary contactsec = new Eloquacontactsecondary
            {
                contact = new ACContactsecondaryeloqua
                {
                    Email = emailid,
                    ID = AC.RetrieveEloquaID(prospectID),
                    AppFeePaid = "Yes",
                    Application_Status = "Review / Paid"
                }
            };           

            string responseCode = string.Empty;
            if (email != null)
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        string strContactListeloqua = JsonConvert.SerializeObject(contactsec).ToString();
                        string eloquaInsert = AC.HttpPost("https://cudint-fryxusjfwk20-fr.integration.ocp.oraclecloud.com:443/ic/api/integration/v1/flows/rest/SYNCHONLINEDATATOELOQUAUPDATE/1.0/student", strContactListeloqua, "second");
                        string eloquaInsertAC = AC.HttpPost("https://cudint-fryxusjfwk20-fr.integration.ocp.oraclecloud.com:443/ic/api/integration/v1/flows/rest/SYNCHONLINEDATATOACTIVECAMPAIGNU/1.0/student", strContactListeloqua, "first");
                        //httpClient.DefaultRequestHeaders.Add("Api-Token", token);
                        //string strContactList = JsonConvert.SerializeObject(contactview).ToString();
                        //StringContent content = new StringContent(strContactList, Encoding.UTF8, "application/json");
                        //HttpResponseMessage response = httpClient.PostAsync("https://cud939.api-us1.com/api/3/contact/sync", content).Result;
                        //if (response.IsSuccessStatusCode)
                        //{
                        //    responseCode = "Success";
                        //}
                    }
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