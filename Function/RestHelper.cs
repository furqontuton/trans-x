using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.Odbc;
using System.Data;
using System.Windows.Forms;

namespace WA_Send_API.Function
{
    public static class RestHelper
    {


        private static readonly string baseURL = "http://10.1.4.23/api/";
        //private static readonly string baseURL = "https://omnichannel.qiscus.com/api/v3/admin/hsm/";

        public static async Task<string> Post(string apikey, string from_phone, string recipient, string message )
        {
            
            var inputData = new Dictionary<string, string>
            //var inputData = new Dictionary<string, string>
            {
                {"apikey", apikey },
                {"from_phone", from_phone },
                {"recipient", recipient },
                {"message", message }
            };
            var input = new FormUrlEncodedContent(inputData);
            using (HttpClient Client = new HttpClient())
            {
                using (HttpResponseMessage res = await Client.PostAsync(baseURL + "sendMessage", input))
                //using (HttpResponseMessage res = await Client.PostAsync(baseURL + "create", inputData))
                {
                    using (HttpContent content = res.Content)
                    { 
                        string data = await content.ReadAsStringAsync();
                        if (data != null)
                        {
                            return data;
                        }
                    }
                }

            }
            return string.Empty;
        }

         /*   public static string BeautifyJson(string jsonStr)
        {
            JToken parseJson = JToken.Parse(jsonStr);
            return parseJson.ToString(Formatting.Indented);
        }
         */
    }
}
