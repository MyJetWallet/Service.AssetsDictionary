using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Routing;
using MyJetWallet.Sdk.Service;
using Prometheus;
using ProtoBuf.Grpc.Reflection;
using ProtoBuf.Grpc.Server;
using Service.AssetsDictionary.Grpc;
using Service.AssetsDictionary.Modules;
using Service.AssetsDictionary.Services;
using Service.AssetsDictionary.Settings;
using SimpleTrading.BaseMetrics;
using SimpleTrading.ServiceStatusReporterConnector;
using SimpleTrading.SettingsReader;

namespace Service.AssetsDictionary
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCodeFirstGrpc(options =>
            {
                options.Interceptors.Add<PrometheusMetricsInterceptor>();
                options.BindMetricsInterceptors();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseMetricServer();

            app.BindServicesTree(Assembly.GetExecutingAssembly());

            app.BindIsAlive();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<AssetsDictionaryService>();
                endpoints.MapGrpcService<SpotInstrumentsDictionaryService>();

                endpoints.MapGrpcSchemaRegistry();
                endpoints.MapGrpcSchema<IAssetsDictionaryService>();
                endpoints.MapGrpcSchema<ISpotInstrumentsDictionaryService>();

                endpoints.MapGet("/api/isalive", async context =>
                {
                    await context.Response.WriteAsync(IsAliveResponse.IsAlive());
                });
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule(new MyNoSqlModule(() => GetSettings().MyNoSqlWriterUrl));
        }

        private SettingsModel GetSettings()
        {
            return SettingsReader.ReadSettings<SettingsModel>(Program.SettingsFileName);
        }
    }


    public static class ProtoGeneratorHelper
    {
        private static readonly Dictionary<string, string> Map = new Dictionary<string, string>();

        public static void MapGrpcSchema<TService>(this IEndpointRouteBuilder endpoints)
        {
            var service = typeof(TService);
            var generator = new SchemaGenerator();
            var schema = generator.GetSchema(service);
            Map[service.Name] = schema;

            endpoints.Map($"/grpc/{service.Name}.proto",
                async context =>
                {
                    await context.Response.WriteAsync(schema);
                });

        }

        public static void MapGrpcSchemaRegistry(this IEndpointRouteBuilder endpoints)
        {
            using var reader = new StreamReader("bcl.proto");
            var bcl = reader.ReadToEnd();

            endpoints.Map("/grpc",
                async context =>
                {
                    await context.Response.WriteAsync(GetListOfServices());
                });

            endpoints.Map("/grpc/protobuf-net/bcl.proto", async context =>
            {
                await context.Response.WriteAsync(bcl);
            });
        }

        private static string GetListOfServices()
        {
            var builder = new StringBuilder();

            builder.AppendLine("<html>");
            builder.AppendLine("<body>");

            builder.AppendLine("<p>List of proto files</p>");

            builder.AppendLine("<ul>");

            builder.AppendLine("<li><a href='/grpc/protobuf-net/bcl.proto'>/grpc/protobuf-net/bcl.proto</a><br></li>");

            foreach (var key in Map.Keys)
            {
                builder.AppendLine($"<li><a href='/grpc/{key}.proto'>/grpc/{key}.proto</a><br></li>");
            }

            builder.AppendLine("</ul>");

            builder.AppendLine("</body>");
            builder.AppendLine("</html>");

            return builder.ToString();
        }
    }


    
}
