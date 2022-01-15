using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Dalk.Web.HttpServer
{
    public class HttpResponse
    {
        public Stream GetStream() => tcp.GetStream();
        private const string HttpVerion = "HTTP/1.1";
        private readonly TcpClient tcp;
        int sci = 200;
        public int StatusCode
        {
            get
            {
                return sci;
            }
            set
            {
                sci = value;
                StatusCodeString = StatusCodeToString();
            }
        }
        public string StatusCodeString { get; set; } = "OK";
        public string ContentType
        {
            get
            {
                return Headers["Content-Type"];
            }
            set
            {
                Headers["Content-Type"] = value;
            }
        }
        public long ContentLenght
        {
            get
            {
                return long.Parse(Headers["Content-Length"]);
            }
            set
            {
                Headers["Content-Length"] = value.ToString();
            }
        }
        public Headers Headers { get; set; }

        public HttpResponse(TcpClient tcp)
        {
            this.tcp = tcp;
            Init();
        }

        private const string Server = "Dalk.Web HTTPListener";
        private static readonly Random random = new Random();
        private void Init()
        {
            sendData = new List<byte>();
            Headers = new Headers();
            ContentType = "text/html";
            Headers["Server"] = Server;
            string guid = "";
            var chrs = "1234567890abcdef";
            for (int i = 0; i < 9; i++)
            {
                guid += chrs[random.Next(chrs.Length)];
            }
            Headers["ETag"] = $@"W/""{guid}-a""";
            Headers["Date"] = DateTime.Now.ToString();
            Headers["Transfer-Encoding"] = "UTF8";
        }

        List<byte> sendData;

        public void Write(byte[] data)
        {
            InitResponse();
            sendData.AddRange(data);
        }
        bool init = false;
        private void InitResponse()
        {
            if (!init)
            {
                byte[] rh = Encoding.UTF8.GetBytes(ConstructRespHead());
                sendData.AddRange(rh);
                init = true;
            }
        }

        private string ConstructRespHead()
        {
            string response = $@"{HttpVerion} {StatusCode} {StatusCodeString}
";
            Headers.ToList().ForEach(x =>
            {
                response += $@"{x.Key}: {x.Value}
";
            });
            response += @"
";
            return response;
        }

        bool sent = false;
        public void Send(bool close = true)
        {
            if (!sent)
            {
                InitResponse();
                byte[] bytes = sendData.ToArray();
                tcp.GetStream().Write(bytes, 0, bytes.Length);
                if (close)
                    tcp.GetStream().Close();
                if (close)
                    tcp.Close();
            }
            else
            {
                throw new ResponseAlreadySentException("Response has already been sent!");
            }
        }

        public async Task SendAsync(bool close = true)
        {
            if (!sent)
            {
                InitResponse();
                byte[] bytes = sendData.ToArray();
                await tcp.GetStream().WriteAsync(bytes, 0, bytes.Length);
                if (close)
                    tcp.GetStream().Close();
                if (close)
                    tcp.Close();
            }
            else
            {
                throw new ResponseAlreadySentException("Response has already been sent!");
            }
        }

        private string StatusCodeToString()
        {
            string sc = "OK";
            switch (StatusCode)
            {
                case 101:
                    sc = "Switching Protocols";
                    break;
                case 102:
                    sc = "Processing";
                    break;
                case 103:
                    sc = "Early Hints";
                    break;

                case 200:
                    sc = "OK";
                    break;
                case 201:
                    sc = "Created";
                    break;
                case 202:
                    sc = "Accepted";
                    break;
                case 203:
                    sc = "Non-Authoritative Information";
                    break;
                case 204:
                    sc = "No Content";
                    break;
                case 205:
                    sc = "Reset Content";
                    break;
                case 206:
                    sc = "Partial Content";
                    break;
                case 207:
                    sc = "Multi-Status";
                    break;
                case 208:
                    sc = "Already Reported";
                    break;
                case 226:
                    sc = "IM Used";
                    break;


                case 300:
                    sc = "Multiple Choices";
                    break;
                case 301:
                    sc = "Moved Permanently";
                    break;
                case 302:
                    sc = "Found";
                    break;
                case 303:
                    sc = "See Other";
                    break;
                case 304:
                    sc = "Not Modified";
                    break;
                case 305:
                    sc = "Use Proxy";
                    break;
                case 306:
                    sc = "Switch Proxy";
                    break;
                case 307:
                    sc = "Temporary Redirect";
                    break;
                case 308:
                    sc = "Permanent Redirect";
                    break;

                case 400:
                    sc = "Bad Request";
                    break;
                case 401:
                    sc = "Unauthorized";
                    break;
                case 402:
                    sc = "Payment Required";
                    break;
                case 403:
                    sc = "Forbidden";
                    break;
                case 404:
                    sc = "Not Found";
                    break;
                case 405:
                    sc = "Not Found";
                    break;
                case 406:
                    sc = "Not Acceptable";
                    break;
                case 407:
                    sc = "Proxy Authentication Required";
                    break;
                case 408:
                    sc = "Request Timeout";
                    break;
                case 409:
                    sc = "Request Timeout";
                    break;
                case 410:
                    sc = "Gone";
                    break;
                case 411:
                    sc = "Length Required";
                    break;
                case 412:
                    sc = "Precondition Failed";
                    break;
                case 413:
                    sc = "Payload Too Large";
                    break;
                case 414:
                    sc = "URI Too Long";
                    break;
                case 415:
                    sc = "Unsupported Media Type";
                    break;
                case 416:
                    sc = "Range Not Satisfiable";
                    break;
                case 417:
                    sc = "Expectation Failed";
                    break;
                case 418:
                    sc = "I'm a teapot";
                    break;
                case 421:
                    sc = "Misdirected Request";
                    break;
                case 422:
                    sc = "Unprocessable Entity";
                    break;
                case 423:
                    sc = "Locked";
                    break;
                case 424:
                    sc = "Failed Dependency";
                    break;
                case 425:
                    sc = "Failed Dependency";
                    break;
                case 426:
                    sc = "Upgrade Required";
                    break;
                case 428:
                    sc = "Precondition Required";
                    break;
                case 429:
                    sc = "Too Many Requests";
                    break;
                case 431:
                    sc = "Request Header Fields Too Large";
                    break;
                case 451:
                    sc = "Unavailable For Legal Reasons";
                    break;

                case 500:
                    sc = "Internal Server Error";
                    break;
                case 501:
                    sc = "Not Implemented";
                    break;
                case 502:
                    sc = "Bad Gateway";
                    break;
                case 503:
                    sc = "Service Unavailable";
                    break;
                case 504:
                    sc = "Gateway Timeout";
                    break;
                case 505:
                    sc = "HTTP Version Not Supported";
                    break;
                case 506:
                    sc = "Variant Also Negotiates";
                    break;
                case 507:
                    sc = "Insufficient Storage";
                    break;
                case 508:
                    sc = "Loop Detected";
                    break;
                case 510:
                    sc = "Not Extended";
                    break;
                case 511:
                    sc = "Network Authentication Required";
                    break;
                default:
                    break;
            }
            return sc;
        }
    }
}
