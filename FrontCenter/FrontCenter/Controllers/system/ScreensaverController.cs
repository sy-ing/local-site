using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrontCenter.AppCode;
using FrontCenter.Models;
using FrontCenter.Models.Data;
using FrontCenter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontCenter.Controllers.system
{
    public class ScreensaverController : Controller
    {
        /// <summary>
        /// 设置屏保时间
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SetScreensaver(Input_Screensaver model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_Screensaver)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (string.IsNullOrEmpty(uol.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }
            //var mall = await dbContext.Mall.Where(i => i.Code == uol.MallCode).FirstOrDefaultAsync();

            //if (mall == null)
            //{
            //    _Result.Code = "510";
            //    _Result.Msg = "无效的商场编码";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}

            //数据验证
            if (model.Time == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的时间";
                _Result.Data = "";

                return Json(_Result);
            }

            //获取当前时间
            var screensaver = await dbContext.Screensaver.Where(i => i.MallCode == uol.MallCode).FirstOrDefaultAsync();

            //更改时间
            if (screensaver == null)
            {
                dbContext.Screensaver.Add(new Screensaver { MallCode = uol.MallCode, Time = (int)model.Time, ScreenType = (int)model.ScreenType });
            }
            else
            {
                screensaver.Time = (int)model.Time;
                screensaver.ScreenType = model.ScreenType.HasValue ? model.ScreenType.Value : 0;
                dbContext.Screensaver.Update(screensaver);
            }


            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "设置成功";
                _Result.Data = "";
                var deviceList = await dbContext.Device.Where(i => i.MallCode == uol.MallCode && !i.IsDelete).ToListAsync();
                if (deviceList.Count > 0)
                {
                    foreach (var item in deviceList)
                    {
                        MsgTemplate msg = new MsgTemplate();
                        msg.SenderID = Method.ServerAddr;
                        msg.ReceiverID = item.Code;
                        msg.MessageType = "json";
                        msg.Content = new { Type = "Screensaver", model.Time };
                        await Method.SendMsgAsync(msg);
                    }
                }
            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "设置失败";
                _Result.Data = "";
            }



            return Json(_Result);
        }


        /// <summary>
        /// 获取屏保时间
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetScreensaver(Input_GetDeviceOptionsNew model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetDeviceOptionsNew)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            if (model == null || string.IsNullOrEmpty(model.MallCode))
            {
                var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
                if (string.IsNullOrEmpty(uol.UserName))
                {
                    _Result.Code = "401";
                    _Result.Msg = "请登陆后再进行操作";
                    _Result.Data = "";
                    return Json(_Result);
                }
                else
                {
                    model.MallCode = uol.MallCode;
                }
            }
            //else
            //{
            //    var mall = await dbContext.Mall.Where(i => i.Code == model.MallCode).FirstOrDefaultAsync();

            //    if (mall == null)
            //    {
            //        _Result.Code = "510";
            //        _Result.Msg = "无效的商场编码";
            //        _Result.Data = "";
            //        return Json(_Result);
            //    }
            //}

            //获取当前时间
            var screensaver = await dbContext.Screensaver.Where(i => i.MallCode == model.MallCode).FirstOrDefaultAsync();


            if (screensaver == null)
            {
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { Time = 30, ScreenType = 0 };
            }
            else
            {
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { Time = screensaver.Time, ScreenType = screensaver.ScreenType.HasValue ? screensaver.ScreenType.Value : 0 };
            }

            return Json(_Result);
        }
    }
}