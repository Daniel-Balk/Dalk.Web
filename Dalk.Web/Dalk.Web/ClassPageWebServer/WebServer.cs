using Dalk.Web.HttpServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Dalk.Web.ClassPageWebServer
{
    public class WebServer
    {
        public List<Thread> RenderThreads { get; set; }
        public Thread ListenerThread { get; set; }
        public int Threads { get; set; } = 32;
        public int Port { get; set; } = 5000;
        private HttpListener HttpListener { get; set; }
        List<Type> PageTypes { get; set; }
        List<WebPage> DefaultPages { get; set; }
        Type LayoutPageType { get; set; }
        public bool EnableWwwroot { get; set; }

        public WebServer()
        {
            RenderThreads = new List<Thread>();
            DefaultPages = new List<WebPage>();
            PageTypes = new List<Type>();
            LayoutPageType = typeof(LayoutPage);
            EnableWwwroot = true;
        }

        public void Run()
        {
            ListenerThread = new Thread(ListenWebPage);
            ListenerThread.Name = "Listener";
            ListenerThread.Start();
        }

        public void RegisterPage<T>() where T : WebPage
        {
            PageTypes.Add(typeof(T));
            DefaultPages.Add((WebPage)Activator.CreateInstance(typeof(T)));
        }

        public void SetLayout<T>() where T : LayoutPage
        {
            LayoutPageType = typeof(T);
        }

        private void ListenWebPage()
        {
            HttpListener = new HttpListener(Port);
            HttpListener.LogError += Error;
            HttpListener.Start();
            while (true)
            {
                try
                {
                    var request = HttpListener.AcceptRequest();
                    Thread handleThread = new Thread(HandleRequest);
                    ThreadAdd(handleThread);
                    handleThread.Start(request);
                }
                catch (Exception ex)
                {
                    Error(ex.ToString());
                }
            }
        }

        private void ThreadAdd(Thread thread)
        {
            if(RenderThreads.Count < Threads)
            {
            }
            else
            {
                RenderThreads[0].Interrupt();
            }
            RenderThreads.Add(thread);
        }

        private void HandleRequest(object obj)
        {
            try
            {
                var request = obj as HttpRequest;
                var tcpSender = request.GetSender();
                Info($"Accesslog: Access from {tcpSender.Client.RemoteEndPoint}");
                var response = request.GetResponse();
                string html = null;
                byte[] bytes;
                bool isFile = false;
                string fp = null;
                var wp = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/wwwroot";
                if (!Directory.Exists(wp))
                    Directory.CreateDirectory(wp);
                        
                var pp = wp + request.Path;
                var fpp = Path.GetFullPath(pp);
                if (fpp.StartsWith(Path.GetFullPath(wp)))
                {
                    if (File.Exists(fpp))
                    {
                        fp = fpp;
                        isFile = true;
                    }
                }


                if (isFile)
                {
                    bytes = File.ReadAllBytes(fp);
                    switch (Path.GetExtension(fp))
                    {
                        case ".css":
                            response.ContentType = "text/css";
                            break;
                        case ".js":
                            response.ContentType = "text/javascript";
                            break;
                        case ".html":
                            response.ContentType = "text/html";
                            break;

                        default:
                            response.ContentType = "application/octet-stream";
                            break;
                    }
                    response.ContentLenght = bytes.Length;
                }
                else
                {
                    int i = 0;
                    var layout = (LayoutPage)Activator.CreateInstance(LayoutPageType);
                    DefaultPages.ForEach(p =>
                    {
                        if (p.MatchesRoute(request.Path))
                        {
                            var page = (WebPage)Activator.CreateInstance(PageTypes[i]);
                            page.Initialize(request);
                            html = layout.GetCompletePage(page);
                        }
                        i++;
                    });
                    if (html == null)
                        html = layout.Get404Page();
                    bytes = Encoding.UTF8.GetBytes(html);
                    response.ContentLenght = bytes.Length;
                }
                response.Write(bytes);
                response.Send();
                Thread.CurrentThread.Interrupt();
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
            }
        }

        public event LogEventHandler Log;

        private void Debug(string message)
        {
            Log?.Invoke(LogLevel.Debug, message);
        }
        private void Info(string message)
        {
            Log?.Invoke(LogLevel.Info, message);
        }
        private void Warn(string message)
        {
            Log?.Invoke(LogLevel.Warn, message);
        }
        private void Error(string message)
        {
            Log?.Invoke(LogLevel.Error, message);
        }
    }
}
