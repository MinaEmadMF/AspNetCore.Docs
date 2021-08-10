using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Http3Sample
{
    public class Program
    {
        #region snippet_UseHttp3
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseKestrel()
            // Set up Quic options
            .UseQuic(options =>
            {
                options.Alpn = "h3-29";
                options.IdleTimeout = TimeSpan.FromHours(1);
            })
            .ConfigureKestrel((context, options) =>
            {
                options.EnableAltSvc = true;
                options.Listen(IPAddress.Any, 5001, listenOptions =>
                {
                    // Use Http3
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                    listenOptions.UseHttps();
                });
            });
            #endregion
            await using var app = builder.Build();

            app.MapGet("/", (Func<string>)(() => "Hello World!"));

            await app.RunAsync();
        }
    }
}