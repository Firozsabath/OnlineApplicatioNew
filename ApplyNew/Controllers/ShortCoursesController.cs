using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplyNew.Models;

namespace ApplyNew.Controllers
{
    public class ShortCoursesController : Controller
    {
        // GET: ShortCourses
        string connectionstringex = System.Configuration.ConfigurationManager.AppSettings["Connectionstringex"];
        string connectioncams = System.Configuration.ConfigurationManager.AppSettings["ConnectionstringCams"];
        DataSet ds = new DataSet();
        Security sec = new Security();
        public ActionResult Index()
        {
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
            List<Country> Countrylistex = new List<Country>();
            Country Country = new Country();
            for (int i = 0; i < ds.Tables[3].Rows.Count; i++)
            {
                Country Cn = new Country();
                Cn.UniqueId = Convert.ToInt16(ds.Tables[3].Rows[i]["UniqueId"].ToString());
                Cn.DisplayText = ds.Tables[3].Rows[i]["DisplayText"].ToString();
                Countrylistex.Add(Cn);
            }
            ViewBag.CountryList = Countrylistex;
            List<GenderType> gender = new List<GenderType>() { new GenderType { Text = "Male", Value = "Male" }, new GenderType { Text = "Female", Value = "Female" } };
            ViewBag.GenderList = gender;

            List<GenderType> Level = new List<GenderType>() { new GenderType { Text = "Beginner", Value = "Beginner" }, new GenderType { Text = "Intermediate", Value = "Intermediate" }, new GenderType { Text = "Advanced", Value = "Advanced" } };
            ViewBag.LevelList = Level;

            return View();
        }

        [HttpPost]
        public ActionResult Index(CTGamedevelopment mdData)
        {
            using (SqlConnection conex = new SqlConnection(connectionstringex))
            {
                int returnValue = -1;
                conex.Open();
                string Query = "insert into CTGamedevelopment(EmailID,ConfirmEmailID,FirstName,LastName,Gender,Nationality,CountryofResidence,EmiratesorPassport,Enquiry,ExpertLevel,Phone,CreatedDate) values('" + mdData.EmailID+"','"+mdData.ConfirmEmailID+ "','" + mdData.FirstName + "','" + mdData.LastName + "','" + mdData.Gender + "','" + mdData.Nationality + "','" + mdData.CountryofResidence + "','" + mdData.EmiratesorPassport + "','" + mdData.Enquiry + "','"+mdData.ExpertLevel+"','"+mdData.Phone+"','"+DateTime.Now+"');SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(Query, conex);
                try
                {
                    object returnObj = cmd.ExecuteScalar();
                    PayReceipt pr = new PayReceipt();
                    if (returnObj != null)
                    {
                        int.TryParse(returnObj.ToString(), out returnValue);
                    }
                    Session["ApplicantID"] = returnValue;
                    sec.SendEmailCTCourses("enquiry", "Welcome to CUD", mdData, pr);
                    sec.SendEmailCTCourses("Admin", "New Enquiry Game Development Program", mdData, pr);
                    return RedirectToAction("CudPaymentCorporateTraning", "OnlinePayment");
                }
                catch(Exception ex)
                {
                    var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                    var sw = new System.IO.StreamWriter(filename, true);
                    sw.WriteLine(DateTime.Now.ToString() + " " + ex.Message + " " + ex.InnerException);
                    sw.Close();                    
                }
                finally
                {
                    conex.Close();
                }
                
            }
          return View();
        }
    }
}