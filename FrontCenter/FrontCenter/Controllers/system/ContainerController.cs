using System;
using System.Collections;
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
    public class ContainerController : Controller
    {
        public async Task<IActionResult> GetContainerList([FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (string.IsNullOrEmpty(uol.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }

            try
            {
                var screenInfo = await dbContext.ScreenInfo.Where(i => i.MallCode == uol.MallCode).OrderBy(i => i.ID).ToListAsync();
                var list = new ArrayList();
                foreach (var item in screenInfo)
                {
                    var container = await dbContext.ContainerBG.Where(i => i.MallCode == uol.MallCode && i.ScreenCode == item.Code).Join(dbContext.AssetFiles, c => c.FileCode, af => af.Code, (c, af) => new
                    {
                        FilePath = Method.OSSServer + af.FilePath,
                        c.FileCode,
                        c.ScreenCode
                    }).FirstOrDefaultAsync();

                    list.Add(new { item.Code, item.SName, FilePath = container == null ? "" : container.FilePath, FileCode = container == null ? "" : container.FileCode, ScreenCode = item.Code });
                }
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = list;
                return Json(_Result);
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "" + e.ToString();
                _Result.Data = "";
                return Json(_Result);

            }
        }

        [HttpPost]
        public async Task<IActionResult> SetContainerBG(Input_ContainerSet model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_ContainerSet)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (string.IsNullOrEmpty(uol.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }
            try
            {
                //检测用户登录情况
                //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
                //if (string.IsNullOrEmpty(username))
                //{
                //    _Result.Code = "401";
                //    _Result.Msg = "请登陆后再进行操作";
                //    _Result.Data = "";
                //    return Json(_Result);
                //}
                var screenCount = await dbContext.ScreenInfo.Where(i => i.Code == model.ScreenCode).CountAsync();
                if (screenCount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的分辨率编码：" + model.ScreenCode;
                    _Result.Data = "";
                    return Json(_Result);
                }

                var files = await dbContext.AssetFiles.Where(i => i.Code == model.FileCode).FirstOrDefaultAsync();
                if (files == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的文件编码：" + model.FileCode;
                    _Result.Data = "";
                    return Json(_Result);
                }

                var container = await dbContext.ContainerBG.Where(i => i.ScreenCode == model.ScreenCode).FirstOrDefaultAsync();
                if (container == null)
                {
                    container = new ContainerBG();
                    container.AddTime = DateTime.Now;
                    container.Code = Guid.NewGuid().ToString();
                    container.FileCode = model.FileCode;
                    container.ScreenCode = model.ScreenCode;
                    container.UpdateTime = DateTime.Now;
                    container.MallCode = uol.MallCode;
                    dbContext.ContainerBG.Add(container);
                }
                else
                {
                    container.FileCode = model.FileCode;
                    container.UpdateTime = DateTime.Now;
                    dbContext.ContainerBG.Update(container);
                }
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    var deviceList = await dbContext.Device.Where(i => i.ScreenInfo == model.ScreenCode).ToListAsync();
                    if (deviceList.Count() > 0)
                    {
                        foreach (var item in deviceList)
                        {
                            MsgTemplate msg = new MsgTemplate();
                            msg.SenderID = Method.ServerAddr;
                            msg.ReceiverID = item.Code;
                            msg.MessageType = "json";
                            msg.Content = new { Type = "BGSet", FilePath = Method.OSSServer + files.FilePath };
                            await Method.SendMsgAsync(msg);
                        }
                    }
                    _Result.Code = "200";
                    _Result.Msg = "设置成功";
                    _Result.Data = "";
                }
                else
                {
                    _Result.Code = "501";
                    _Result.Msg = "设置失败";
                    _Result.Data = "";
                }
                return Json(_Result);
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "" + e.ToString();
                _Result.Data = "";
                return Json(_Result);

            }
        }
        [HttpPost]
        public async Task<IActionResult> GetContainerBGByScreen(Input_GetContainerBGByScreen model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetContainerBGByScreen)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            _Result.Code = "200";
            _Result.Msg = "获取成功";
            var device = await dbContext.Device.Where(i => i.Code == model.Code).FirstOrDefaultAsync();
            if (device == null)
            {
                _Result.Data = "";
            }
            else
            {
                var container = await dbContext.ContainerBG.Where(i => i.ScreenCode == device.ScreenInfo).Join(dbContext.AssetFiles, c => c.FileCode, af => af.Code, (c, af) => new
                {
                    FilePath = Method.OSSServer + af.FilePath
                }).FirstOrDefaultAsync();

                if (container == null)
                {
                    _Result.Data = "";
                }
                else
                {
                    _Result.Data = container.FilePath;
                }
            }
            return Json(_Result);
        }


    }
}