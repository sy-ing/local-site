using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace FrontCenter.Controllers.app
{
    public class ApplicationController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> GetClassList(Input_GetClassList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetClassList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (string.IsNullOrEmpty(uol.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }
            model.AppClassCode = string.IsNullOrEmpty(model.AppClassCode) ? "" : model.AppClassCode;
            var list = await dbContext.AppClassNew.Where(i => i.ParentCode == model.AppClassCode && !i.IsDel).OrderBy(i => i.Order).ToListAsync();
            _Result.Data = list;
            _Result.Code = "200";
            _Result.Msg = "获取成功";
            return Json(_Result);
        }

        /// <summary>
        /// 应用添加
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(Input_AppInfoNew model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_AppInfoNew)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
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
                var screenCount = await dbContext.ScreenInfo.Where(i => i.Code == model.ScreenInfo).CountAsync();
                if (screenCount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的分辨率编码：" + model.ScreenInfo;
                    _Result.Data = "";
                    return Json(_Result);
                }

                var file = await dbContext.AssetFiles.Where(i => i.Code == model.FileCode).FirstOrDefaultAsync();
                if (file == null)
                {

                    _Result.Code = "510";
                    _Result.Msg = "无效的文件编码：" + model.FileCode;
                    _Result.Data = "";
                    return Json(_Result);
                }

                var iconFile = await dbContext.AssetFiles.Where(i => i.Code == model.IconFileCode).FirstOrDefaultAsync();
                if (iconFile == null)
                {

                    _Result.Code = "510";
                    _Result.Msg = "无效的应用图标编码：" + model.IconFileCode;
                    _Result.Data = "";
                    return Json(_Result);
                }

                var appClass = await dbContext.AppClassNew.Where(i => i.Code == model.AppClass).FirstOrDefaultAsync();
                if (appClass == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的应用分类编码：" + model.AppClass;
                    _Result.Data = "";
                    return Json(_Result);
                }

                var appClassSec = await dbContext.AppClassNew.Where(i => i.Code == model.AppSecClass).FirstOrDefaultAsync();
                if (appClassSec == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的二级应用分类编码：" + model.AppSecClass;
                    _Result.Data = "";
                    return Json(_Result);
                }

                if (string.IsNullOrEmpty(model.Name))
                {
                    _Result.Code = "510";
                    _Result.Msg = "应用名称不能为空";
                    _Result.Data = "";
                    return Json(_Result);
                }

                if (model.Name.Length > 6)
                {
                    _Result.Code = "510";
                    _Result.Msg = "应用名称最多不能超过6个字符";
                    _Result.Data = "";
                    return Json(_Result);
                }

                if (model.PlatformType == 2 && string.IsNullOrEmpty(model.Namespace))
                {
                    _Result.Code = "510";
                    _Result.Msg = "android应用命名空间不能为空";
                    _Result.Data = "";
                    return Json(_Result);
                }

                //if (model.PlatformType == 1 && string.IsNullOrEmpty(model.Startup))
                //{
                //    _Result.Code = "510";
                //    _Result.Msg = "启动项名称不能为空";
                //    _Result.Data = "";
                //    return Json(_Result);
                //}

                if (string.IsNullOrEmpty(model.Version))
                {
                    _Result.Code = "510";
                    _Result.Msg = "版本号不能为空";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var appIsExists = await dbContext.ApplicationNew.Where(i => i.MallCode == uol.MallCode && i.Name == model.Name && !i.IsDel && i.PlatformType == model.PlatformType).CountAsync();
                if (appIsExists > 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "应用名称已存在";
                    _Result.Data = "";
                    return Json(_Result);
                }
                string appCode = Guid.NewGuid().ToString();
                ApplicationNew applicationNew = new ApplicationNew()
                {
                    MallCode = uol.MallCode,
                    AddTime = DateTime.Now,
                    AppClass = model.AppClass,
                    AppID = "",
                    AppSecClass = model.AppSecClass,
                    Code = appCode,
                    Description = "",
                    FileCode = model.FileCode,
                    IconFileCode = model.IconFileCode,
                    IsDel = false,
                    Name = model.Name,
                    NameEn = model.NameEn,
                    Namespace = model.Namespace,
                    PlatformType = model.PlatformType,
                    ScreenInfoCode = model.ScreenInfo,
                    UpdateTime = DateTime.Now,
                    Version = model.Version,
                    Startup = model.Startup
                };

                AppSite appSite = new AppSite
                {
                    AddTime = DateTime.Now,
                    AppCode = appCode,
                    Code = Guid.NewGuid().ToString(),
                    Href = model.ServerUrl
                };

                dbContext.ApplicationNew.Add(applicationNew);
                dbContext.AppSite.Add(appSite);
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    //SendNews(Method.ServerAddr + file.FilePath, model.Duration, model.ScreenInfo, dbContext);
                    _Result.Code = "200";
                    _Result.Msg = "添加成功";
                    _Result.Data = "";
                }
                else
                {
                    _Result.Code = "501";
                    _Result.Msg = "添加失败";
                    _Result.Data = "";
                }
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";

            }
            return Json(_Result);
        }


        /// <summary>
        /// 应用更新
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Input_AppInfoEdit model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_AppInfoEdit)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

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

                var applicationNew = await dbContext.ApplicationNew.Where(i => i.Code == model.Code).FirstOrDefaultAsync();
                if (applicationNew == null)
                {
                    _Result.Code = "401";
                    _Result.Msg = "无效的应用编码：" + model.Code;
                    _Result.Data = "";
                    return Json(_Result);
                }

                var file = await dbContext.AssetFiles.Where(i => i.Code == model.FileCode).FirstOrDefaultAsync();
                if (file == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的文件编码：" + model.FileCode;
                    _Result.Data = "";
                    return Json(_Result);
                }

                var iconFile = await dbContext.AssetFiles.Where(i => i.Code == model.IconFileCode).FirstOrDefaultAsync();
                if (iconFile == null)
                {

                    _Result.Code = "510";
                    _Result.Msg = "无效的应用图标编码：" + model.IconFileCode;
                    _Result.Data = "";
                    return Json(_Result);
                }

                if (model.PlatformType == 2 && string.IsNullOrEmpty(model.Namespace))
                {
                    _Result.Code = "510";
                    _Result.Msg = "android应用命名空间不能为空";
                    _Result.Data = "";
                    return Json(_Result);
                }

                //if (model.PlatformType == 1 && string.IsNullOrEmpty(model.Startup))
                //{
                //    _Result.Code = "510";
                //    _Result.Msg = "启动项名称不能为空";
                //    _Result.Data = "";
                //    return Json(_Result);
                //}

                if (string.IsNullOrEmpty(model.Version))
                {
                    _Result.Code = "510";
                    _Result.Msg = "版本号不能为空";
                    _Result.Data = "";
                    return Json(_Result);
                }

                applicationNew.UpdateTime = DateTime.Now;
                //applicationNew.Description = model.FileCode;
                applicationNew.IconFileCode = model.IconFileCode;
                applicationNew.Namespace = model.Namespace;
                applicationNew.PlatformType = model.PlatformType;
                applicationNew.Version = model.Version;
                applicationNew.Startup = model.Startup;
                applicationNew.FileCode = model.FileCode;
                dbContext.ApplicationNew.Update(applicationNew);

                AppSite appSite = await dbContext.AppSite.Where(i => i.AppCode == model.Code).FirstOrDefaultAsync();
                if (appSite != null)
                {
                    appSite.Href = model.ServerUrl;
                    dbContext.AppSite.Update(appSite);
                }

                if (await dbContext.SaveChangesAsync() > 0)
                {
                    //SendNews(Method.ServerAddr + file.FilePath, model.Duration, model.ScreenInfo, dbContext);
                    _Result.Code = "200";
                    _Result.Msg = "更新成功";
                    _Result.Data = "";
                }
                else
                {
                    _Result.Code = "501";
                    _Result.Msg = "更新失败";
                    _Result.Data = "";
                }
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";

            }
            return Json(_Result);
        }


        [HttpPost]
        public async Task<IActionResult> GetDeviceByAppCode(Input_GetDeviceByAppCode model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetDeviceByAppCode)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            try
            {
                var application = await dbContext.ApplicationNew.Where(i => i.Code == model.AppCode).Join(dbContext.ScreenInfo, a => a.ScreenInfoCode, s => s.Code, (a, s) => new
                {
                    a.MallCode,
                    a.Name,
                    a.AppClass,
                    a.AppID,
                    a.AppSecClass,
                    a.IconFileCode,
                    a.NameEn,
                    a.PlatformType,
                    a.Version,
                    s.SName,
                    a.ScreenInfoCode
                }).Join(dbContext.AssetFiles, a => a.IconFileCode, af => af.Code, (a, af) => new {
                    a.Name,
                    a.AppClass,
                    a.AppID,
                    a.AppSecClass,
                    a.IconFileCode,
                    a.NameEn,
                    a.PlatformType,
                    a.Version,
                    a.SName,
                    a.ScreenInfoCode,
                    a.MallCode,
                    IconFilePath = Method.OSSServer + af.FilePath
                }).FirstOrDefaultAsync();
                if (application == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "应用编码输入错误";
                    _Result.Data = null;
                    return Json(_Result);
                }



                var sysremtype = "Windows";
                switch (application.PlatformType)
                {
                    case 1:
                        sysremtype = "Windows";
                        break;
                    case 2:
                        sysremtype = "Android";
                        break;
                    case 3:
                        sysremtype = "HTML5";
                        break;
                    default:
                        break;
                }

                var deviceList = await dbContext.Device.Where(i => i.MallCode == application.MallCode && !i.IsDelete && i.ScreenInfo == application.ScreenInfoCode && (i.SystemType == sysremtype || sysremtype == "HTML5")).Join(dbContext.ScreenInfo.Where(i => i.Code == application.ScreenInfoCode), d => d.ScreenInfo, s => s.Code, (d, s) => new
                {
                    d.DevNum,
                    d.Building,
                    d.Floor,
                    d.IP,
                    d.MAC,
                    d.Mark,
                    d.ScreenInfo,
                    d.Position,
                    d.Code,
                    s.SName
                }).Join(dbContext.Building, d => d.Building, b => b.Code, (d, b) => new
                {
                    d.DevNum,
                    d.Building,
                    d.Floor,
                    d.IP,
                    d.MAC,
                    d.Mark,
                    d.ScreenInfo,
                    d.Position,
                    d.SName,
                    d.Code,
                    BuildingName = b.Name

                }).Join(dbContext.Floor, d => d.Floor, f => f.Code, (d, f) => new
                {
                    d.DevNum,
                    d.Building,
                    d.Floor,
                    d.IP,
                    d.MAC,
                    d.Mark,
                    d.Position,
                    d.ScreenInfo,
                    d.SName,
                    d.BuildingName,
                    d.Code,
                    FloorName = f.Name
                }).ToListAsync();
                var existsDeviceList = deviceList.Join(dbContext.AppDev.Where(i => i.AppCode == model.AppCode), d => d.Code, dg => dg.DevCode, (d, dg) => d).ToList();
                var unExistsDeviceList = deviceList.Except(existsDeviceList).ToList();
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { UnExistsDeviceList = unExistsDeviceList, ExistsDeviceList = existsDeviceList, App = application };

            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);
            }

            return Json(_Result);
        }

        [HttpPost]
        public async Task<IActionResult> DelAppNew(Input_DelAppNew model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_DelAppNew)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


            try
            {
                //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
                //if (string.IsNullOrEmpty(username))
                //{
                //    _Result.Code = "401";
                //    _Result.Msg = "请登陆后再进行操作";
                //    _Result.Data = "";
                //    return Json(_Result);
                //}
                var app = await dbContext.ApplicationNew.Where(i => i.Code == model.AppCode).FirstOrDefaultAsync();
                if (app == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的应用编码：" + model.AppCode;
                    _Result.Data = "";
                    return Json(_Result);
                }

                app.IsDel = true;
                dbContext.ApplicationNew.Update(app);
                var appUser = await dbContext.UserAppNew.Where(i => i.AppCode == model.AppCode).ToListAsync();
                dbContext.UserAppNew.RemoveRange(appUser);
                var appDevList = await dbContext.AppDev.Where(i => i.AppCode == model.AppCode).ToListAsync();
                var appTimeList = new List<AppTime>();
                foreach (var item in appDevList)
                {
                    var appTime = await dbContext.AppTime.Where(i => i.AppCode == item.Code).ToListAsync();
                    appTimeList.AddRange(appTime);
                    if (item.Default)
                    {
                        var appDevOther = await dbContext.AppDev.Where(i => i.DevCode == item.DevCode && i.Code != item.Code).OrderBy(i => i.AddTime).FirstOrDefaultAsync();
                        if (appDevOther != null)
                        {
                            var appTimeOther = await dbContext.AppTime.Where(i => i.AppCode == appDevOther.Code).ToListAsync();
                            appTimeList.AddRange(appTimeOther);
                            appDevOther.Default = true;
                            dbContext.AppDev.Update(appDevOther);
                        }
                    }
                }
                var deviceList = await dbContext.Device.Where(i => !i.IsDelete).Join(appDevList, d => d.Code, ad => ad.DevCode, (d, ad) => d).ToListAsync();
                dbContext.AppDev.RemoveRange(appDevList);
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    _Result.Code = "200";
                    _Result.Msg = "删除成功";
                    _Result.Data = "";

                    if (deviceList.Count > 0)
                    {
                        foreach (var item in deviceList)
                        {
                            var appTime = await dbContext.AppTime.Join(dbContext.TimeSlot.Where(i => i.BeginTimeSlot == (DateTime.Now.ToString("HH") + ":00")), at => at.TimeSlot, ts => ts.Code, (at, ts) => at).ToListAsync();
                            var localApp = await dbContext.AppDev.Where(i => i.DevCode == item.Code).Join(appTime, ad => ad.Code, at => at.AppCode, (ad, at) => ad).FirstOrDefaultAsync();
                            var defaultAppCode = "";
                            if (localApp != null)
                            {
                                defaultAppCode = localApp.AppCode;
                            }
                            else
                            {
                                var devDefaultApp = await dbContext.AppDev.Where(i => i.Default && i.DevCode == item.Code).FirstOrDefaultAsync();
                                if (devDefaultApp != null)
                                {
                                    defaultAppCode = devDefaultApp.AppCode;
                                }
                            }
                            MsgTemplate msg = new MsgTemplate();
                            msg.SenderID = Method.ServerAddr;
                            msg.ReceiverID = item.Code;
                            msg.MessageType = "json";
                            msg.Content = new { Type = "AppDown", AppID = model.AppCode, DefaultAppCode = defaultAppCode };
                            await Method.SendMsgAsync(msg);
                        }
                    }
                }


            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);
            }

            return Json(_Result);
        }


        [HttpPost]
        public async Task<IActionResult> AppPublish(Input_PublishAppToDevice model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_PublishAppToDevice)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            //检测用户登录情况
            //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
            //if (string.IsNullOrEmpty(username))
            //{
            //    _Result.Code = "401";
            //    _Result.Msg = "请登陆后再进行操作";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}
            try
            {
                var application = await dbContext.ApplicationNew.Where(i => i.Code == model.AppCode).FirstOrDefaultAsync();
                if (application == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "应用编码输入错误";
                    _Result.Data = null;
                    return Json(_Result);
                }

                //应用设备关系
                var appDevices = await dbContext.AppDev.Where(i => i.AppCode == model.AppCode).ToListAsync();

                if (model.DeviceCodes.Count() > 0)
                {
                    var errorNoCodeList = new ArrayList();
                    var errorScreenList = new ArrayList();
                    //判断ID是否都为有效设备
                    foreach (var item in model.DeviceCodes)
                    {
                        var dev = await dbContext.Device.Where(i => i.Code == item).FirstOrDefaultAsync();
                        if (dev == null)
                        {
                            errorNoCodeList.Add(item);
                        }
                        if (dev.ScreenInfo.ToLower() != application.ScreenInfoCode.ToLower())
                        {
                            errorScreenList.Add(dev.IP);
                        }
                    }
                    if (errorNoCodeList.Count > 0)
                    {
                        _Result.Code = "510";
                        _Result.Msg = "没有编码为：" + string.Join(";", (string[])errorNoCodeList.ToArray(typeof(string))) + "的设备";
                        _Result.Data = "";
                        return Json(_Result);
                    }
                    if (errorScreenList.Count > 0)
                    {
                        _Result.Code = "510";
                        _Result.Msg = "IP为：" + string.Join(";", (string[])errorScreenList.ToArray(typeof(string))) + "的设备屏幕分辨率与应用分辨率不一致";
                        _Result.Data = "";
                        return Json(_Result);
                    }
                }
                List<string> _OldDeviceCode = new List<string>();//待移除的设备
                List<string> existsDevice = new List<string>();//数据库中已包含的设备
                List<string> _NewDeviceCode = new List<string>();//待添加的设备

                //列表中不包含 移除
                foreach (var gd in appDevices)
                {

                    if (!model.DeviceCodes.Contains(gd.DevCode))
                    {
                        _OldDeviceCode.Add(gd.DevCode);

                        //移除对应的应用、设备关系
                        dbContext.AppDev.Remove(gd);
                        //删除设备应用时间段关系
                        var devTimes = await dbContext.AppTime.Where(i => i.AppCode == gd.Code).ToListAsync();
                        dbContext.AppTime.RemoveRange(devTimes);
                        //获取当前设备所以应用
                        var deviceAppList = await dbContext.AppDev.Where(i => i.DevCode == gd.DevCode && i.AppCode != model.AppCode).OrderBy(i => i.AddTime).ToListAsync();
                        //判断是否有默认应用
                        var defaultApp = deviceAppList.Where(i => i.Default).FirstOrDefault();
                        //如果设备有多余应用但是没有默认应用，则设置第一个应用为默认应用，并删除当前应用的时间段关系
                        if (deviceAppList.Count > 0 && defaultApp == null)
                        {
                            var appDevDefault = deviceAppList.FirstOrDefault();
                            appDevDefault.Default = true;
                            var devDefaultTimes = await dbContext.AppTime.Where(i => i.AppCode == appDevDefault.Code).ToListAsync();
                            dbContext.AppTime.RemoveRange(devDefaultTimes);
                        }
                    }
                    else
                    {
                        existsDevice.Add(gd.DevCode);
                    }
                }

                //列表中不存在 添加
                foreach (var device in model.DeviceCodes)
                {
                    if (!existsDevice.Contains(device))
                    {
                        var appDevice = new AppDev { AddTime = DateTime.Now, AppCode = model.AppCode, Code = Guid.NewGuid().ToString(), Default = false, DevCode = device, UpdateTime = DateTime.Now };
                        var devAppCount = await dbContext.AppDev.Where(i => i.DevCode == device).CountAsync();
                        if (devAppCount == 0)
                        {
                            appDevice.Default = true;
                        }
                        dbContext.AppDev.Add(appDevice);
                        _NewDeviceCode.Add(appDevice.DevCode);
                    }
                }
                int result = await dbContext.SaveChangesAsync();
                _Result.Code = "200";
                _Result.Msg = "发布成功";
                _Result.Data = "";

                if (result > 0)
                {
                    if (_NewDeviceCode.Count > 0)
                    {
                        var deviceList = dbContext.Device.Where(i => _NewDeviceCode.Contains(i.Code)).ToList();
                        var file = await dbContext.AssetFiles.Where(i => i.Code == application.FileCode).FirstOrDefaultAsync();
                        var iconFile = await dbContext.AssetFiles.Where(i => i.Code == application.IconFileCode).FirstOrDefaultAsync();
                        foreach (var item in deviceList)
                        {
                            var app = await dbContext.AppDev.Where(i => i.DevCode == item.Code && i.AppCode == model.AppCode).FirstOrDefaultAsync();
                            MsgTemplate msg = new MsgTemplate();
                            msg.SenderID = Method.ServerAddr;
                            msg.ReceiverID = item.Code;
                            msg.MessageType = "json";
                            msg.Content = new { Type = "AppPublish", Default = app.Default, AppName = application.Name, FilePath = file == null ? "" : Method.OSSServer + file.FilePath, AppNameen = application.Startup, IconFilePath = Method.OSSServer + iconFile.FilePath, appid = application.Code, application.Namespace, application.Version };
                            await Method.SendMsgAsync(msg);
                        }
                    }

                    if (_OldDeviceCode.Count > 0)
                    {
                        var deviceList = dbContext.Device.Where(i => _OldDeviceCode.Contains(i.Code)).ToList();
                        foreach (var item in deviceList)
                        {

                            var appTime = await dbContext.AppTime.Join(dbContext.TimeSlot.Where(i => i.BeginTimeSlot == (DateTime.Now.ToString("HH") + ":00")), at => at.TimeSlot, ts => ts.Code, (at, ts) => at).ToListAsync();
                            var localApp = await dbContext.AppDev.Where(i => i.DevCode == item.Code).Join(appTime, ad => ad.Code, at => at.AppCode, (ad, at) => ad).FirstOrDefaultAsync();
                            var defaultAppCode = "";
                            if (localApp != null)
                            {
                                defaultAppCode = localApp.AppCode;
                            }
                            else
                            {
                                var devDefaultApp = await dbContext.AppDev.Where(i => i.Default && i.DevCode == item.Code).FirstOrDefaultAsync();
                                if (devDefaultApp != null)
                                {
                                    defaultAppCode = devDefaultApp.AppCode;
                                }
                            }
                            MsgTemplate msg = new MsgTemplate();
                            msg.SenderID = Method.ServerAddr;
                            msg.ReceiverID = item.Code;
                            msg.MessageType = "json";
                            msg.Content = new { Type = "AppDown", AppID = application.Code, DefaultAppCode = defaultAppCode };
                            await Method.SendMsgAsync(msg);
                        }
                    }
                }
            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";

            }
            return Json(_Result);
        }


        /// <summary>
        /// 获取应用列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        //[HttpPost]
        public async Task<IActionResult> GetAppList([FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            var apps = await dbContext.Application.Where(i => i.IsDel == false).Join(dbContext.AssetFiles, ap => ap.IconFileID, af => af.ID, (ap, af) => new
            {
                Name = ap.Name,
                ID = ap.ID,
                AppClass = ap.AppClass,
                AppSecClass = ap.AppSecClass,
                Description = ap.Description,
                IconFilePath = Method.OSSServer + af.FilePath.ToString(),
                ScreenInfoID = ap.ScreenInfoID,
                Version = ap.Version,
                AppID = ap.AppID,
                AddTime = ap.AddTime.ToString("yyyy-MM-dd HH:mm:ss")

            }).Join(dbContext.AppClass, ap => ap.AppClass, ac => ac.ID, (ap, ac) => new {

                ap.Name,
                ap.ID,
                AppClass = ac.ClassName,
                ap.AppSecClass,
                ap.Description,
                ap.IconFilePath,
                ap.ScreenInfoID,
                ap.Version,
                ap.AppID,
                ap.AddTime

            }).Join(dbContext.AppClass, ap => ap.AppSecClass, ac => ac.ID, (ap, ac) => new {

                ap.Name,
                ap.ID,
                ap.AppClass,
                AppSecClass = ac.ClassName,
                ap.Description,
                ap.IconFilePath,
                ap.ScreenInfoID,
                ap.Version,
                ap.AppID,
                ap.AddTime

            }).AsNoTracking().ToListAsync();
            ArrayList arrayList = new ArrayList();
            foreach (var app in apps)
            {
                string href = string.Empty;
                var sitecount = await dbContext.AppSite.Where(i => i.AppCode == app.AppID).AsNoTracking().CountAsync();
                if (sitecount > 0)
                {
                    var appsite = await dbContext.AppSite.Where(i => i.AppCode == app.AppID).AsNoTracking().FirstOrDefaultAsync();
                    href = appsite.Href;

                    //检测用户登录情况
                    //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
                    //if (!string.IsNullOrEmpty(username))
                    //{
                    //    var _User = dbContext.Accounts.Where(i => i.AccountName == username).FirstOrDefault();
                    //    string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    //    string salt = "QianMu";
                    //    string key = salt + dtime;

                    //    string _EncryptionKey = Method.StringToPBKDF2Hash(key);

                    //    _EncryptionKey = System.Net.WebUtility.UrlEncode(_EncryptionKey);
                    //    href = href + "?account=" + username + "&CheckTime=" + dtime + "&Token=" + _EncryptionKey + "&Pwd=" + _User.PassWord;
                    //}

                }
                arrayList.Add(new
                {
                    app.Name,
                    app.ID,
                    app.AppClass,
                    app.AppSecClass,
                    app.Description,
                    app.IconFilePath,
                    app.ScreenInfoID,
                    app.Version,
                    Href = href,
                    app.AddTime,
                    AppCode = app.AppID,
                });
            }


            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = arrayList;
            return Json(_Result);
        }

        public async Task<IActionResult> GetAppListNew([FromServices] ContextString dbContext)
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
            var apps = await dbContext.ApplicationNew.Where(i => i.MallCode == uol.MallCode && i.IsDel == false).Join(dbContext.AssetFiles, ap => ap.IconFileCode, af => af.Code, (ap, af) => new
            {
                Name = ap.Name,
                ID = ap.ID,
                AppClass = ap.AppClass,
                AppSecClass = ap.AppSecClass,
                Description = ap.Description,
                ap.IconFileCode,
                IconFilePath = Method.OSSServer + af.FilePath.ToString(),
                ScreenInfo = ap.ScreenInfoCode,
                Version = ap.Version,
                ap.Code,
                ap.PlatformType,
                ap.Startup,
                ap.Namespace,
                AddTime = ap.AddTime.ToString("yyyy-MM-dd HH:mm:ss")

            })
            .Join(dbContext.AppClassNew, ap => ap.AppClass, ac => ac.Code, (ap, ac) => new
            {

                ap.Name,
                ap.ID,
                AppClass = ac.ClassName,
                ap.AppSecClass,
                ap.Description,
                ap.IconFileCode,
                ap.IconFilePath,
                ap.ScreenInfo,
                ap.Version,
                ap.Code,
                ap.PlatformType,
                ap.Startup,
                ap.Namespace,
                ap.AddTime

            })
            .Join(dbContext.AppClassNew, ap => ap.AppSecClass, ac => ac.Code, (ap, ac) => new {

                ap.Name,
                ap.ID,
                ap.AppClass,
                AppSecClass = ac.ClassName,
                ap.Description,
                ap.IconFileCode,
                ap.IconFilePath,
                ap.ScreenInfo,
                ap.Version,
                ap.Code,
                ap.PlatformType,
                ap.Startup,
                ap.Namespace,
                ap.AddTime

            }).Join(dbContext.ScreenInfo.Where(i => i.MallCode == uol.MallCode), ap => ap.ScreenInfo, s => s.Code, (ap, s) => new {
                ap.Name,
                ap.ID,
                ap.AppClass,
                ap.AppSecClass,
                ap.Description,
                ap.IconFileCode,
                ap.IconFilePath,
                ap.ScreenInfo,
                ap.Version,
                ap.Code,
                ap.PlatformType,
                ap.Startup,
                ap.Namespace,
                ap.AddTime,
                s.SName
            }).AsNoTracking().ToListAsync();
            ArrayList arrayList = new ArrayList();
            foreach (var app in apps)
            {
                string href = string.Empty;
                string ServerUrl = string.Empty;
                var sitecount = await dbContext.AppSite.Where(i => i.AppCode == app.Code).AsNoTracking().CountAsync();
                if (sitecount > 0)
                {
                    var appsite = await dbContext.AppSite.Where(i => i.AppCode == app.Code).AsNoTracking().FirstOrDefaultAsync();
                    href = appsite.Href;
                    ServerUrl = appsite.Href;
                    //检测用户登录情况
                    //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
                    //if (!string.IsNullOrEmpty(username))
                    //{
                    //    var _User = dbContext.Accounts.Where(i => i.AccountName == username).FirstOrDefault();
                    //    string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    //    string salt = "QianMu";
                    //    string key = salt + dtime;

                    //    string _EncryptionKey = Method.StringToPBKDF2Hash(key);

                    //    _EncryptionKey = System.Net.WebUtility.UrlEncode(_EncryptionKey);
                    //    href = href + "?account=" + username + "&CheckTime=" + dtime + "&Token=" + _EncryptionKey + "&Pwd=" + _User.PassWord;
                    //}

                }
                var appDeviceCount = await dbContext.AppDev.Where(i => i.AppCode == app.Code).Join(dbContext.Device, ad => ad.DevCode, d => d.Code, (ad, d) => ad).CountAsync();
                arrayList.Add(new
                {
                    app.Name,
                    app.ID,
                    app.AppClass,
                    app.AppSecClass,
                    app.Description,
                    app.IconFilePath,
                    app.IconFileCode,
                    app.ScreenInfo,
                    app.Version,
                    app.Startup,
                    Href = href,
                    ServerUrl,
                    app.AddTime,
                    app.PlatformType,
                    app.SName,
                    AppCode = app.Code,
                    app.Namespace,
                    FileCode = "",
                    DeviceCount = appDeviceCount
                });
            }


            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = arrayList;
            return Json(_Result);
        }

        /// <summary>
        /// 删除应用
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> DelApps(string ids, [FromServices] ContextString dbContext)
        {

            QianMuResult _Result = new QianMuResult();

            //检测用户登录情况
            //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
            //if (string.IsNullOrEmpty(username))
            //{
            //    _Result.Code = "401";
            //    _Result.Msg = "请登陆后再进行操作";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}

            //检测ID合法性
            if (String.IsNullOrEmpty(ids))
            {
                _Result.Code = "510";
                _Result.Msg = "未能检测到IDS";
                _Result.Data = "";
                return Json(_Result);
            }
            string[] _ids = ids.Split(',');
            List<int> list = new List<int>();
            foreach (var item in _ids)
            {
                if (!Method.IsNumeric(item))
                {
                    _Result.Code = "510";
                    _Result.Msg = "Erro:包含非法的ID" + item;
                    _Result.Data = "";
                    return Json(_Result);
                }
                list.Add(int.Parse(item));
            }

            //判断ID是否都为有效应用
            foreach (var item in list)
            {
                var q = await dbContext.Application.Where(i => i.ID == item).AsNoTracking().CountAsync();
                if (q <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "Erro:没有ID为：" + item + "的应用";
                    _Result.Data = "";
                    return Json(_Result);
                }
            }


            var appnames = string.Empty;

            foreach (var item in list)
            {
                //删除应用到设备组关系
                // dbContext.ApplicationDevice.RemoveRange(dbContext.ApplicationDevice.Where(i => i.AppID == item));
                var appdevs = await dbContext.ApplicationDevice.Where(i => i.AppID == item).ToListAsync();
                foreach (var appdev in appdevs)
                {
                    appdev.IsDel = true;
                }
                dbContext.ApplicationDevice.UpdateRange(appdevs);
                //删除应用钟
                // dbContext.AppUsageInfo.RemoveRange(dbContext.AppUsageInfo.Where(i => i.AppID == item));

                var appuis = await dbContext.AppUsageInfo.Where(i => i.AppID == item).ToListAsync();
                foreach (var appui in appuis)
                {
                    appui.IsDel = true;
                }

                dbContext.AppUsageInfo.UpdateRange(appuis);


                //删除应用
                var app = dbContext.Application.Where(i => i.ID == item).SingleOrDefault();
                app.IsDel = true;
                appnames += app.Name + ",";
                dbContext.Application.Update(app);



            }

            if (await dbContext.SaveChangesAsync() > 0)
            {

                _Result.Code = "200";
                _Result.Msg = "删除成功";
                _Result.Data = "";

                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "应用模块", LogMsg = username + "删除了名称为：" + appnames.TrimEnd(',') + "的应用，ID为：" + ids, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "删除", IP = ip });

                // dbContext.SaveChanges();

            }
            else
            {
                _Result.Code = "200";
                _Result.Msg = "删除失败";
                _Result.Data = "";
            }
            return Json(_Result);

        }

        /// <summary>
        /// 获取应用信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetAppInfo(int? id, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            ////检测用户登录情况
            //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
            //if (string.IsNullOrEmpty(username))
            //{
            //    _Result.Code = "401";
            //    _Result.Msg = "请登陆后再进行操作";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}

            if (id == null)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个ID";
                _Result.Data = "";
                return Json(_Result);
            }

            var app = await dbContext.Application.Where(i => (i.ID == id && i.IsDel == false)).Join(dbContext.AppClass, ap => ap.AppClass, ac => ac.ID, (ap, ac) => new {
                ap.ID,
                ap.Name,
                ap.NameEn,
                ap.DevSupport,
                ap.Description,
                AppClass = ac.ClassName,
                ap.AppSecClass,
                ap.IconFileID
            }).Join(dbContext.AppClass, ap => ap.AppSecClass, ac => ac.ID, (ap, ac) => new {
                ap.ID,
                ap.Name,
                ap.NameEn,
                ap.DevSupport,
                ap.Description,
                ap.AppClass,
                AppSecClass = ac.ClassName,
                ap.IconFileID
            })
                .AsNoTracking().FirstOrDefaultAsync();

            if (app == null)
            {
                _Result.Code = "1";
                _Result.Msg = "无效的应用ID";
                _Result.Data = "";
                return Json(_Result);
            }
            var iconfile = await dbContext.AssetFiles.Where(i => i.ID == app.IconFileID).Select(s => new { IconFilePath = Method.OSSServer + s.FilePath.ToString() }).AsNoTracking().FirstOrDefaultAsync();
            List<long> PreviewFileIDs = new List<long>();
            //  PreviewFileIDs = app.PreviewFiles.Split(',').Select(i => Convert.ToInt64(i)).ToList();

            // var PreviewFiles = await dbContext.AssetFiles.Where(i => PreviewFileIDs.Contains(i.ID)).Select(s => new { FilePath = Method.ServerAddr + s.FilePath.ToString() }).AsNoTracking().ToListAsync();


            var appinfo = new { app.ID, app.Name, app.NameEn, app.AppClass, app.AppSecClass, iconfile.IconFilePath, HardwareLabel = app.DevSupport, app.Description };

            _Result.Code = "200";
            _Result.Msg = "查询成功";
            _Result.Data = appinfo;

            //var ip = Method.GetUserIp(this.HttpContext);
            //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "应用模块", LogMsg = username + "获取了ID为：" + id + "的应用信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
            //dbContext.SaveChanges();

            return Json(_Result);
        }

        /// <summary>
        /// 程序钟
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> SetAppClock(Input_AppClock model, [FromServices] ContextString dbContext)
        {

            QianMuResult _Result = new QianMuResult();

            //检测用户登录情况
            //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
            //if (string.IsNullOrEmpty(username))
            //{
            //    _Result.Code = "401";
            //    _Result.Msg = "请登陆后再进行操作";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}

            var appclocks = await dbContext.AppUsageInfo.Where(i => (i.DevID == model.DevID && i.IsDel == false)).AsNoTracking().ToListAsync();
            if (appclocks != null)
            {
                TimeSpan tsml = TimeSpan.Parse(model.LaunchTime);
                TimeSpan tsme = TimeSpan.Parse(model.ExpiryDate);
                foreach (var ac in appclocks)
                {
                    TimeSpan tsacl = TimeSpan.Parse(ac.LaunchTime);
                    TimeSpan tsace = TimeSpan.Parse(ac.ExpiryDate);
                    //判断是否有时间冲突

                    if ((tsacl < tsml && tsace > tsml) ||
                     (tsacl < tsme && tsace > tsml) ||
                     (tsacl >= tsml && tsace <= tsme))
                    {
                        _Result.Code = "1";
                        _Result.Msg = "设备时段已被占用，设置失败！";
                        _Result.Data = "";
                        return Json(_Result);
                    }
                    //    if ( (ac.LaunchTime <= model.LaunchTime && ac.ExpiryDate > model.LaunchTime) || 
                    //    (ac.LaunchTime < model.ExpiryDate && ac.ExpiryDate >= model.LaunchTime) ||
                    //    (ac.LaunchTime >= model.LaunchTime && ac.ExpiryDate <= model.ExpiryDate) )
                    //{
                    //    _Result.Code = "1";
                    //    _Result.Msg = "设备时段已被占用，设置失败！";
                    //    _Result.Data = "";
                    //    return Json(_Result);
                    //}
                }
            }

            dbContext.AppUsageInfo.Add(new AppUsageInfo { Code = Guid.NewGuid().ToString(),UpdateTime = DateTime.Now, AppID = model.AppID, DevID = model.DevID, LaunchTime = model.LaunchTime, ExpiryDate = model.ExpiryDate, AddTime = DateTime.Now, IsDel = false });

            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "添加成功";
                _Result.Data = "";

                var dev = dbContext.Device.Where(i => i.ID == model.DevID).FirstOrDefault();
                var app = dbContext.Application.Where(i => i.ID == model.AppID).FirstOrDefault();

                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "应用模块", LogMsg = username + "给IP为" + dev.IP + "的设备设定在" + model.LaunchTime + "到" + model.ExpiryDate + "时间段内运行名称为：" + app.Name + "的程序", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });

                //  dbContext.SaveChanges();
            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "添加失败";
                _Result.Data = "";
            }

            return Json(_Result);
        }




        /// <summary>
        /// 编辑用户-应用关系
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UserAppEdit(Input_EditUserApp model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_EditUserApp)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            //检测用户登录情况
            //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
            //if (string.IsNullOrEmpty(username))
            //{
            //    _Result.Code = "401";
            //    _Result.Msg = "请登陆后再进行操作";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}

            if (model.UserIDS.Count() <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "用户ID不可为空";
                _Result.Data = "";
                return Json(_Result);
            }

            if (model.UserIDS.Count() > model.UserIDS.Distinct().Count() || model.AppCode.Count() > model.AppCode.Distinct().Count())
            {
                _Result.Code = "510";
                _Result.Msg = "重复的用户ID或应用编码";
                _Result.Data = "";
                return Json(_Result);
            }

            foreach (var uid in model.UserIDS)
            {
                var count = await dbContext.Account.Where(i => i.ID == uid).AsNoTracking().CountAsync();
                if (count <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的用户ID:" + uid;
                    _Result.Data = "";
                    return Json(_Result);
                }
            }

            foreach (var c in model.AppCode)
            {
                var ftcount = await dbContext.Application.Where(i => i.AppID == c).CountAsync();
                if (ftcount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的应用编码：" + c;
                    _Result.Data = "";
                    return Json(_Result);
                }
            }

            foreach (var uid in model.UserIDS)
            {
                //移除旧的标签
                //   dbContext.UserApp.RemoveRange(dbContext.UserApp.Where(i => i.UserID ==uid));

                //添加新的标签
                List<UserApp> list = new List<UserApp>();
                foreach (var c in model.AppCode)
                {
                    var count = await dbContext.UserApp.Where(i => i.AppCode == c && i.UserID == uid).CountAsync();
                    if (count <= 0)
                    {
                        list.Add(new UserApp { AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), UserID = uid, AppCode = c, UpdateTime = DateTime.Now });
                    }
                    else
                    {
                        dbContext.UserApp.RemoveRange(dbContext.UserApp.Where(i => i.AppCode == c && i.UserID == uid));
                    }

                }

                list = list.Distinct().ToList();
                dbContext.UserApp.AddRange(list);

            }




            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "修改成功";
                _Result.Data = "";



                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "应用模块", LogMsg = username + "修改应用用户关系,访问数据：" + inputStr, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });

                // dbContext.SaveChanges();

            }
            else
            {
                _Result.Code = "1";
                _Result.Msg = "修改失败";
                _Result.Data = "";
            }

            return Json(_Result);
        }

        /// <summary>
        /// 编辑用户-应用关系
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UserAppEditNew(Input_EditUserAppNew model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_EditUserAppNew)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            //检测用户登录情况
            //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
            //if (string.IsNullOrEmpty(username))
            //{
            //    _Result.Code = "401";
            //    _Result.Msg = "请登陆后再进行操作";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}

            if (model.UserCodeList.Count() <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "用户ID不可为空";
                _Result.Data = "";
                return Json(_Result);
            }

            if (model.UserCodeList.Count() > model.UserCodeList.Distinct().Count() || model.AppCode.Count() > model.AppCode.Distinct().Count())
            {
                _Result.Code = "510";
                _Result.Msg = "重复的用户编码或应用编码";
                _Result.Data = "";
                return Json(_Result);
            }

            foreach (var ucode in model.UserCodeList)
            {
                var count = await dbContext.Account.Where(i => i.Code == ucode).AsNoTracking().CountAsync();
                if (count <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的用户编码:" + ucode;
                    _Result.Data = "";
                    return Json(_Result);
                }
            }

            foreach (var c in model.AppCode)
            {
                var ftcount = await dbContext.ApplicationNew.Where(i => i.Code == c).CountAsync();
                if (ftcount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的应用编码：" + c;
                    _Result.Data = "";
                    return Json(_Result);
                }
            }

            foreach (var uCode in model.UserCodeList)
            {
                //移除旧的标签
                //   dbContext.UserApp.RemoveRange(dbContext.UserApp.Where(i => i.UserID ==uid));

                //添加新的标签
                List<UserAppNew> list = new List<UserAppNew>();
                foreach (var c in model.AppCode)
                {
                    var count = await dbContext.UserAppNew.Where(i => i.AppCode == c && i.UserCode == uCode).CountAsync();
                    if (count <= 0)
                    {
                        list.Add(new UserAppNew { AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), UserCode = uCode, AppCode = c, UpdateTime = DateTime.Now });
                    }
                    else
                    {
                        dbContext.UserAppNew.RemoveRange(dbContext.UserAppNew.Where(i => i.AppCode == c && i.UserCode == uCode));
                    }

                }

                list = list.Distinct().ToList();
                dbContext.UserAppNew.AddRange(list);

            }




            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "修改成功";
                _Result.Data = "";



                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "应用模块", LogMsg = username + "修改应用用户关系,访问数据：" + inputStr, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });

                // dbContext.SaveChanges();

            }
            else
            {
                _Result.Code = "1";
                _Result.Msg = "修改失败";
                _Result.Data = "";
            }

            return Json(_Result);
        }


        public async Task<IActionResult> GetAppUser(string AppCode, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            if (string.IsNullOrEmpty(AppCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入应用编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var appcount = await dbContext.Application.Where(i => i.AppID == AppCode).CountAsync();
            if (appcount <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的应用编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var list = await dbContext.UserApp.Where(i => i.AppCode == AppCode).Join(dbContext.Account.Where(i => i.Activity == true), ua => ua.UserID, ac => ac.ID, (ua, ac) => new
            {
                ua.ID,
                ua.AppCode,
                ua.UserID,
                ac.AccountName


            }).ToListAsync();


            _Result.Code = "200";
            _Result.Msg = "查询成功";
            _Result.Data = list;
            if (list == null)
            {
                _Result.Data = "";
            }
            return Json(_Result);
        }


        public async Task<IActionResult> GetAppUserList(string AppCode, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            if (string.IsNullOrEmpty(AppCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入应用编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var appcount = await dbContext.Application.Where(i => i.AppID == AppCode).CountAsync();
            if (appcount <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的应用编码";
                _Result.Data = "";
                return Json(_Result);
            }



            var list = await dbContext.Output_AppUser.FromSql(@"select a.ID as UserID,a.AccountName ,a.NickName,CASE ISNULL(b.id,0) WHEN 0 THEN 0 ELSE 1 END   as [Status] from Account a left join  (select *  from   UserApp where AppCode = @AppCode) b on a.ID = b.UserID
                                                          where a.Activity = 1", new SqlParameter("@AppCode", AppCode)).ToListAsync();
            _Result.Code = "200";
            _Result.Msg = "查询成功";
            _Result.Data = list;
            if (list == null)
            {
                _Result.Data = "";
            }

            return Json(_Result);
        }


        [HttpPost]
        public async Task<IActionResult> GetAppUserListNew(Input_GetAppUserListNew model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetAppUserListNew)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            if (string.IsNullOrEmpty(model.AppCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入应用编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var app = await dbContext.ApplicationNew.Where(i => i.Code == model.AppCode).FirstOrDefaultAsync();
            if (app == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的应用编码";
                _Result.Data = "";
                return Json(_Result);
            }

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@AppCode", model.AppCode),
                new SqlParameter("@MallCode",app.MallCode)
            };

            var list = await dbContext.Output_AppUserNew.FromSql(@"select a.ID as UserID,a.AccountName ,a.NickName,a.Code AS UserCode,CASE ISNULL(b.id,0) WHEN 0 THEN 0 ELSE 1 END  
 as [Status] from Account a left join  
 (select *  from   UserAppNew where AppCode = @AppCode) b 
 on  a.Code = b.UserCode
 where a.Activity = 1 and a.MallCode = @MallCode
 union
 select 
 b.ID as UserID,b.AccountName ,b.NickName,b.Code AS UserCode,CASE ISNULL(c.id,0) WHEN 0 THEN 0 ELSE 1 END  
 as [Status] 
   from MallManager a left join Account b on a.ManagerCode = b.Code 
  left join  (select *  from   UserAppNew where AppCode = @AppCode) c 
  on b.Code = c.UserCode
  where b.Activity = 1 and a.MallCode = @MallCode", sqlParameters).ToListAsync();

            //var list = await dbContext.Output_AppUserNew.FromSql(@"select a.ID as UserID,a.AccountName ,a.NickName,a.Code AS UserCode,CASE ISNULL(b.id,0) WHEN 0 THEN 0 ELSE 1 END   as [Status] from Account a left join  (select *  from   UserAppNew where AppCode = @AppCode) b on  a.Code = b.UserCode
            //                                              where a.Activity = 1 and a.MallCode = @MallCode", sqlParameters).ToListAsync();
            _Result.Code = "200";
            _Result.Msg = "查询成功";
            _Result.Data = list;
            if (list == null)
            {
                _Result.Data = "";
            }

            return Json(_Result);
        }


        /// <summary>
        /// 获取用户应用
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetUserAppNew(Input_GetUserAppNew model, [FromServices] ContextString dbContext)
        {

            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetUserAppNew)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            var count = await dbContext.Account.Where(i => i.Activity == true && i.Code == model.UserCode).CountAsync();
            if (string.IsNullOrEmpty(model.UserCode) || count <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的用户编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var apps = await dbContext.UserAppNew.Where(i => i.UserCode == model.UserCode).Join(dbContext.ApplicationNew, ua => ua.AppCode, ap => ap.Code, (ua, ap) => new
            {

                ap.AddTime,
                ap.AppClass,
                ap.AppID,
                ap.AppSecClass,
                ap.Description,
                ap.Developer,
                ap.DevSupport,
                ap.FileCode,
                ap.IconFileCode,
                ap.ID,
                ap.IsDel,
                ap.Name,
                ap.NameEn,
                ap.PreviewFiles,
                ap.ScreenInfoCode,
                ap.Version,
                ap.Code,
                ap.PlatformType
            }).Join(dbContext.AssetFiles, ap => ap.IconFileCode, af => af.Code, (ap, af) => new
            {
                Name = ap.Name,
                ID = ap.ID,
                AppClass = ap.AppClass,
                AppSecClass = ap.AppSecClass,
                Description = ap.Description,
                IconFilePath = Method.OSSServer + af.FilePath.ToString(),
                ap.ScreenInfoCode,
                Version = ap.Version,
                AppID = ap.AppID,
                ap.Code,
                ap.PlatformType

            }).Join(dbContext.AppClassNew, ap => ap.AppClass, ac => ac.Code, (ap, ac) => new {

                ap.Name,
                ap.ID,
                AppClass = ac.ClassName,
                ap.AppSecClass,
                ap.Description,
                ap.IconFilePath,
                ap.ScreenInfoCode,
                ap.Version,
                ap.AppID,
                ap.Code,
                ap.PlatformType

            }).Join(dbContext.AppClassNew, ap => ap.AppSecClass, ac => ac.Code, (ap, ac) => new {

                ap.Name,
                ap.ID,
                ap.AppClass,
                AppSecClass = ac.ClassName,
                ap.Description,
                ap.IconFilePath,
                ap.ScreenInfoCode,
                ap.Version,
                ap.AppID,
                ap.Code,
                ap.PlatformType

            }).Join(dbContext.ScreenInfo, ap => ap.ScreenInfoCode, s => s.Code, (ap, s) => new {
                ap.Name,
                ap.ID,
                ap.AppClass,
                ap.AppSecClass,
                ap.Description,
                ap.IconFilePath,
                ap.ScreenInfoCode,
                ap.Version,
                ap.AppID,
                ap.Code,
                s.SName,
                ap.PlatformType

            }).AsNoTracking().ToListAsync(); ;


            ArrayList arrayList = new ArrayList();
            foreach (var app in apps)
            {
                string href = string.Empty;
                var isLogOut = 0;
                var sitecount = await dbContext.AppSite.Where(i => i.AppCode == app.Code).AsNoTracking().CountAsync();
                if (sitecount > 0)
                {
                    var appsite = await dbContext.AppSite.Where(i => i.AppCode == app.Code).AsNoTracking().FirstOrDefaultAsync();

                    if (!string.IsNullOrEmpty(appsite.Href))
                    {
                        href = appsite.Href;

                   
                    }

                }
                arrayList.Add(new
                {
                    app.Name,
                    app.ID,
                    app.AppClass,
                    app.AppSecClass,
                    app.Description,
                    app.IconFilePath,
                    app.Code,
                    app.SName,
                    app.Version,
                    Href = href,
                    app.PlatformType,
                    IsLotOut = isLogOut
                });
            }


            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = arrayList;

            return Json(_Result);
        }
    }
}