using Dalk.Web;
using System;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            System.Net.WebClient ec = new();
            WebClient wc = new();
            Console.WriteLine(wc.DownloadString("https://www.dalkyt.de/mcf/info.txt"));
        }
    }
}
