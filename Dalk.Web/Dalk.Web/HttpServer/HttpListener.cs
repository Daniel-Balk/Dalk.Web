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
            try
            {
                var tcp = listener.AcceptTcpClient();
                var bytes = ReadAllBytes(tcp);
                HttpRequest request = new HttpRequest(SplitByteArray(RemoveValues(bytes, 0x0d), 0x0a).ToArray(), bytes)
                {
                    sender = tcp
                };
                request.response = new HttpResponse(tcp);
                return request;
            }
            catch (Exception ex)
            {
                LogError?.Invoke(ex.ToString());
                return AcceptRequest();
            }
        }

        public void AcceptRequestAsync()
        {
            BeginAcceptRequest(InvokeRS);
        }

        private async void InvokeRS(IAsyncResult ar)
        {
            var tl = await EndAcceptRequest(ar);
            RequestAccepted?.Invoke(tl);
        }

        public IAsyncResult BeginAcceptRequest(AsyncCallback callback)
        {
            return listener.BeginAcceptTcpClient(callback, null);
        }

        public event RequestAcceptEventHandler RequestAccepted;
        public void HandleEventBased()
        {
            while (true)
            {
                var r = AcceptRequest();
                RequestAccepted?.Invoke(r);
            }
        }  
        
        public void HandleEventBasedAsync()
        {
            AsyncCallback h = (y) => { };
            h = async (x) =>
            {
                var r = await EndAcceptRequest(x);
                RequestAccepted?.Invoke(r);
                BeginAcceptRequest(h);
            };
            BeginAcceptRequest(h);
        }

        public async Task<HttpRequest> EndAcceptRequest(IAsyncResult ar)
        {
            var tcp = listener.EndAcceptTcpClient(ar);
            try {
                var bytes = await ReadAllBytesAsync(tcp);
                HttpRequest request = new HttpRequest(SplitByteArray(RemoveValues(bytes, 0x0d), 0x0a).ToArray(), bytes)
                {
                    sender = tcp
                };
                request.response = new HttpResponse(tcp);
                return request;
            }
            catch (Exception ex)
            {
                LogError?.Invoke(ex.ToString());
                return AcceptRequest();
            }
        }

        public int BufferSize { get; set; } = 8192;
        private byte[] ReadAllBytes(TcpClient client)
        {
            int bytesRead = 0;
            byte[] recvbuf = new byte[BufferSize];
            do
            {
                bytesRead = client.GetStream().Read(recvbuf, 0, recvbuf.Length);
                return recvbuf;

            } while (bytesRead != 0);
        }

        private async Task<byte[]> ReadAllBytesAsync(TcpClient client)
        {
            int bytesRead = 0;
            byte[] recvbuf = new byte[BufferSize];
            do
            {
                bytesRead = await client.GetStream().ReadAsync(recvbuf, 0, recvbuf.Length);
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

        public void Stop()
        {
            listener.Stop();
        }

        public event Action<string> LogError;
    }
    public delegate void RequestAcceptEventHandler(HttpRequest request);
}