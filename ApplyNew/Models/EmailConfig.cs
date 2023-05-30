using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using static System.Net.Mime.MediaTypeNames;

namespace ApplyNew.Models
{
    public class Emailcontent
    {
        public string Emailto { get; set; }
        public string Emailfrom { get; set; }
        public string[] Emailcc { get; set; }
        public string Emailbody { get; set; }
        public string Subject { get; set; }
        //public List<IFormFile> Attachments { get; set; }

    }
    public class EmailConfig
    {
        public bool SendEmail_new(string recipient, string subject, string body)
        {
            bool status = false;
            MailMessage mm = new MailMessage("Info@cud.ac.ae", recipient); // Message Body Initialisation.
            mm.Subject = subject; //Adding Subject To the Mail/
                                  // mm.Body = body;       // Adding the Message Body.
            mm.Body = body;       // Adding the Message Body.
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
                status = true;
            }
            catch (Exception ex)
            {
                var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                var sw = new System.IO.StreamWriter(filename, true);
                sw.WriteLine(DateTime.Now.ToString() + " " + ex.Message + " " + ex.InnerException);
                sw.Close();
                //throw;
            }
            return status;
            //return status;
        }

        public string CreatebodyforOTP(string OTP)
        {

            //string body = "OTP for prospect creation at Canadian University DUbai is "+ OTP + ". OTP valid for 10 minutes. Do not share your OTP with anyone.";            
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HostingEnvironment.MapPath("~/ProfilecreationOTP.html")))
            {
                body = reader.ReadToEnd();
            }
            //Phonenum = Session["Phone"].ToString();
            body = body.Replace("{OTPGENERATED}", OTP);

            return body;
        }

        public async Task<bool> SendEmail(string recipient, string subject, string Name, string Prospectid, string Password, string Approvalcode, string Amount)
        {
            var body = Createbody(Name, recipient, Prospectid, Password, Approvalcode, Amount);
            var emailApiUrl = ConfigurationManager.AppSettings["CUDConfigAPI_Url"];
            var requestContent = new Emailcontent
            {
                Emailbody = body,
                Emailto = recipient,
                Subject = subject
            };
            var requestBody = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8,"application/json");
            using (var httpc = new HttpClient())
            {
                var httpResult = await httpc.PostAsync(emailApiUrl + "SendEmailFromInfo", requestBody);
                if(httpResult.StatusCode == HttpStatusCode.OK)
                {                    
                    return true;
                }
            }
            return false;
        }

        private string Createbody(string Name, string email, string Prospectid, string Password, string Approvalcode, string Amount)
        {

            string body = "";
            if (Approvalcode == "")
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

    }
}