using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.Hosting;

namespace ApplyNew.Models
{
    public class Security
    {
        //Uncomment the below for live
      //  private const String SECRET_KEY1 = "737d54bcdfd24aab8f4d46b38d3bd5485b32c83c2a0247a1b8d505402526f03fcacb523300c647ad9e47c1df60360a13c24561bdfca248518059ba1378f14c744b0d95aac3de45e68351ce4e52b3d89405cbbf79d3154df2ab1613cf1964c416f2df83c393dd4c378b606f3af354698e0fcaf9e029d24facafeb54b310f4c16d";

        //Uncomment the below for UAT
        //private const String SECRET_KEY1 = "c7c4514ff32d419794336dde4ae2f13723860daecc5a4f529984fbc5080eb36b9dab605785c441dda164833792ae70cb551c48028bd24b07b70a45d44fb218c66d774dcce0d144b683dd1487dafe34d079b52b56be824d7fa9e2b3757b41742ca31f7920809e4ba0a918af92e58893aefa83ab45a68d41f388afe6aee4cba50f";

        private string SECRET_KEY1 =  ConfigurationManager.AppSettings["SecretKey"];

        public string sign(IDictionary<string, string> paramsArray)
        {
            string returnvalue = string.Empty;
            returnvalue = sign(buildDataToSign(paramsArray), SECRET_KEY1);            
            return returnvalue;
        }

