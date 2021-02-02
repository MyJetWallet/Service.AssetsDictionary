using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Service.AssetsDictionary.SDK;
using Service.AssetsDictionary.Settings;
using SimpleTrading.SettingsReader;

namespace Service.AssetsDictionary
{
    public class Program
    {
        public const string SettingsFileName = ".myjetwallet";

        public static SettingsModel Settings { get; private set; }

        public static void Main(string[] args)
        {
            Console.Title = "MyJetWallet Service.AssetsDictionary";

            Settings = SettingsReader.ReadSettings<SettingsModel>(SettingsFileName);

            using var loggerFactory = LogConfigurator.Configure("MyJetWallet", Settings.SeqServiceUrl);

            var logger = loggerFactory.CreateLogger<Program>();

            try
            {
                logger.LogInformation("Application is being started");

                CreateHostBuilder(loggerFactory, args).Build().Run();

                logger.LogInformation("Application has been stopped");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Application has been terminated unexpectedly");
            }
        }

        public static IHostBuilder CreateHostBuilder(ILoggerFactory loggerFactory, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, 5000, o => o.Protocols = HttpProtocols.Http1);
                        options.Listen(IPAddress.Any, 5001, o => o.Protocols = HttpProtocols.Http2);
                    });

                    webBuilder.UseUrls("http://*:5000", "http://*:5001");

                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton(loggerFactory);
                    services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
                });
    }
}
