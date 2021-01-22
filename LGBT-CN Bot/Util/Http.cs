using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LGBTCN.Bot.Util
{
    public class Http
    {
        private static readonly HttpClient client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        public static async Task<string> GetAsync(string url)
        {
            try
            {
                return await client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                Log.E("http.get", ex.Message);
                return null;
            }
        }

        public static async Task<string> PostAsync(string url, string content)
        {
            try
            {
                var httpContent = new StringContent(content);
                var response = await client.PostAsync(url, httpContent);

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Log.E("http.post", ex.Message);
                return null;
            }
        }
    }
}
