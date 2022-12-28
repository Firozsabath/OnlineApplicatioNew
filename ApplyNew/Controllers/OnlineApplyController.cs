using ApplyNew.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ApplyNew.Controllers
{
    public class OnlineApplyController : Controller
    {
        string connectioncams = System.Configuration.ConfigurationManager.AppSettings["ConnectionstringCams"];
        DataSet ds = new DataSet();
        RemotePost Postdata = new RemotePost();        
        private CultureInfo cultureInfo;

        // GET: OnlineApply
        public ActionResult Index(string language)
        {
            if (!String.IsNullOrEmpty(language))
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
                Session["language"] = language;
            }
            string Source_type = string.Empty;
            string Lead_source = string.Empty;
            string Marketing_channel = string.Empty;
            string Communication_channel = string.Empty;
            string Campaig_name = string.Empty;
            string qs = Request.ServerVariables["QUERY_STRING"];

            if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_source_type"]))
            {
                Source_type = Request.QueryString["utm_source_type"].ToString();                
            }

            if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_lead_source"]))
            {
                Lead_source = Request.QueryString["utm_lead_source"].ToString();
            }

            if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_marketing_channel"]))
            {
                Marketing_channel = Request.QueryString["utm_marketing_channel"].ToString();
            }

            if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_communication_channel"]))
            {
                Communication_channel = Request.QueryString["utm_communication_channel"].ToString();
            }

            if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_campaign_name"]))
            {
                Campaig_name = Request.QueryString["utm_campaign_name"].ToString();
            }
            ViewBag.utms = qs;
            ACContactFIelds AC = new ACContactFIelds();
            AC.ScriptToWriteInCRM();
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection fm)
        {
            //System.Threading.Thread.Sleep(5000*2);
            bool Status = true;
            string Origin = string.Empty;
           string prospectid = string.Empty;
            string EmailID = string.Empty;
            string qs = fm["hdnqs"].ToString();

            //THis is a temporary fix until we find the root cause.
            if (fm["x_Origin"]!= null)
            {
                Origin = fm["x_Origin"].ToString();
                prospectid = fm["x_prospectid"].ToString();
                EmailID = fm["x_EmailID"].ToString();
            }
            else if (fm["Origin"] != null)
            {
                Origin = fm["Origin"].ToString();
                prospectid = fm["prospectid"].ToString();
                EmailID = fm["EmailID"].ToString();
            }
            else
            {
                prospectid = fm["prospectid"].ToString();
                EmailID = fm["EmailID"].ToString();
            }

            if (ProspectStatus(prospectid, EmailID) == "Success")
            {
                Session["ProspectID"] = prospectid;
                Session["EmailID"] = EmailID;
                FormsAuthentication.SetAuthCookie(prospectid, false);
                Status = true;
            }
            else
                Status = false;

            string URL = string.Empty;
            bool ApplicationSubmitted = Postdata.prospectApplicationstatus(prospectid);

            if (ApplicationSubmitted) { 
                Session["AppStatus"] = "Completed";
                URL = Url.Action("LeadComplete", "Lead");
                //URL = Url.Action("LeadUpdate", "Lead");
            }
            else {
                Session["AppStatus"] = "Progress";
                URL = Url.Action("LeadUpdate", "Lead") + "?" + qs + "";
            }
            if (Origin == "External")
            {
                if (ApplicationSubmitted)
                    return RedirectToAction("LeadComplete", "Lead");
                else
                    return RedirectToAction("LeadUpdate", "Lead");
            }

            return Json(new { Success = Status, newurl = URL }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProspectStatusCheck(string prospectid)
        {
            bool Status = true;
            Status = Postdata.prospectApplicationstatus(prospectid);
            return Json(new { Success = Status }, JsonRequestBehavior.AllowGet);
        }


        public string ProspectStatus(string prospectid,string emailid)
        {
            //bool Status = true;
            string Status = "";
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                sqlcon.Open();
                string ViewName = "SELECT * FROM  dbo.PreAdmission INNER JOIN dbo.Prospect_Address ON dbo.PreAdmission.ProspectID = dbo.Prospect_Address.ProspectID INNER JOIN dbo.Address ON dbo.Prospect_Address.AddressID = dbo.Address.AddressID where PreAdmission.ProspectID = '" + prospectid + "' and Email1 = '"+emailid+"' and Address.AddressTypeID = 287";
                SqlCommand cmd1 = new SqlCommand(ViewName, sqlcon);
                cmd1.CommandType = CommandType.Text;
                SqlDataAdapter sqldataforview = new SqlDataAdapter(cmd1);
                sqldataforview.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)                
                    Status = "Success";                
                else
                    Status = "";
                sqlcon.Close();
            }
            //return Json(new{ Success=Status}, JsonRequestBehavior.AllowGet) ;
            return Status;
        }
    }
}