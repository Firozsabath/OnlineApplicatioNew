using ApplyNew.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ApplyNew.Models
{
    public class ACContactFIelds
    {
        string connectionstringex = ConfigurationManager.AppSettings["Connectionstringex"];
        string connectioncams = ConfigurationManager.AppSettings["ConnectionstringCams"];

        public string FormFields(string email)
        {
            var token = ConfigurationManager.AppSettings["ActiveCampaignToken"];           
            string responseCode = string.Empty;

            // var email = "firoz.sabath@cud.ac.ae";
            string contactid = string.Empty;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Api-Token", token);

                    var response = httpClient.GetAsync("https://cud939.api-us1.com/api/3/contacts?email=" + email).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content;
                        // return responseContent.ReadAsStringAsync().GetAwaiter().GetResult();
                        string apiResponse = responseContent.ReadAsStringAsync().GetAwaiter().GetResult();
                        var EmailExists = JsonConvert.DeserializeObject<JObject>(apiResponse);
                        if (EmailExists["contacts"] != null)
                        {
                            if (EmailExists["contacts"].Count() > 0)
                            {
                                var contact = EmailExists["contacts"];
                                contactid = contact.First["id"].ToString();
                            }
                        }
                        else { contactid = null; }
                        return contactid;
                    }

                }
            }
            catch (Exception ex)
            {
                var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                var sw = new System.IO.StreamWriter(filename, true);
                sw.WriteLine(DateTime.Now.ToString() + " " + ex.Message + " " + ex.InnerException);
                sw.Close();
                Console.WriteLine($"{ex.Message}-{ex.InnerException}");
                throw;
            }
            return contactid;
        }

        public string FormFields123(string Newvalue, string fieldId, string Email)
        {
            var token = ConfigurationManager.AppSettings["ActiveCampaignToken"];
            var contactid = FormFields(Email);
            string Values = string.Empty;
            List<string> commas = new List<string>();
            if (contactid == null || contactid == "")
            {
                if (fieldId == "101")
                {
                    if (Newvalue == "")
                    {
                        Values = "Online Application";
                    }
                    else
                        Values = "Online Application," + Newvalue;
                }
                else
                {
                    Values = Newvalue;
                }

                return Values;
            }
            string responseCode = string.Empty;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Api-Token", token);

                    var response = httpClient.GetAsync("https://cud939.api-us1.com/api/3/contacts/" + contactid).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content;
                        string apiResponsecontact = responseContent.ReadAsStringAsync().GetAwaiter().GetResult();
                        var Contctdetailsss = JsonConvert.DeserializeObject<JObject>(apiResponsecontact);

                        foreach (var items in Contctdetailsss)
                        {
                            if (items.Key == "fieldValues")
                            {
                                var Fields = items.Value;
                                var fieldValue = Fields.ToObject<IList<ACJsonFieldvalues>>();
                                var fval = fieldValue.Where(x => x.field == fieldId);

                                if (fval.FirstOrDefault() != null)
                                {
                                    commas = fval.FirstOrDefault().value.Split(',').ToList();

                                    if (Newvalue != "" && Newvalue != null)
                                        commas.Add(Newvalue);

                                    var Inputfield = commas.Distinct();

                                    Values = string.Join(",", Inputfield);
                                }
                                else
                                {
                                    if (fieldId == "101")
                                    {
                                        if (Newvalue == "")
                                        {
                                            Values = "Online Application";
                                        }
                                        else
                                            Values = "Online Application," + Newvalue;
                                    }
                                    else
                                    {
                                        Values = Newvalue;
                                    }

                                }
                                return Values;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                var sw = new System.IO.StreamWriter(filename, true);
                sw.WriteLine(DateTime.Now.ToString() + " " + ex.Message + " " + ex.InnerException);
                sw.Close();
                Console.WriteLine($"{ex.Message}-{ex.InnerException}");
                throw;
            }
            return "";
        }


        public string HttpPost(string URI, string Parameters, string type)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            req.ContentType = "application/json; charset=utf-8";
            if (type == "first") { req.Method = "POST"; } else { req.Method = "PUT"; }            
            req.Timeout = 600000;
            req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("rajasekhar.mandala@aspiresys.com:Password99669"));
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Parameters);
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);
            os.Close();
            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null)
                return null;
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        public bool UpdateEloquaID_toCams(string Eloquaid, string prospectid)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                sqlcon.Open();
                string sqry = "update PreApplication_CUDCustom set EloquaApplicationID  = '" + Eloquaid + "' where ProspectID  = '" + prospectid + "'";
                SqlCommand cmd1 = new SqlCommand(sqry, sqlcon);
                try
                {
                    cmd1.ExecuteNonQuery();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    sqlcon.Close();
                }
            }
        }
        public string RetrieveEloquaID(string prospectid)
        {
            string ID = string.Empty;
            using (SqlConnection sqlcon = new SqlConnection(connectioncams))
            {
                sqlcon.Open();
                string sqry = "select * from PreApplication_CUDCustom where ProspectId = '" + prospectid + "'";
                SqlCommand cmd1 = new SqlCommand(sqry, sqlcon);
                SqlDataAdapter cad = new SqlDataAdapter(cmd1);
                DataSet ds = new DataSet();
                cad.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ID = ds.Tables[0].Rows[0]["EloquaApplicationID"].ToString();
                }
                return ID;
            }
        }

        public string RetrieveAgentName(string Agentcode)
        {
            string AgentName = string.Empty;
            if(!String.IsNullOrEmpty(Agentcode))
            {
                using (SqlConnection sqlcon = new SqlConnection(connectionstringex))
                {
                    sqlcon.Open();
                    string sqry = "select * from OnlineAgentLookup where AgentCode = '" + Agentcode + "'";
                    SqlCommand cmd1 = new SqlCommand(sqry, sqlcon);
                    SqlDataAdapter cad = new SqlDataAdapter(cmd1);
                    DataSet ds = new DataSet();
                    cad.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        AgentName = ds.Tables[0].Rows[0]["AgentName"].ToString();
                    }                   
                }
            }
            return AgentName;
        }

        public string GetFieldNameForOracle(string fieldType, string id)
        {
            string fieldName = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                using (SqlConnection sqlcon = new SqlConnection(connectionstringex))
                {
                    sqlcon.Open();
                    string sqry = "select * from Tbl_OracleFieldMap where Type = '"+fieldType+"' and DescriptionID = '"+id+"'";
                    SqlCommand cmd1 = new SqlCommand(sqry, sqlcon);
                    SqlDataAdapter cad = new SqlDataAdapter(cmd1);
                    DataSet ds = new DataSet();
                    cad.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        fieldName = ds.Tables[0].Rows[0]["Description"].ToString();
                    }
                }
            }           

            return fieldName;
        }

        public bool ScriptToWriteInCRM()
        {
            string connectioncams = ConfigurationManager.AppSettings["ConnectionstringCams"];
            DataSet ds = new DataSet();
            string responseCode = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(connectioncams))
                {
                    con.Open();
                    string SPName = "SistoOracleMissingLeads";
                    SqlCommand cmd = new SqlCommand(SPName, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqldata = new SqlDataAdapter(cmd);
                    sqldata.Fill(ds);
                    Eloquacontact Contacttoeloqua = new Eloquacontact();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            Contacttoeloqua.contact = new ACContactsample
                            {

                                ID = ds.Tables[0].Rows[i]["EloquaApplicationID"].ToString(),
                                FirstName = ds.Tables[0].Rows[i]["FirstName"].ToString(),
                                Email = ds.Tables[0].Rows[i]["Email1"].ToString(),
                                LastName = ds.Tables[0].Rows[i]["LastName"].ToString(),
                                Phone = ds.Tables[0].Rows[i]["Phone1"].ToString(),
                                Program = GetFieldNameForOracle("Program", ds.Tables[0].Rows[i]["ProgramID"].ToString()).Trim(),
                                Enrollement_Term = GetFieldNameForOracle("Term", ds.Tables[0].Rows[i]["AnticipatedEntryTermID"].ToString()).Trim(),
                                Nationality = ds.Tables[0].Rows[i]["country"].ToString(),
                                ProspectID = ds.Tables[0].Rows[i]["ProspectID"].ToString(),
                                Prospect_Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["UpdateTime"]).ToString("MM/dd/yyyy"),
                                Transfer_from_another_institution = ds.Tables[0].Rows[i]["Transfercredits"].ToString(),
                                Hear_about_us = ds.Tables[0].Rows[i]["SourceOfLead"].ToString(),
                                Date_of_Birth = Convert.ToDateTime(ds.Tables[0].Rows[i]["BirthDate"]).ToString("MM/dd/yyyy"),
                                Gender = ds.Tables[0].Rows[i]["SexCode"].ToString(),
                                I_am_applying_for = GetFieldNameForOracle("DGType", ds.Tables[0].Rows[i]["GPAGroupID"].ToString()).Trim(),
                                Application_Status = ds.Tables[0].Rows[i]["Appstatus"].ToString(),
                                Utm_Campaign_Name = "",
                                Utm_Communication_Channel = "",
                                Utm_Source_Type = "",
                                Online_Application = "Yes",
                                AgentOrganization = RetrieveAgentName(ds.Tables[0].Rows[i]["agentcode"].ToString())
                            };


                            try
                            {
                                using (var httpClient = new HttpClient())
                                {
                                    string sampleeloqua = JsonConvert.SerializeObject(Contacttoeloqua).ToString();
                                    //string eloquaInsert = AC.HttpPost("https://cudint-fryxusjfwk20-fr.integration.ocp.oraclecloud.com:443/ic/api/integration/v1/flows/rest/SYNCHONLINEDATATOELOQUA/2.0/student", sampleeloqua, "first");
                                    string eloquaInsertAC = HttpPost("https://cudint-fryxusjfwk20-fr.integration.ocp.oraclecloud.com/ic/api/integration/v1/flows/rest/SYNCHONLINEDATATOACTIVECAMPAIGN/1.0/student", sampleeloqua, "first");

                                    //if (!String.IsNullOrWhiteSpace(eloquaInsert) && eloquaInsert != "")
                                    //{
                                    //    var eloquaResponse = JsonConvert.DeserializeObject<JObject>(eloquaInsert);
                                    //    if (eloquaResponse["Eloqua ID"] != null)
                                    //    {
                                    //        string EloquaID = eloquaResponse["Eloqusa ID"].ToString();
                                    //        Session["EloquaID"] = EloquaID;
                                    //        var EloquaIDStatus = AC.UpdateEloquaID_toCams(EloquaID, ProspectID);
                                    //    }
                                    //}
                                }
                                responseCode = "success";
                            }
                            catch (Exception Ex)
                            {
                                var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                                var sw = new System.IO.StreamWriter(filename, true);
                                sw.WriteLine(DateTime.Now.ToString() + " " + Ex.Message + " " + Ex.InnerException);
                                sw.Close();
                                return false;
                                throw;
                            }

                        }
                    }
                }
            }
            catch (Exception)
            {               
                return false;
                throw;
            }

            return true;
        }

        public string OTPGeneration()
        {
            Random rnd = new Random();
            var str = (rnd.Next(10000,99999)).ToString();
            return str;
        }

        public string[] RetrieveOTP(string otp, string email)
        {
            string sqlQry = string.Empty;
            string[] generatedOtp = new string[2];
            using (SqlConnection con = new SqlConnection(connectionstringex))
            {
                con.Open();               
                DataSet ds = new DataSet();
                sqlQry = "select * from OnlineApp_Otp where  Email = '" +email + "' and OTP = '" + otp + "'";
                SqlCommand sqlcmd = new SqlCommand(sqlQry,con);
                sqlcmd.CommandType = CommandType.Text;
                SqlDataAdapter sqladapeter = new SqlDataAdapter(sqlcmd);
                sqladapeter.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    generatedOtp[0] = ds.Tables[0].Rows[0]["OTP"].ToString();
                    generatedOtp[1] = ds.Tables[0].Rows[0]["SendDate"].ToString();
                }
            }
            return generatedOtp;
        }

        public bool isOtpLocked(string email)
        {
            bool islocked = false;

            using (SqlConnection sqlcon = new SqlConnection(connectionstringex))
            {
                DataSet dshs = new DataSet();
                string SqlQuery = "select COUNT(*) as otpcount from OnlineApp_Otp where SendDate >= DATEADD(MI,-15,GETDATE()) and Email = '" + email + "' ";
                SqlCommand cmd = new SqlCommand(SqlQuery, sqlcon);
                SqlDataAdapter sqldata = new SqlDataAdapter(cmd);
                sqldata.Fill(dshs);
                for (int i = 0; i < dshs.Tables[0].Rows.Count; i++)
                {
                    var recordCount = Convert.ToInt16(dshs.Tables[0].Rows[i]["otpcount"].ToString());

                    if (recordCount > 3) return islocked = true;
                }

            }

            return islocked;

        }


        public bool ManipulateOtpLog(OnlineApp_Otp data, string type)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionstringex))
            {
                sqlcon.Open();
                string sqry = string.Empty;
                if (type == "Insert")
                    sqry = "insert into OnlineApp_Otp(Email,OTP,SendDate,Verified) values('"+data.Email+"','"+data.OTP+"','"+data.SendDate+"',0)";
                else if(type == "Update")
                    sqry = "update OnlineApp_Otp set Verified = 1, VerifyDate = '" + data.VerifyDate + "' where Email = '" + data.Email + "' and OTP = '" + data.OTP + "'";

                SqlCommand cmd1 = new SqlCommand(sqry, sqlcon);
                try
                {
                    cmd1.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "log\\" + "logErrors.txt";
                    var sw = new System.IO.StreamWriter(filename, true);
                    sw.WriteLine(DateTime.Now.ToString() + " " + ex.Message + " " + ex.InnerException);
                    sw.Close();
                    return false;
                }
                finally
                {
                    sqlcon.Close();
                }
            }            
        }

        //public string ScriptformissingApplicationtoAC()
        //{
        //    ActiveCampaignSF ActiveCampaign = new ActiveCampaignSF();
        //    string status = string.Empty;
        //    string connectioncams = ConfigurationManager.AppSettings["ConnectionstringCams"];
        //    var token = ConfigurationManager.AppSettings["ActiveCampaignToken"];
        //    string responseCode = string.Empty;
        //    DataSet ds = new DataSet();

        //    using (SqlConnection con = new SqlConnection(connectioncams))
        //    {
        //        con.Open();
        //        string SPName = "ActiveCampaignMissingAplicant";
        //        SqlCommand cmd = new SqlCommand(SPName, con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        SqlDataAdapter sqldata = new SqlDataAdapter(cmd);
        //        sqldata.Fill(ds);
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            {

        //                var email = ds.Tables[0].Rows[i]["FirstName"].ToString();
        //                List<ACCustomFields> customfields = new List<ACCustomFields>();
        //                customfields.Add(new ACCustomFields() { field = "75", value = ds.Tables[0].Rows[i]["AppFeePaid"].ToString() });
        //                customfields.Add(new ACCustomFields() { field = "15", value = ds.Tables[0].Rows[i]["ApplicantStatus"].ToString() });

        //                customfields.Add(new ACCustomFields()
        //                {
        //                    //Program of Interest
        //                    field = "5",
        //                    value = ActiveCampaign.ACProgrambasedonID(Convert.ToInt32(ds.Tables[0].Rows[i]["ProgramID"]))
        //                });
        //                customfields.Add(new ACCustomFields()
        //                {
        //                    //Expected Year of Enrolment
        //                    field = "4",
        //                    value = ActiveCampaign.ACTermbasedonID(Convert.ToInt32(ds.Tables[0].Rows[i]["AnticipatedEntryTermID"]))
        //                });
        //                customfields.Add(new ACCustomFields()
        //                {
        //                    //Nationality
        //                    field = "77",
        //                    value = ds.Tables[0].Rows[i]["Nationality"].ToString()
        //                });
        //                customfields.Add(new ACCustomFields()
        //                {
        //                    //ProspectID
        //                    field = "16",
        //                    value = ds.Tables[0].Rows[i]["ProspectID"].ToString()
        //                });
        //                customfields.Add(new ACCustomFields()
        //                {
        //                    //Prospect Date
        //                    field = "36",
        //                    value = ds.Tables[0].Rows[i]["ProspectIdDate"].ToString()
        //                });
        //                customfields.Add(new ACCustomFields()
        //                {
        //                    //How did you hear about CUD?
        //                    field = "13",
        //                    value = ds.Tables[0].Rows[i]["Source"].ToString()
        //                });
        //                customfields.Add(new ACCustomFields()
        //                {
        //                    //Date of Birth
        //                    field = "51",
        //                    value = Convert.ToDateTime(ds.Tables[0].Rows[i]["BirthDate"].ToString()).ToString("MM/dd/yyyy")
        //                });
        //                customfields.Add(new ACCustomFields()
        //                {
        //                    //Gender
        //                    field = "67",
        //                    value = ds.Tables[0].Rows[i]["Gender"].ToString()
        //                });
        //                customfields.Add(new ACCustomFields()
        //                {
        //                    //Application Status
        //                    field = "115",
        //                    value = "Yes"
        //                });
        //                customfields.Add(new ACCustomFields() { field = "56", value = ds.Tables[0].Rows[i]["Residence"].ToString() });
        //                customfields.Add(new ACCustomFields() { field = "27", value = ds.Tables[0].Rows[i]["HighSchoolCountry"].ToString() });
        //                customfields.Add(new ACCustomFields() { field = "22", value = ds.Tables[0].Rows[i]["HighSchoolName"].ToString() });

        //                ACViewModel contactview = new ACViewModel
        //                {
        //                    contact = new ACContact
        //                    {
        //                        firstName = ds.Tables[0].Rows[i]["FirstName"].ToString(),
        //                        email = ds.Tables[0].Rows[i]["ProspectEmail"].ToString(),
        //                        lastName = ds.Tables[0].Rows[i]["LastName"].ToString(),
        //                        phone = ds.Tables[0].Rows[i]["ProspectPhone"].ToString(),
        //                        fieldValues = customfields
        //                    },
        //                };
        //                var contactid = IsEmailExists(ds.Tables[0].Rows[i]["ProspectEmail"].ToString());
        //                if (!contactid)
        //                {
        //                    try
        //                    {
        //                        using (var httpClient = new HttpClient())
        //                        {
        //                            httpClient.DefaultRequestHeaders.Add("Api-Token", token);
        //                            string strContactList = JsonConvert.SerializeObject(contactview).ToString();

        //                            StringContent content = new StringContent(strContactList, Encoding.UTF8, "application/json");

        //                            HttpResponseMessage response = httpClient.PostAsync("https://cud939.api-us1.com/api/3/contact/sync", content).Result;

        //                            if (response.IsSuccessStatusCode)
        //                            {
        //                                responseCode = "Success";
        //                            }

        //                        }
        //                       // return responseCode;
        //                    }
        //                    catch (Exception Ex)
        //                    {
        //                        return $"{Ex.Message} - {Ex.InnerException}";
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return responseCode;
        //}

        //public bool IsEmailExists(string email)
        ////public bool IsEmailExists(string email)
        //{
        //    bool Exists = false;
        //    string token = ConfigurationManager.AppSettings["ActiveCampaignToken"];
        //    try
        //    {
        //        using (var httpClient = new HttpClient())
        //        {
        //            httpClient.DefaultRequestHeaders.Add("Api-Token", token);

        //            var response = httpClient.GetAsync("https://cud939.api-us1.com/api/3/contacts?email=" + email).GetAwaiter().GetResult();
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var responseContent = response.Content;
        //                // return responseContent.ReadAsStringAsync().GetAwaiter().GetResult();
        //                string apiResponse = responseContent.ReadAsStringAsync().GetAwaiter().GetResult();
        //                var EmailExists = JsonConvert.DeserializeObject<JObject>(apiResponse);
        //                if (EmailExists["contacts"] != null)
        //                {
        //                    if (EmailExists["contacts"].Count() > 0)
        //                    {

        //                        Exists = true;
        //                    }
        //                    else
        //                    {
        //                        Exists = false;
        //                    }
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //return $"{ex.Message} - {ex.InnerException}";
        //        throw;
        //    }

        //    return Exists;
        //}
    }
}