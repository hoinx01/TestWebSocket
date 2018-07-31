using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Iken.Startup
{
    public static class WebhostInitiator
    {
        public static void RunWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .Build();

            IWebHost webHost = WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                //.ConfigureAppConfiguration((webHostBuilderContext, configurationbuilder) =>
                //{
                //    configurationbuilder
                //            .AddJsonFile("appsettings.json", optional: true);
                //    configurationbuilder.AddEnvironmentVariables();
                //})
                .UseStartup<Startup>()
                //.UseKestrel(options =>
                //{
                //    //options.Listen(IPAddress.Loopback, 5000);

                //})
                .UseKestrel()
                .Build();
            webHost.Run();
        }
    }
}
