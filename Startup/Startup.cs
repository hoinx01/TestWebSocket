using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using SrvCornet.Core;
using SrvCornet.Http;
using Startup.Components;
using Startup.Middleware;

namespace Iken.Startup
{
    public class Startup
    {
        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }
        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            HostingEnvironment = env;
            Configuration = config;
            AppSettings.SetConfig(config);
        }

        // Use this method to add services to the container.
        //optional
        public void ConfigureServices(IServiceCollection services)
        {
            //Đăng kí cho automapper
            AutoMapperInitiator.Init();
            //Đăng kí cho mvc, có thằng này mới viết controller mvc được
            services.AddMvc(options => {
                options.Filters.Add(new CustomApiExceptionFilter());
            });
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
            });
            
            //Đăng kí dependency
            DependencyRegister.Register(services);
            //Thiết lập 1 static instance của ServiceProvider để có thể gọi resolve 1 dependency instance 1 cách linh hoạt
            DependencyManager.SetServiceProvider(services.BuildServiceProvider());
        }

        // Use this method to configure the HTTP request pipeline.
        //require
        public void Configure(IApplicationBuilder app)
        {
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);
            app.UseMiddleware<SocketMiddleware>();
            app.UseMvc();

            

            //app.Use(async (context, next) =>
            //{
            //    if (context.Request.Path.StartsWithSegments("/ws"))
            //    {
            //        if (context.WebSockets.IsWebSocketRequest)
            //        {
            //            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            //            await Echo(context, webSocket);
            //        }
            //        else
            //        {
            //            context.Response.StatusCode = 400;
            //        }
            //    }
            //    else
            //    {
            //        await next();
            //    }

            //});
        }
        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            try
            {
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            catch(Exception e)
            {
                throw e;
            }
            
        }
    }
}
