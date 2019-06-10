using FrontCenter.Models;
using FrontCenter.Models.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    public class Method
    {
        public static IHostingEnvironment _hostingEnvironment { get; set; }

        public static IConfigurationRoot Configuration { get; set; }

        public static ContextString _context { get; set; }

        //服务器地址
        public static string ServerAddr { get; set; }

        //数据库字符串
        public static string ContextStr { get; set; }

        public static string OSSServer { get; set; }

        public static string MallSite { get; set; }

        public static string ServerMac { get; set; }

        //客户唯一代码
        public static string CusID { get; set; }



        /// <summary>
        /// 判断服务器是否有此用户
        /// </summary>
        /// <param name="dbContext">数据库连接</param>
        /// <param name="_AccountName">用户名</param>
        /// <returns></returns>
        public static bool FindAllByName(ContextString dbContext, string _AccountName)
        {
            bool _R = false;
            if (dbContext.Account.Where(i => i.AccountName == _AccountName && i.Activity).Count() > 0)
            {
                _R = true;
            }
            return _R;
        }

        public static string GetServerMac()
        {
            var networks = NetworkInterface.GetAllNetworkInterfaces();
            string macAddressStr = string.Empty;
            foreach (var network in networks)
            {
                if (network.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    var physicalAddress = network.GetPhysicalAddress();
                    macAddressStr = string.Join(":", physicalAddress.GetAddressBytes().Select(b => b.ToString("X2")));
                    break;
                }
            }
            return macAddressStr;
        }

        /// <summary>
        /// 通过ID获取用户信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="_AccountID"></param>
        /// <returns></returns>

        public async static Task<Account> GetUserByID(ContextString dbContext, int _AccountID)
        {
            return await dbContext.Account.SingleOrDefaultAsync(i => i.ID == _AccountID);
        }

        public static string GetRandomStr(int length, ContextString dbContext)
        {


            Random rd = new Random();
            string str = "abcdefghijklmnopqrstuvwxyz0123456789";
            bool isnewstr = true;
            string result = string.Empty;

            do
            {
                result = string.Empty;
                for (int i = 0; i < length; i++)
                {
                    result += str[rd.Next(str.Length)];
                }

                var count = dbContext.RandomStr.Where(i => i.Str == result).Count();

                if (count <= 0)
                {
                    dbContext.RandomStr.Add(new RandomStr
                    {
                        AddTime = DateTime.Now,
                        Code = Guid.NewGuid().ToString(),
                        Str = result,
                        UpdateTime = DateTime.Now
                    });

                    if (dbContext.SaveChanges() > 0)
                    {
                        isnewstr = false;
                    }


                }
            } while (isnewstr);
            return result;
        }

        /// <summary>
        /// 创建一个用户
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="_Account"></param>
        /// <returns></returns>
        public async static Task<int> CreateAccount(ContextString dbContext, Account _Account)
        {
            dbContext.Account.Add(_Account);
            await dbContext.SaveChangesAsync();
            return _Account.ID;
        }

        #region 生成随机数字
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        public static string Number(int Length)
        {
            return Number(Length, false);
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string Number(int Length, bool Sleep)
        {
            if (Sleep) System.Threading.Thread.Sleep(3);
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }
        #endregion

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
        /// DateTime时间格式转换为Unix时间戳格式    
        /// </summary>    
        /// <param name="time"> DateTime时间格式</param>    
        /// <returns>Unix时间戳格式</returns>    
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);//等价的建议写法
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// 判断字符串是否为正整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[0-9]\d*$");
            return reg1.IsMatch(str);
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

        public static string PostMoths(string url, string param)
        {
            try
            {
                string strURL = url;
                System.Net.HttpWebRequest request;
                request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=UTF-8";
                string paraUrlCoded = param;
                byte[] payload;
                payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
                request.ContentLength = payload.Length;
                Stream writer = request.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
                System.Net.HttpWebResponse response;
                response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.Stream s;
                s = response.GetResponseStream();
                string StrDate = "";
                string strValue = "";
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                while ((StrDate = Reader.ReadLine()) != null)
                {
                    strValue += StrDate + "\r\n";
                }
                return strValue;

            }
            catch (Exception e)
            {
                return e.ToString();

            }


        }


        public static async Task<bool> StartDownTask()
        {
            return await HttpDldFile.DownTask();
        }
    }
}
