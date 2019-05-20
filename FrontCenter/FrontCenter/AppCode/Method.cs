using FrontCenter.Models;
using FrontCenter.Models.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    public class Method
    {
        public static IHostingEnvironment _hostingEnvironment { get; set; }

        //数据库字符串
        public static string ContextStr { get; set; }

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
    }
}
