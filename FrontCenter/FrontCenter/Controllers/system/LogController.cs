using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrontCenter.AppCode;
using FrontCenter.Models;
using FrontCenter.Models.Data;
using FrontCenter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontCenter.Controllers.system
{
    public class LogController : Controller
    {

        /// <summary>
        /// 查询店铺列表
        /// </summary>
        /// <param name="ShopID"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryLogList(Input_LogQuery model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            //Stream stream = HttpContext.Request.Body;
            //byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            //stream.Read(buffer, 0, buffer.Length);
            //string inputStr = Encoding.UTF8.GetString(buffer);
            //model = (Input_ShopCond)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            DateTime _BeginTime = new DateTime();
            DateTime _EndTime = new DateTime();

            //检测用户输入格式

            if (string.IsNullOrEmpty(model.BeginTime))
            {
                _BeginTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd ") + "  00:00:00");
            }
            else
            {
                try
                {
                    _BeginTime = Convert.ToDateTime(Convert.ToDateTime(model.BeginTime).ToString("yyyy-MM-dd ") + "  00:00:00");
                }
                catch (Exception)
                {
                    var _BT = Method.GMT2Local(model.BeginTime);
                    _BeginTime = Convert.ToDateTime(Convert.ToDateTime(_BT).ToString("yyyy-MM-dd ") + "  00:00:00");
                }

            }
            if (string.IsNullOrEmpty(model.EndTime))
            {
                _EndTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd ") + "  00:00:00").AddDays(1).AddMilliseconds(-1);
            }
            else
            {
                try
                {
                    _EndTime = Convert.ToDateTime(Convert.ToDateTime(model.EndTime).ToString("yyyy-MM-dd ") + "  00:00:00").AddDays(1).AddMilliseconds(-1);
                }
                catch (Exception)
                {
                    var _ED = Method.GMT2Local(model.EndTime);
                    _EndTime = Convert.ToDateTime(Convert.ToDateTime(_ED).ToString("yyyy-MM-dd ") + "  00:00:00").AddDays(1).AddMilliseconds(-1);
                }

            }


            var loglist = await dbContext.SysLog.Where(i => i.MallCode == "" && i.SystemModule == "Manage" && i.AddTime >= _BeginTime && i.AddTime <= _EndTime).Select(s => new {
                s.AccountName,
                AddTime = s.AddTime.ToString("yyyy-MM-dd HH:mm:ss"),
                s.Code,
                s.ID,
                IP = s.IP == "::1" ? "127.0.0.1" : s.IP,
                s.LogMsg,
                s.ModuleName,
                s.Type


            }).ToListAsync();

            //模块名称过滤
            if (!string.IsNullOrEmpty(model.Keywords))
            {

                loglist = loglist.Where(i => i.ModuleName.Contains(model.Keywords) || i.AccountName.Contains(model.Keywords) || i.LogMsg.Contains(model.Keywords)).ToList();
            }
            loglist = loglist.OrderByDescending(O => O.AddTime).ToList();
            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = loglist;

            return Json(_Result);
        }

        /// <summary>
        /// 查询日志信息
        /// </summary>
        /// <param name="model">查询条件</param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        //[HttpPost]
        public async Task<IActionResult> QueryLogPageList(Input_LogPageQuery model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            //检测用户登录情况
            UserOnLine user = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (user == null || string.IsNullOrEmpty(user.UserCode) || (model.Type.HasValue && string.IsNullOrEmpty(user.MallCode)))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }

            DateTime _BeginTime = new DateTime();
            DateTime _EndTime = new DateTime();

            //检测用户输入格式

            if (string.IsNullOrEmpty(model.BeginTime))
            {
                _BeginTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd ") + "  00:00:00");
            }
            else
            {
                try
                {
                    _BeginTime = Convert.ToDateTime(Convert.ToDateTime(model.BeginTime).ToString("yyyy-MM-dd ") + "  00:00:00");
                }
                catch (Exception)
                {
                    var _BT = Method.GMT2Local(model.BeginTime);
                    _BeginTime = Convert.ToDateTime(Convert.ToDateTime(_BT).ToString("yyyy-MM-dd ") + "  00:00:00");
                }

            }
            if (string.IsNullOrEmpty(model.EndTime))
            {
                _EndTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd ") + "  00:00:00").AddDays(1).AddMilliseconds(-1);
            }
            else
            {
                try
                {
                    _EndTime = Convert.ToDateTime(Convert.ToDateTime(model.EndTime).ToString("yyyy-MM-dd ") + "  00:00:00").AddDays(1).AddMilliseconds(-1);
                }
                catch (Exception)
                {
                    var _ED = Method.GMT2Local(model.EndTime);
                    _EndTime = Convert.ToDateTime(Convert.ToDateTime(_ED).ToString("yyyy-MM-dd ") + "  00:00:00").AddDays(1).AddMilliseconds(-1);
                }

            }
            if (_BeginTime > _EndTime)
            {
                _Result.Code = "510";
                _Result.Msg = "开始时间不可大于结束时间";
                _Result.Data = "";
                return Json(_Result);
            }

            var loglist = await dbContext.SysLog.Where(i => ((i.MallCode == "" && i.SystemModule == "Manage" && !model.Type.HasValue) || (i.MallCode == user.MallCode && i.SystemModule == "Mall" && model.Type.HasValue)) && i.AddTime >= _BeginTime && i.AddTime <= _EndTime).Select(s => new {
                s.AccountName,
                AddTime = s.AddTime.ToString("yyyy-MM-dd HH:mm:ss"),
                s.Code,
                s.ID,
                IP = s.IP == "::1" ? "127.0.0.1" : s.IP,
                s.LogMsg,
                s.ModuleName,
                s.Type


            }).ToListAsync();

            //模块名称过滤
            if (!string.IsNullOrEmpty(model.Keywords))
            {

                loglist = loglist.Where(i => i.ModuleName.Contains(model.Keywords) || i.AccountName.Contains(model.Keywords) || i.LogMsg.Contains(model.Keywords)).ToList();
            }
            loglist = loglist.OrderByDescending(O => O.AddTime).ToList();
            int allPage = 1;
            var allCount = loglist.Count();
            if (model.Paging == 1)
            {
                if (model.PageIndex == null)
                {
                    model.PageIndex = 1;
                }
                if (model.PageSize == null)
                {
                    model.PageSize = 10;
                }
                allPage = (int)(allCount / model.PageSize);
                if (allCount % model.PageSize > 0)
                {
                    allPage = allPage + 1;
                }
                loglist = loglist.Skip(((int)model.PageIndex - 1) * (int)model.PageSize).Take((int)model.PageSize).ToList();

            }
            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = new { List = loglist, AllPage = allPage, AllCount = allCount };
            return Json(_Result);
        }
    }
}