//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>The program class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api
{
    using System;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Https;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    bool.TryParse(Environment.GetEnvironmentVariable("IsDockerDeployment"), out bool isDockerDeployment);
                    webBuilder.UseKestrel(options =>
                             {
                                 if (isDockerDeployment)
                                 {
                                     options.Listen(new IPEndPoint(IPAddress.Any, 443), listenOptions =>
                                     {
                                         var configuration = (IConfiguration)options.ApplicationServices.GetService(typeof(IConfiguration));
                                         var certPassword = Environment.GetEnvironmentVariable("KestrelPassword");
                                         var certPath = Environment.GetEnvironmentVariable("KestrelPath");
                                         Console.WriteLine(certPassword);
                                         Console.WriteLine(certPath);

                                         var certificate = new X509Certificate2(certPath, certPassword);
                                         Console.WriteLine("Certificate provided");
                                         var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions()
                                         {
                                             ClientCertificateMode = ClientCertificateMode.NoCertificate,
                                             SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                                             ServerCertificate = certificate,
                                         };
                                         listenOptions.UseHttps(httpsConnectionAdapterOptions);
                                     });
                                 }
                             });
                    if (!isDockerDeployment)
                    {
                        webBuilder.UseIIS();
                    }
                    webBuilder.UseStartup<Startup>();
                });
    }
}