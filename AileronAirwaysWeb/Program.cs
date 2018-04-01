using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace AileronAirwaysWeb
{
    public class Program
    {   //unsecure connection
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        //MS doc kestred connection
        //public static void Main(string[] args)
        //{
        //    BuildWebHost(args).Run();
        //}
        //public static IWebHost BuildWebHost(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //    .UseStartup<Startup>()
        //    .UseKestrel(options =>
        //    {
        //        options.Listen(IPAddress.Loopback, 44346, listenOptions =>
        //                {
        //                    listenOptions.UseHttps("c:\\tmp\\localhost.pfx", "P55sw0rd");
        //              });
        //    })
        //            .UseIISIntegration()
        //            .UseContentRoot(Directory.GetCurrentDirectory())
        //            .UseStartup<Startup>()
        //            .UseUrls("https://localhost:44346/")
        //            .Build();


        //blog 
        //public static void Main(string[] args)
        //{
        //    var config = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddEnvironmentVariables()
        //        .AddJsonFile("certificate.json", optional: true, reloadOnChange: true)
        //        .AddJsonFile($"certificate.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
        //        .Build();

        //    var certificateSettings = config.GetSection("certificateSettings");
        //    string certificateFileName = certificateSettings.GetValue<string>("filename");
        //    string certificatePassword = certificateSettings.GetValue<string>("password");

        //    var certificate = new X509Certificate2(certificateFileName, certificatePassword);

        //    var host = new WebHostBuilder()
        //        .UseKestrel(
        //            options =>
        //            {
        //                options.AddServerHeader = false;
        //                options.Listen(IPAddress.Loopback, 44346, listenOptions =>
        //                {
        //                    listenOptions.UseHttps(certificate);
        //                });
        //            }
        //        )
        //        .UseIISIntegration()
        //        .UseConfiguration(config)
        //        .UseContentRoot(Directory.GetCurrentDirectory())
        //        .UseStartup<Startup>()
        //        .UseUrls("https://localhost:44346/")
        //        .Build();

        //    host.Run();
        //}

    }
}
