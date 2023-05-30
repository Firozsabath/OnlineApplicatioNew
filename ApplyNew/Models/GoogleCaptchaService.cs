using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ApplyNew.Models
{
    public class GoogleCaptchaService
    {
        public async Task<bool> VerifyToken(string token)
        {
            var url = ConfigurationManager.AppSettings["GoogleRecaptchaAPI"];
            var secretKey = ConfigurationManager.AppSettings["GoogleRecaptchaSecretKey"];
            try
            {
                string framedURL = url + "?secret=" + secretKey + "&response=" + token + "";
                using (var client = new HttpClient())
                {
                    var apiresponse = await client.GetAsync(framedURL);
                    if(apiresponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var responseString = await apiresponse.Content.ReadAsStringAsync();
                        var googleResult = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(responseString);

                        return googleResult.success && googleResult.score >= 0.5;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }

    public class GoogleCaptchaResponse
    {
        public bool success { get; set; }
        public double score { get; set; }
    }
}