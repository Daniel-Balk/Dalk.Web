using Dalk.Web.HttpServer.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Dalk.Web.HttpServer
{
    public class HttpRequest
    {
        private readonly byte[][] vs;
        private readonly byte[] bytes;
        internal HttpResponse response;

        public HttpMethod Method { get; set; }

        public byte[] Content
        {
            get
            {
                return ctn.ToArray();
            }
        }

        public byte[] Raw
        {
            get
            {
                return bytes;
            }
        }

        public Cookie[] Cookies
        {
            get
            {
                List<Cookie> cookies = new List<Cookie>();
                if (Headers.ContainsKey("Cookie"))
                {
                    var cks = Headers["Cookie"].Split(new string[] { ";" },StringSplitOptions.RemoveEmptyEntries);
                    cks.ToList().ForEach(c =>
                    {
                        var ck = c.Trim().TrimEnd();
                        var sp = ck.Split('=');
                        var key= sp.FirstOrDefault();
                        var value = ck.Remove(0, key.Length + 1);
                        Cookie k = new Cookie(key.Trim().TrimEnd(), value.TrimEnd().Trim());
                        cookies.Add(k);
                    });
                }
                else
                {
                    
                }
                return cookies.ToArray();
            }
        }

        private List<byte> ctn;

        public HttpRequest(byte[][] vs, byte[] bytes)
        {
            this.vs = vs;
            this.bytes = bytes;

            Init();
        }

        public Headers Headers { get; private set; }

        public string UserAgent
        {
            get
            {
                return Headers["User-Agent"];
            }
        }

        public string Host
        {
            get
            {
                return Headers["Host"];
            }
        }

        public string Path { get; private set; }

        private void Init()
        {
            Headers = new Headers();
            ctn = new List<byte>();
            var mt = vs[0];
            var mto = Encoding.UTF8.GetString(mt).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var method = mto.First();
            Path = mto[1];
            Method = (HttpMethod)Enum.Parse(typeof(HttpMethod), method);
            var hab = vs.ToList();
            hab.RemoveAt(0);
            bool switchToBody = false;
            int i = 0;
            bool j = false;
            int k = 0;
            ctn.Clear();
            hab.ForEach(x =>
            {
                var xs = Encoding.UTF8.GetString(x);
                if (!string.IsNullOrWhiteSpace(xs))
                {
                    if (i + 1 < hab.Count)
                    {
                        if (!Encoding.UTF8.GetString(hab[i + 1]).Contains(": "))
                        {
                            j = true;
                        }
                    }
                    i++;
                }
                if (!switchToBody)
                {
                    try
                    {
                        var dqi = xs.IndexOf(":");
                        var value = xs.Remove(0, dqi + 2);
                        var key = xs.Remove(dqi);
                        Headers[key.Trim().TrimEnd()] = value;
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    ctn.AddRange(x);
                }
                if (j)
                {
                    k++;
                    switchToBody = k > 1;
                }
            });
            try
            {
                ctn.RemoveAt(0);
            }
            catch(Exception) { }
        }

        public HttpResponse GetResponse()
        {
            return response;
        }
        internal TcpClient sender;
        internal int bufferSize;

        public TcpClient GetSender()
        {
            return sender;
        }

        public bool IsWebsocket { get; private set; } = false;
        public WebSocket WebSocket { get; private set; }

        public WebSocket AcceptWebSocket()
        {
            response.wsclose = false;

            var key = Headers["Sec-WebSocket-Key"] + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            var sha = SHA1.Create();
            var hash = sha.ComputeHash(Encoding.Default.GetBytes(key));
            var accept = Convert.ToBase64String(hash);
            response.Headers["Sec-WebSocket-Accept"] = accept;
            response.StatusCode = 101;
            response.Headers["Upgrade"] = "websocket";
            response.Headers["Connection"] = "Upgrade";

            WebSocket = new WebSocket(sender);
            response.Send();
            return WebSocket;
        }

        internal bool Proof()
        {
            if (Headers.ContainsKey("Upgrade"))
            {
                if (Headers["Upgrade"].Contains("websocket"))
                {
                    response.wsclose = true;
                    IsWebsocket = true;
                }
            }
            return true;
        }
    }
}