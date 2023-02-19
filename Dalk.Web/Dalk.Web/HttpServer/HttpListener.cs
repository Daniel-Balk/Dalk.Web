using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dalk.Web.HttpServer
{
    public class HttpListener
    {
        public int Port { get; }
        private TcpListener listener;
        private bool active;
        public HttpListener(int port)
        {
            Port = port;
        }

        public void Start()
        {
            active = true;
            listener = new TcpListener(System.Net.IPAddress.Any, Port);
            listener.Start();

            CreateTaskPender();
        }
#if NET5_0_OR_GREATER
        private List<(TcpClient tcp, byte[] bytes)> pendingRequests = new();
#else
        class i {
            public i(TcpClient tcp, byte[] bytes){
                this.tcp = tcp;
                this.bytes = bytes;
            }

            public TcpClient tcp;
            public byte[] bytes;
        }
        private List<i> pendingRequests = new List<i>();
#endif
        private static Dictionary<TcpClient, int> idcache = new Dictionary<TcpClient, int>();
        internal static Dictionary<TcpClient, Action<byte[]>> incept = new Dictionary<TcpClient, Action<byte[]>>();
        internal static Dictionary<TcpClient, bool> _incept = new Dictionary<TcpClient, bool>();
        private int idc = 0;

        internal static void Exclude(TcpClient client)
        {
            idcache[client] = -1;
        }

        internal static void Intercept(TcpClient client, Action<byte[]> action)
        {
            _incept[client] = true;
            incept[client] = action;
        }

        private void CreateTaskPender()
        {
            Task.Run(() =>
            {
                while (active)
                {
                    var tcp = listener.AcceptTcpClient();
                    idcache[tcp] = idc++;
                    var bytes = ReadAllBytes(tcp, BufferSize);

                    _incept[tcp] = false;
                    var task = Task.Run(() => Watch(tcp));

#if NET5_0_OR_GREATER
                    pendingRequests.Add((tcp, bytes));
#else
                    pendingRequests.Add(new i(tcp, bytes));
#endif
                }                
            });
        }

        private void Watch(TcpClient tcp)
        {
            while (tcp.Connected && active)
            {
                if (idcache[tcp] < 0)
                    return;

                var bytes = ReadAllBytes(tcp, BufferSize);

                if (idcache[tcp] < 0)
                    return;

                if (_incept[tcp])
                {
                    incept[tcp](bytes);
                }
                else
                {
#if NET5_0_OR_GREATER
                    pendingRequests.Add((tcp, bytes));
#else
                    pendingRequests.Add(new i(tcp, bytes));
#endif
                }
            }
        }

        public HttpRequest AcceptRequest()
        {
            try
            {
                while (active)
                {
                    if(pendingRequests.Count == 0)
                    {
                        Thread.Sleep(5);
                        continue;
                    }

                    var poll = pendingRequests[0];
                    pendingRequests.RemoveAt(0);
                    var tcp = poll.tcp;
                    var bytes = poll.bytes;

                    if (bytes.ToList().FindLastIndex(x => x != 0) < 0)
                        continue;

                    HttpRequest request = new HttpRequest(SplitByteArray(RemoveValues(bytes, 0x0d), 0x0a).ToArray(), bytes)
                    {
                        sender = tcp,
                        bufferSize = BufferSize
                    };
                    request.response = new HttpResponse(tcp) { request = request};
                    if (!request.Proof())
                        continue;
                    return request;
                }
                //var tcp = listener.AcceptTcpClient();
                //var bytes = ReadAllBytes(tcp);
            }
            catch (Exception ex)
            {
                LogError?.Invoke(ex.ToString());
                return AcceptRequest();
            }

            return null;
        }
        /*
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
        */

        public event RequestAcceptEventHandler RequestAccepted;
        public void HandleEventBased()
        {
            while (true)
            {
                var r = AcceptRequest();
                RequestAccepted?.Invoke(r);
            }
        }  
        /*
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
        */
        public int BufferSize { get; set; } = 8192;
        internal static byte[] ReadAllBytes(TcpClient client, int bufferSize)
        {
            int bytesRead = 0;
            byte[] recvbuf = new byte[bufferSize];
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
            active = false;
        }
        internal static byte[] TrimEnd(byte[] array)
        {
            int lastIndex = Array.FindLastIndex(array, b => b != 0);

            Array.Resize(ref array, lastIndex + 1);

            return array;
        }
        public event Action<string> LogError;
    }
    public delegate void RequestAcceptEventHandler(HttpRequest request);
}