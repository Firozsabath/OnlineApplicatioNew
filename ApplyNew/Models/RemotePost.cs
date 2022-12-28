using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class RemotePost
    {
        private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();
        string connectionstringex = System.Configuration.ConfigurationManager.AppSettings["Connectionstringex"];
        string connectioncams = System.Configuration.ConfigurationManager.AppSettings["ConnectionstringCams"];

        public string Url = "";  
        public string Method = "post";  
        public string FormName = "form1";  
  
    public void Add(string name, string value) {  
        Inputs.Add(name, value);  
    }  
  
    public void Post() {  
        System.Web.HttpContext.Current.Response.Clear();  
  
        System.Web.HttpContext.Current.Response.Write("<html><head>");  
  
        System.Web.HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));  
        System.Web.HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));  
        for (int i = 0; i < Inputs.Keys.Count; i++) {  
            System.Web.HttpContext.Current.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));  
        }  
        System.Web.HttpContext.Current.Response.Write("</form>");  
        System.Web.HttpContext.Current.Response.Write("</body></html>");  
  
        System.Web.HttpContext.Current.Response.End();  
    }
        public bool prospectApplicationstatus(string prospectid)
        {
            bool status = false;
            DataSet ds1 = new DataSet();
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                sqlcon.Open();
                string sqry = "select * from PreApplication where ProspectId = '" + prospectid + "'";
                SqlCommand cmd1 = new SqlCommand(sqry, sqlcon);
                cmd1.CommandType = CommandType.Text;
                SqlDataAdapter sqldataforview = new SqlDataAdapter(cmd1);
                sqldataforview.Fill(ds1);
                if (ds1.Tables[0].Rows.Count > 0)
                    status = Convert.ToBoolean(ds1.Tables[0].Rows[0]["ApplicationSubmitted"]);
                else
                    status = false;
                sqlcon.Close();
            }
            return status;
        }       

        public string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }

        public string Decode(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return System.Text.Encoding.UTF8.GetString(encoded);
        }


    }
}