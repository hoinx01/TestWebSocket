using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Startup.Controllers
{
    [Route("deals")]
    public class DealController : ControllerBase
    {
        [Route("{id}")]
        public async Task Process([FromRoute] int id)
        {
            var socket = (WebSocket)HttpContext.Items["WebSocket"];
            var receivedData = (WebSocketReceiveResult)HttpContext.Items["WebSocketReceiveResult"];
            var buffer = (byte[])HttpContext.Items["Buffer"];
            string data = Encoding.ASCII.GetString(buffer, 0, receivedData.Count);



            var outBuffer = Encoding.ASCII.GetBytes(data);
            await socket.SendAsync(new ArraySegment<byte>(outBuffer, 0, outBuffer.Length),receivedData.MessageType, true, CancellationToken.None);
        }
    }
}
