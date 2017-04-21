using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nsdn.Nyasama.Uwp.Utilities
{
    class RequestParams
    {
        public Dictionary<String, String> Modules { get; set; } = new Dictionary<string, string>();
        public Dictionary<String, String> Cookies { get; set; } = new Dictionary<string, string>();
        private CookieCollection _cookieCollection { get; set; } = new CookieCollection();

        public RequestParams() { }
        public RequestParams(Dictionary<String, String> modules, Dictionary<String, String> cookies)
        {
            this.Modules = modules;
            this.Cookies = cookies;
        }

        public RequestParams AddModule(string name, string value)
        {
            Modules.Add(name, value);
            return this;
        }

        public string ParseModules()
        {
            try
            {
                String s = "?";
                foreach (var module in Modules)
                {
                    s += $"{module.Key}={module.Value}&";
                }
                return s;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public CookieCollection ParseCookies()
        {
            foreach (var cookie in Cookies)
            {
                _cookieCollection.Add(new Cookie(cookie.Key, cookie.Value));
            }
            return _cookieCollection;
        }
    }
}
