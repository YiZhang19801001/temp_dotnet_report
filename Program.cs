using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace demoBusinessReport
{
    public class Program
    {
        /** worked version 1 but get error when copy whole project to another machine*/
        public static void Main(string[] args)
        {
           BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>()

               //params string[] urls
               .UseUrls(urls: "http://192.168.1.88:5000")
               //.UseUrls(urls: "http://192.168.20.101:5000")

               .Build();

        /** Host on IIS */
        //public static void Main(string[] args)
        //{
        //    var host = new WebHostBuilder()
        //        .UseKestrel()
        //        .UseContentRoot(Directory.GetCurrentDirectory())
        //        .UseIISIntegration()
        //        .UseStartup<Startup>()
        //        .Build();

        //    host.Run();
        //}

        // /** default version to start my project */
        // public static void Main(string[] args)
        // {
        //     BuildWebHost(args).Run();
        // }

        // public static IWebHost BuildWebHost(string[] args) =>
        //     WebHost.CreateDefaultBuilder(args)
        //         .UseStartup<Startup>()
        //         .Build();
    }
}