        private static String sign(String data, String secretKey)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secretKey);

            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);
            byte[] messageBytes = encoding.GetBytes(data);
            return Convert.ToBase64String(hmacsha256.ComputeHash(messageBytes));
        }

        private static String buildDataToSign(IDictionary<string, string> paramsArray)
        {
            String[] signedFieldNames = paramsArray["signed_field_names"].Split(',');
            IList<string> dataToSign = new List<string>();

            foreach (String signedFieldName in signedFieldNames)
            {
                dataToSign.Add(signedFieldName + "=" + paramsArray[signedFieldName]);
            }

            return commaSeparate(dataToSign);
        }

        private static String commaSeparate(IList<string> dataToSign)
        {
            return String.Join(",", dataToSign);
        }

        public string SendEmail(string recipient, string subject, string Name, string Prospectid, string Password,string Approvalcode,string Amount)
        {
            string status = string.Empty;
            MailMessage mm = new MailMessage("Info@cud.ac.ae", recipient); // Message Body Initialisation.
            mm.Subject = subject; //Adding Subject To the Mail/
                                  // mm.Body = body;       // Adding the Message Body.
            mm.Body = Createbody(Name,recipient,Prospectid,Password,Approvalcode,Amount);       // Adding the Message Body.
            // mm.Attachments.Add(new Attachment(path)); //The Code is used to Attach the pdf to the mail.
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient(); // Initialising the SMTP
            smtp.Host = "cudsvsmt001.cud.ae";
            smtp.EnableSsl = false;
            NetworkCredential NetworkCred = new NetworkCredential();
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = NetworkCred;
            smtp.Port = 25;
            try
            {
                smtp.Send(mm); // Forwarding the Created mail to the recepient.
                status = "Success";
            }
            catch (Exception ex)
            {
                var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                var sw = new System.IO.StreamWriter(filename, true);
                sw.WriteLine(DateTime.Now.ToString() + " " + ex.Message + " " + ex.InnerException);
                sw.Close();
                throw;
            }

            return status;
            //return status;
        }

        public string SendEmailCTCourses(string Type, string subject,  CTGamedevelopment data = null, PayReceipt pr = null)
        {
            string status = string.Empty;
            //MailMessage mm = new MailMessage();
            //mm.From = new MailAddress("Info@cud.ac.a");
            string recepient = string.Empty;
            if (Type == "enquiry")
            {
                recepient = data.EmailID;
                
            }
            else if(Type == "Admin")
            {
                //recepient = "firoz.sabath@cud.ac.ae";
                recepient =  "hanane.ellaiti@cud.ac.ae";
               
            }
            else if(Type == "Receipt")
            {
                recepient = pr.email;
            }
            MailMessage mm = new MailMessage("Info@cud.ac.ae", recepient); // Message Body Initialisation.
            if(Type == "Admin") { mm.CC.Add("medhat.moustafa@cud.ac.ae"); };
            mm.Subject = subject; //Adding Subject To the Mail/
                                  // mm.Body = body;       // Adding the Message Body.
            mm.Body = CreatebodyCTCourses(Type,data,pr);       // Adding the Message Body.
            // mm.Attachments.Add(new Attachment(path)); //The Code is used to Attach the pdf to the mail.
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient(); // Initialising the SMTP
            smtp.Host = "cudsvsmt001.cud.ae";
            smtp.EnableSsl = false;
            NetworkCredential NetworkCred = new NetworkCredential();
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = NetworkCred;
            smtp.Port = 25;
            try
            {                           
                smtp.Send(mm); // Forwarding the Created mail to the recepient.               
                status = "Success";
            }
            catch (Exception ex)
            {
                var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                var sw = new System.IO.StreamWriter(filename, true);
                sw.WriteLine(DateTime.Now.ToString() + " " + ex.Message + " " + ex.InnerException);
                sw.Close();
                throw;
            }

            return status;
            //return status;
        }

        private string CreatebodyCTCourses(string Type, CTGamedevelopment data = null, PayReceipt pr = null)
        {

            string body = "";
            if (Type == "enquiry")
            {
                using (StreamReader reader = new StreamReader(HostingEnvironment.MapPath("~/EmailtempforGameDev.html")))
                {
                    body = reader.ReadToEnd();
                }
                //Phonenum = Session["Phone"].ToString();
                body = body.Replace("{Name}", data.FirstName);
               
            }
            else if (Type == "Admin")
            {
                using (StreamReader reader = new StreamReader(HostingEnvironment.MapPath("~/EmailtempforGameDev_admin.html")))
                {
                    body = reader.ReadToEnd();
                }
                //Phonenum = Session["Phone"].ToString();
                body = body.Replace("{Date}", DateTime.Now.ToString());
                body = body.Replace("{Name}", data.FirstName);
                body = body.Replace("{Email}", data.EmailID);
                body = body.Replace("{ExpLevel}", data.ExpertLevel);
                body = body.Replace("{Residence}", data.CountryofResidence);
                body = body.Replace("{EID}", data.EmiratesorPassport);
                body = body.Replace("{Enquiry}", data.Enquiry);
                body = body.Replace("{Phone}", data.Phone);
            }
            else if(Type == "Receipt")
            {
                using (StreamReader reader = new StreamReader(HostingEnvironment.MapPath("~/ReceiptTemp_CT.html")))
                {
                    body = reader.ReadToEnd();
                }
                //Phonenum = Session["Phone"].ToString();
                body = body.Replace("{Date}", DateTime.Now.ToString());
                body = body.Replace("{ID}", pr.approvalcode);               
                body = body.Replace("{Name}", pr.Name);
                body = body.Replace("{Email}", pr.email);
                body = body.Replace("{amount}", pr.amount);

            }

            return body;
        }


        private string Createbody(string Name,string email,string Prospectid,string Password, string Approvalcode, string Amount)
        {

            string body = "";
            if(Approvalcode == "")
            { 
            using (StreamReader reader = new StreamReader(HostingEnvironment.MapPath("~/EmailtempforSP.html")))
            {
                body = reader.ReadToEnd();
            }
                //Phonenum = Session["Phone"].ToString();
                body = body.Replace("{Name}", Name);
                body = body.Replace("{Email}", email);
                body = body.Replace("{PrID}", Prospectid);
                body = body.Replace("{Password}", Password);
            }
            else
            {
                using (StreamReader reader = new StreamReader(HostingEnvironment.MapPath("~/ReceiptTemp.html")))
                {
                    body = reader.ReadToEnd();
                }
                //Phonenum = Session["Phone"].ToString();
                body = body.Replace("{Date}", DateTime.Now.ToString());
                body = body.Replace("{ID}", Approvalcode);
                body = body.Replace("{PrID}", Prospectid);
                body = body.Replace("{Name}", Name);
                body = body.Replace("{Email}", email);                
                body = body.Replace("{amount}", Amount);
            }
                       
            return body;
        }

        public string SendEmailAgentProspect(string recipient, string subject, ProspectAgentDetails prospectDT)
        {
            string status = string.Empty;
            MailMessage mm = new MailMessage("Info@cud.ac.ae", recipient); // Message Body Initialisation.
            mm.Subject = subject; //Adding Subject To the Mail/
                                  // mm.Body = body;       // Adding the Message Body.
            mm.Body = Createbodyfor_agent_prospect(prospectDT);       // Adding the Message Body.
            // mm.Attachments.Add(new Attachment(path)); //The Code is used to Attach the pdf to the mail.
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient(); // Initialising the SMTP
            smtp.Host = "cudsvsmt001.cud.ae";
            smtp.EnableSsl = false;
            NetworkCredential NetworkCred = new NetworkCredential();
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = NetworkCred;
            smtp.Port = 25;
            try
            {
                smtp.Send(mm); // Forwarding the Created mail to the recepient.
                status = "Success";
            }
            catch (Exception ex)
            {
                var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                var sw = new System.IO.StreamWriter(filename, true);
                sw.WriteLine(DateTime.Now.ToString() + " " + ex.Message + " " + ex.InnerException);
                sw.Close();
                throw;
            }

            return status;
            //return status;
        }

        public ProspectAgentDetails getLeadInfo(string prospectid)
        {
            string connectioncams = ConfigurationManager.AppSettings["ConnectionstringCams"];
            ProspectAgentDetails Details = new ProspectAgentDetails();
            SqlConnection con = new SqlConnection(connectioncams);
            DataSet dsview = new DataSet();
            string ViewName = "OnlineApp_LeadInfo";
            SqlCommand cmd1 = new SqlCommand(ViewName, con);
            cmd1.Parameters.Add(new SqlParameter("@prospectID", SqlDbType.NVarChar)).Value = prospectid;
            cmd1.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sqldataforview = new SqlDataAdapter(cmd1);
            sqldataforview.Fill(dsview);
            if (dsview.Tables[0].Rows.Count > 0)
            {
                Details.AgentCode = dsview.Tables[0].Rows[0]["agentcode"].ToString();
                Details.PropsectID = prospectid;
                Details.Prospect_Email = dsview.Tables[0].Rows[0]["Email1"].ToString();
                Details.Prospect_FullName = dsview.Tables[0].Rows[0]["FullName"].ToString();
                Details.Prospect_Phone = dsview.Tables[0].Rows[0]["phone1"].ToString();
                //Details.Prospect_program = dsview.Tables[0].Rows[0]["AppFeeAmount"].ToString();
                Details.Prospect_Term = dsview.Tables[0].Rows[0]["ExpectedTerm"].ToString();
            }


            return Details;
        }

        private string Createbodyfor_agent_prospect(ProspectAgentDetails prospectDT)
        {
            string body = "";
            
            using (StreamReader reader = new StreamReader(HostingEnvironment.MapPath("~/Emailtempfor_Agent.html")))
            {
                body = reader.ReadToEnd();
            }
            //Phonenum = Session["Phone"].ToString();
            body = body.Replace("{Name}", prospectDT.Prospect_FullName);
            body = body.Replace("{Email}", prospectDT.Prospect_Email);
            body = body.Replace("{PrID}", prospectDT.PropsectID);
            body = body.Replace("{Term}", prospectDT.Prospect_Term);
            body = body.Replace("{Phone}", prospectDT.Prospect_Phone);           

            return body;
        }

        public string SendEmailRecovery(string recipient, string subject, string Name, string prospectID, string Url)
        {
            string status = string.Empty;
            MailMessage mm = new MailMessage("Info@cud.ac.ae", recipient); // Message Body Initialisation.
            mm.Subject = subject; //Adding Subject To the Mail/
                                  // mm.Body = body;       // Adding the Message Body.
            mm.Body = Createbodyfor_SendEmailRecovery(prospectID, Name, Url);       // Adding the Message Body.
            // mm.Attachments.Add(new Attachment(path)); //The Code is used to Attach the pdf to the mail.
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient(); // Initialising the SMTP
            smtp.Host = "cudsvsmt001.cud.ae";
            smtp.EnableSsl = false;
            NetworkCredential NetworkCred = new NetworkCredential();
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = NetworkCred;
            smtp.Port = 25;
            try
            {
                smtp.Send(mm); // Forwarding the Created mail to the recepient.
                status = "Success";
            }
            catch (Exception ex)
            {
                var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                var sw = new System.IO.StreamWriter(filename, true);
                sw.WriteLine(DateTime.Now.ToString() + " " + ex.Message + " " + ex.InnerException);
                sw.Close();
                throw;
            }

            return status;
            //return status;
        }

        private string Createbodyfor_SendEmailRecovery(string prospectID, string Name, string Url)
        {
            string body = "";

            using (StreamReader reader = new StreamReader(HostingEnvironment.MapPath("~/Emailtempfor_prospectRecovery.html")))
            {
                body = reader.ReadToEnd();
            }
            
            body = body.Replace("{Name}", Name);
            body = body.Replace("{prospectID}", prospectID);
            body = body.Replace("{Url}", Url);
            return body;
        }

    }
}