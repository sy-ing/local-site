using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    public class ChatWebSocketMiddleware
    {
        public static ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> _sockets = new ConcurrentDictionary<string, System.Net.WebSockets.WebSocket>();

        private readonly RequestDelegate _next;

        public ChatWebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //判断是否是WebSockets请求
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            //线程标志
            CancellationToken ct = context.RequestAborted;

            //当前连接
            var currentSocket = await context.WebSockets.AcceptWebSocketAsync();
            QMLog qMLog = new QMLog();
            qMLog.WriteLogToFile("socketId", context.Request.Query["Code"]);
            var socketId = context.Request.Query["Code"];

            if (string.IsNullOrEmpty(socketId))
            {
                return;
            }
            //获取客户端IP
            // var remoteIpAddress = context.Request.HttpContext.Connection.RemoteIpAddress;

            //将客户端IP作为标识符
            // string socketId = remoteIpAddress.ToString();

            //var ip = context.Request.Headers["X_REAL_IP"].FirstOrDefault();
            //if (string.IsNullOrEmpty(ip))
            //{
            //    ip = context.Connection.RemoteIpAddress.ToString();
            //}


            //将客户端IP作为标识符
            //string socketId = ip;

          

            //判断是否为已有连接，不是的话创建
            if (!_sockets.ContainsKey(socketId))
            {
                _sockets.TryAdd(socketId, currentSocket);
            }
            else
            {
                WebSocket socket;
                _sockets.TryGetValue(socketId, out socket);
                if (_sockets.TryRemove(socketId, out socket))
                {
                    qMLog.WriteLogToFile("添加socketId", context.Request.Query["Code"]);
                    _sockets.TryAdd(socketId, currentSocket);
                }
            }


            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                string response = await ReceiveStringAsync(currentSocket, ct);

                if (response == null)
                {
                    break;
                }

                //MsgTemplate msg = JsonConvert.DeserializeObject<MsgTemplate>(response);

                if (string.IsNullOrEmpty(response))
                {
                    if (currentSocket.State != WebSocketState.Open)
                    {
                        break;
                    }

                    continue;
                }
                /*
                foreach (var socket in _sockets)
                {
                    if (socket.Value.State != WebSocketState.Open)
                    {
                        continue;
                    }
                     // 控制只有接收者才能收到消息
                    if (socket.Key == msg.ReceiverID || socket.Key == socketId)
                    {
                        await SendStringAsync(socket.Value, JsonConvert.SerializeObject(msg), ct);
                    }
                }
                */
            }

            //_sockets.TryRemove(socketId, out dummy);

            await currentSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
            currentSocket.Dispose();
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static Task SendStringAsync(System.Net.WebSockets.WebSocket socket, string data, CancellationToken ct = default(CancellationToken))
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }


        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private static async Task<string> ReceiveStringAsync(System.Net.WebSockets.WebSocket socket, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                var buffer = new ArraySegment<byte>(new byte[1024]);
                using (var ms = new MemoryStream())
                {
                    WebSocketReceiveResult result;
                    do
                    {

                        ct.ThrowIfCancellationRequested();

                        result = await socket.ReceiveAsync(buffer, ct);



                        ms.Write(buffer.Array, buffer.Offset, result.Count);


                    }
                    while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);
                    if (result.MessageType != WebSocketMessageType.Text)
                    {
                        return null;
                    }

                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {

                        return await reader.ReadToEndAsync();
                    }
                }

            }
            catch (Exception)
            {
                QMLog qm = new QMLog();
                qm.WriteLogToFile("", "WebSocket断开");
                return null;
            }
        }




    }
}
