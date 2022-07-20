using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Dalk.Web
{
    public class WebClient
    {
        public WebClient()
        {
            UserAgent = "Dalk.Web WebClient";
        }

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public Headers Headers { get; set; } = new Headers();
        private static WebRequest ConstructWebRequest(string url, HttpMethod method, Headers headers, int timeout = 30 * 1000,string contentType = null)
        {
            WebRequest wr = WebRequest.Create(url);
            wr.Method = method.ToString().ToUpper();
            wr.Headers.Clear();
            headers.ToList().ForEach((h) =>
            {
                wr.Headers.Add(h.Key, h.Value);

            });
            wr.Timeout = timeout;
            wr.ContentType = contentType;
            return wr;
        }

        public byte[] Post(string url, byte[] data, out int statusCode, string contentType = "application/x-www-form-urlencoded")
        {
            ConstructHeaders(url);
            var wr = ConstructWebRequest(url: url, method: HttpMethod.POST, headers: Headers, contentType: contentType);
            wr.ContentLength = data.Length;
            var ds = wr.GetRequestStream();
            ds.Write(data, 0, data.Length);
            ds.Close();

            HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
            var rd = GetDataFromResponse(response);
            statusCode = (int)response.StatusCode;
            response.Close();

            return rd;
        }

        public byte[] Post(string url, byte[] data, string contentType = "application/x-www-form-urlencoded")
        {
            return Post(url, data, out _, contentType);
        }

        public byte[] Get(string url, out int statusCode)
        {
            ConstructHeaders(url);
            var wr = ConstructWebRequest(url: url, method: HttpMethod.GET, headers: Headers, contentType: null);

            HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
            var rd = GetDataFromResponse(response);
            statusCode= (int)response.StatusCode;
            response.Close();

            return rd;
        }

        private void ConstructHeaders(string url)
        {
            var uri = new Uri(url);
            Headers["Host"] = uri.Host;
            Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8";
            Headers["Accept-Language"] = "en-US;q=0.7,en;q=0.3";
            Headers["Accept-Encoding"] = "gzip, deflate, br";
            Headers["Referer"] = uri.AbsoluteUri.Remove(uri.AbsoluteUri.IndexOf("/", 8)) + "/";
            Headers["Dnt"] = "1";
            Headers["Upgrade-Insecure-Requests"] = "1";
            Headers["Sec-Fetch-Dest"] = "document";
            Headers["Sec-Fetch-Mode"] = "navigate";
            Headers["Sec-Fetch-Site"] = "same-origin";
            Headers["Sec-Fetch-User"] = "?1";
            Headers["Te"] = "trailers";
            Headers["Content-Type"] = "";
            Headers["Content-Length"] = "";
            Headers["Cache-Control"] = "max-age=0";
        }

        public byte[] Get(string url)
        {
            return Get(url, out _);
        }

        public byte[] Post(string url, object o)
        {
            return Post(url,
                Encoding.GetBytes(JsonConvert.SerializeObject(o)), out _,
                "application/json"
                );
        }

        public byte[] Post(string url, string str)
        {
            return Post(url,
                Encoding.GetBytes(str), out _,
                "application/text"
                );
        }

        public void DownloadFile(string url, string filename) => File.WriteAllBytes(filename, Get(url));
        public byte[] DownloadBytes(string url) => Get(url);
        public byte[] DownloadData(string url) => Get(url);
        public string DownloadString(string url) => Encoding.GetString(Get(url));

        private static byte[] GetDataFromResponse(HttpWebResponse response)
        {
            var stream = response.GetResponseStream();
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            stream.Close();

            var bytes = ms.ToArray();
            ms.Close();
            return bytes;
        }


        public string UserAgent
        {
            get
            {
                return Headers["User-Agent"];
            }
            set
            {
                Headers["User-Agent"] = value;
            }
        }
    }
}