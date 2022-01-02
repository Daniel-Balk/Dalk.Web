using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
            hab.ForEach(x =>
            {
                var xs = Encoding.UTF8.GetString(x);
                if (string.IsNullOrWhiteSpace(xs))
                {
                    switchToBody = true;
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
                    ctn.Add(0x0a);
                }
            });
        }

        public HttpResponse GetResponse()
        {
            return response;
        }
        internal TcpClient sender;
        public TcpClient GetSender()
        {
            return sender;
        }
    }
}