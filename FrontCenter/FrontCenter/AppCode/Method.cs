using FrontCenter.Models;
using FrontCenter.Models.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    public class Method
    {
        public static IHostingEnvironment _hostingEnvironment { get; set; }

        //服务器地址
        public static string ServerAddr { get; set; }

        //数据库字符串
        public static string ContextStr { get; set; }

        public static string OSSServer { get; set; }

        /// <summary>
        /// 获取当前用户用户名
        /// </summary>
        /// <returns></returns>
        public static UserOnLine GetLoginUserName(ContextString dbContext, HttpContext context)
        {
            UserOnLine uol = new UserOnLine();
            int? _ID = context.Session.GetInt32("AccountID");
            if (_ID == null)
            {
                return uol;
            }
            var _User = dbContext.Account.Where(i => i.ID == _ID).FirstOrDefault();
            if (_User == null)
            {
                return uol;
            }

            var mallcode = context.Session.GetString("MallCode");
            uol.UserName = _User.AccountName;
            uol.UserCode = _User.Code;
            uol.MallCode = mallcode;
            uol.ID = _ID.Value;
            uol.SystemModule = _User.SystemModule;
            return uol;
        }



        /// <summary>
        /// 获取用户IP
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUserIp(HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            //   var ip = context.Request.Headers["X-Original-For"].FirstOrDefault();
            QMLog qm = new QMLog();
            qm.WriteLogToFile("", ip);
            qm.WriteLogToFile("", context.Connection.RemoteIpAddress.ToString());
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }

        /// <summary>
        /// 字符串进行PBKDF2加密
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>

        public static string StringToPBKDF2Hash(string inputStr)
        {
            byte[] salt = new byte[128 / 8];

            string outString = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                     password: inputStr,//明文
                                                     salt: salt,//盐
                                                     prf: KeyDerivationPrf.HMACSHA1,//加密方式
                                                     iterationCount: 10000,//迭代次数
                                                     numBytesRequested: 256 / 8));

            return outString;
        }


        /// <summary>
        /// 发送websocket消息
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> SendMsgAsync(MsgTemplate msg)
        {

            bool _Result = false;
            try
            {
                if (!ChatWebSocketMiddleware._sockets.ContainsKey(msg.ReceiverID))
                {
                    //不存在接收端连接
                    return _Result;
                }
                var socket = ChatWebSocketMiddleware._sockets.Where(i => i.Key == msg.ReceiverID).FirstOrDefault();

                if (socket.Value.State != WebSocketState.Open)
                {
                    //接收端已断开
                    return _Result;
                }
                // 控制只有接收者才能收到消息

                await ChatWebSocketMiddleware.SendStringAsync(socket.Value, JsonConvert.SerializeObject(msg), CancellationToken.None);
                _Result = true;
            }
            catch
            {

                throw;
            }

            return _Result;
        }




        /// <summary>    
        /// GMT时间转成本地时间   
        /// DateTime dt1 = GMT2Local("Thu, 29 Sep 2014 07:04:39 GMT");  
        /// 转换后的dt1为：2014-9-29 15:04:39  
        /// DateTime dt2 = GMT2Local("Thu, 29 Sep 2014 15:04:39 GMT+0800");  
        /// 转换后的dt2为：2014-9-29 15:04:39  
        /// </summary>    
        /// <param name="gmt">字符串形式的GMT时间</param>    
        /// <returns></returns>    
        public static DateTime GMT2Local(string gmt)
        {
            DateTime dt;
            //gmt = gmt.Replace("GMT+0800", "").TrimEnd();
            //gmt = gmt.Replace("GMT 0800", "").TrimEnd();


            // gmt = gmt.Split("GMT")[0].TrimEnd();

            gmt = gmt.Substring(0, gmt.IndexOf("GMT")).TrimEnd();

            string format = "ddd MMM dd yyyy HH:mm:ss";
            DateTime.TryParseExact(gmt, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
            return dt;
        }
    }
}
