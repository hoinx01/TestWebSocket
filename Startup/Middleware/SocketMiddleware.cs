using Microsoft.AspNetCore.Http;
using SrvCornet.Core;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Startup.Middleware
{
    public class SocketMiddleware
    {
        private readonly RequestDelegate next;
        private readonly int bufferMaxSizeInKilobyte = AppSettings.GetInt32("Socket:BufferMaxSizeInKilobyte");
        public SocketMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            //if (!context.Request.Path.StartsWithSegments("/ws/"))
            //{
            //    await next.Invoke(context);
            //}
            if (!context.WebSockets.IsWebSocketRequest)
            {
                //context.Response.StatusCode = 400;
                await next(context);
            }
            else
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                byte[] buffer = new byte[bufferMaxSizeInKilobyte];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    context.Items.Add("WebSocket", webSocket);
                    context.Items.Add("WebSocketReceiveResult", result);
                    context.Items.Add("Buffer", buffer);
                    await next(context);
                    
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            
        }
    }
}
