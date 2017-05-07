using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Nsdn.Nyasama.Uwp.Utilities
{
    class Network
    {
        public static string NYASAMA_URL = "http://bbs.nyasama.com/";
        public static string NYASAMA_API_URL = NYASAMA_URL + "api/mobile/index.php";
        public static string USER_AVATAR_URL = NYASAMA_URL + "uc_server/avatar.php";
        public static string USER_AGENT = "Mozilla/5.0 (iPhone; U; CPU iPhone OS 3_0 like Mac OS X; en-us) AppleWebKit/420.1 (KHTML, like Gecko) Version/3.0 Mobile/1A542a Safari/419.3";
        public static int TIME_OUT = 10000;

        public static string GetUserAvatarLink(int uid)
        {
            RequestParams rp = new RequestParams();
            rp.Modules.Add("uid", uid.ToString());
            rp.Modules.Add("size", "middle");
            return USER_AVATAR_URL + rp.ParseModules();
        }

        public static async Task<JObject> GetContentJsonAsync(RequestParams rp)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(NYASAMA_API_URL + rp.ParseModules());
            request.Method = "GET";
            request.Headers["User-Agent"] = USER_AGENT;
            request.ContinueTimeout = TIME_OUT;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    JObject json = JObject.Parse(await sr.ReadToEndAsync());
                    return json;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
