using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Dalk.Web.HttpServer
{
    public class HttpListener
    {
        public int Port { get; }
        private TcpListener listener;
        public HttpListener(int port)
        {
            Port = port;
        }

        public void Start()
        {
            listener = new TcpListener(System.Net.IPAddress.Any, Port);
            listener.Start();
        }

        public HttpRequest AcceptRequest()
        {
            var tcp = listener.AcceptTcpClient();
            var bytes = ReadAllBytes(tcp);
            HttpRequest request = new HttpRequest(SplitByteArray(RemoveValues(bytes, 0x0d), 0x0a).ToArray(),bytes);
            request.response = new HttpResponse(tcp);
            return request;
        }

        private static byte[] ReadAllBytes(TcpClient client)
        {
            int bytesRead = 0;
            byte[] recvbuf = new byte[8192];
            do
            {
                bytesRead = client.GetStream().Read(recvbuf, 0, recvbuf.Length);
                return recvbuf;

            } while (bytesRead != 0);
        }

        private static IEnumerable<byte[]> SplitByteArray(IEnumerable<byte> source, byte marker)
        {
            List<byte> current = new List<byte>();

            foreach (byte b in source)
            {
                if (b == marker)
                {
                    if (current.Count > 0)
                        yield return current.ToArray();

                    current.Clear();
                }

                current.Add(b);
            }

            if (current.Count > 0)
                yield return current.ToArray();
        }

        private static byte[] RemoveValues(byte[] source, byte key)
        {
            var lst = source.ToList();
            lst.RemoveAll(x => x == key);
            return lst.ToArray();                
        }
    }
}
