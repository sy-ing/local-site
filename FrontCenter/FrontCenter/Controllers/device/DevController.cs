using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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


namespace FrontCenter.Controllers.device
{
    public class DevController : Controller
    {

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDeviceList(Input_GetDevList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            UserOnLine user = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (user == null || string.IsNullOrEmpty(user.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetDevList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            try
            {
                string MallCode = model.MallCode;
                if (string.IsNullOrEmpty(model.MallCode))
                {
                    MallCode = user.MallCode;
                }
                if (string.IsNullOrEmpty(MallCode))
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的商场编码";
                    _Result.Data = "";
                    return Json(_Result);
                }
                var mall = await dbContext.Mall.Where(i => i.Code == MallCode).FirstOrDefaultAsync();
                if (mall == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的商场编码";
                    _Result.Data = "";
                    return Json(_Result);
                }



                if (model.FontStatus == null)
                {
                    model.FontStatus = -1;
                }

                if (model.DevStatus == null)
                {
                    model.DevStatus = -1;
                }

                var devices = await dbContext.Device.Where(i => !i.IsDelete && i.MallCode == MallCode
                  && ((i.IP.Contains(model.SearchKey) || i.DevNum.Contains(model.SearchKey) || i.Position.Contains(model.SearchKey)) || (String.IsNullOrEmpty(model.SearchKey)))
                  && (i.ScreenInfo == model.ScreenCode || String.IsNullOrEmpty(model.ScreenCode))
                  && (i.Floor == model.FloorCode || (String.IsNullOrEmpty(model.FloorCode)))
                  && (i.FrontOnline == Convert.ToBoolean(model.FontStatus) || model.FontStatus == -1)
                  && (i.DeviceOnline == Convert.ToBoolean(model.DevStatus) || model.DevStatus == -1)
                ).Join(dbContext.ScreenInfo, de => de.ScreenInfo, si => si.Code, (de, si) => new {
                    de.Code,
                    de.ID,
                    de.DevNum,
                    de.IP,
                    si.SName,
                    de.ScreenInfo,
                    de.Floor,
                    de.ShutdownTime,
                    de.DeviceOnline,
                    de.FrontOnline,
                    de.Position,
                    de.Mark,
                    de.IsSyn,
                    de.IsShow,
                    de.ScreenshotSrc,
                    de.Version,
                    de.AddTime,
                    de.SystemType,
                    de.DeviceType,
                    de.Operable


                }).Join(dbContext.Floor, de => de.Floor, fl => fl.Code, (de, fl) => new {
                    de.Code,
                    de.ID,
                    de.DevNum,
                    de.IP,
                    de.SName,
                    de.ScreenInfo,
                    Floor = fl.Name,
                    de.ShutdownTime,
                    de.DeviceOnline,
                    de.FrontOnline,
                    de.Position,
                    de.Mark,
                    de.IsSyn,
                    de.IsShow,
                    de.ScreenshotSrc,
                    de.Version,
                    de.AddTime,
                    de.SystemType,
                    de.DeviceType,
                    de.Operable

                }).Join(dbContext.DataDict, de => de.DeviceType, dd => dd.Code, (de, dd) => new {

                    de.Code,
                    de.ID,
                    de.DevNum,
                    de.IP,
                    de.SName,
                    de.ScreenInfo,
                    de.Floor,
                    de.ShutdownTime,
                    de.DeviceOnline,
                    de.FrontOnline,
                    de.Position,
                    de.Mark,
                    de.IsSyn,
                    de.IsShow,
                    de.ScreenshotSrc,
                    de.Version,
                    de.AddTime,
                    de.SystemType,
                    DeviceTypeCode = de.DeviceType,
                    DeviceType = dd.DictValue,
                    de.Operable

                }).OrderByDescending(i => i.AddTime).ToListAsync();


                if (model.Paging == null)
                {
                    model.Paging = 0;
                }

                if (model.PageIndex == null)
                {
                    model.PageIndex = 1;
                }

                if (model.PageSize == null)
                {
                    model.PageSize = 10;
                }

                if (model.Paging == 0)
                {
                    ArrayList devlist = new ArrayList();
                    foreach (var de in devices)
                    {
                        var dglist = await dbContext.DeviceToGroup.Where(i => i.DeviceCode == de.Code).Join(dbContext.DeviceGroup, dtg => dtg.GroupCode, dg => dg.Code, (dtg, dg) => new
                        {
                            dg.Code,
                            dg.GName


                        }).ToListAsync();

                        var devapp = await dbContext.DevAppOnline.Where(i => i.DeviceCode == de.Code && i.UpdateTime.AddMinutes(1) >= DateTime.Now).FirstOrDefaultAsync();
                        var AppVersion = string.Empty;
                        var AppName = string.Empty;
                        var ContainerVersion = string.Empty;
                        if (devapp != null)
                        {
                            AppVersion = devapp.AppVersion;
                            AppName = devapp.AppName;
                            ContainerVersion = devapp.ContainerVersion;
                        }
                        var devc = await dbContext.DeviceCoordinate.Where(i => i.DevCode == de.Code).FirstOrDefaultAsync();
                        devlist.Add(new
                        {
                            de.Code,
                            de.ID,
                            de.DevNum,
                            de.IP,
                            de.SName,
                            de.ScreenInfo,
                            de.Floor,
                            de.ShutdownTime,
                            de.DeviceOnline,
                            de.FrontOnline,
                            de.Position,
                            de.Mark,
                            de.IsSyn,
                            de.IsShow,
                            de.ScreenshotSrc,
                            de.Version,
                            de.SystemType,
                            de.DeviceTypeCode,
                            de.DeviceType,
                            de.Operable,
                            AppVersion,
                            AppName,
                            ContainerVersion,
                            IsSetPosition = devc == null ? 2 : 1,
                            GroupList = dglist
                        });
                    }
                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = devlist;
                }
                else
                {

                    int allPage = 1;
                    int allCount = devices.Count();
                    allPage = (int)(allCount / model.PageSize);
                    if (allCount % model.PageSize > 0)
                    {
                        allPage = allPage + 1;
                    }

                    devices = devices.Skip(((int)model.PageIndex - 1) * (int)model.PageSize).Take((int)model.PageSize).ToList();


                    ArrayList devlist = new ArrayList();
                    foreach (var de in devices)
                    {
                        var dglist = await dbContext.DeviceToGroup.Where(i => i.DeviceCode == de.Code).Join(dbContext.DeviceGroup, dtg => dtg.GroupCode, dg => dg.Code, (dtg, dg) => new
                        {
                            dg.Code,
                            dg.GName


                        }).ToListAsync();

                        var devapp = await dbContext.DevAppOnline.Where(i => i.DeviceCode == de.Code && i.UpdateTime.AddMinutes(1) >= DateTime.Now).FirstOrDefaultAsync();
                        var AppVersion = string.Empty;
                        var AppName = string.Empty;
                        var ContainerVersion = string.Empty;
                        if (devapp != null)
                        {
                            AppVersion = devapp.AppVersion;
                            AppName = devapp.AppName;
                            ContainerVersion = devapp.ContainerVersion;
                        }
                        var devc = await dbContext.DeviceCoordinate.Where(i => i.DevCode == de.Code).FirstOrDefaultAsync();

                        devlist.Add(new
                        {
                            de.Code,
                            de.ID,
                            de.DevNum,
                            de.IP,
                            de.SName,
                            de.ScreenInfo,
                            de.Floor,
                            de.ShutdownTime,
                            de.DeviceOnline,
                            de.FrontOnline,
                            de.Position,
                            de.Mark,
                            de.IsSyn,
                            de.IsShow,
                            de.ScreenshotSrc,
                            de.Version,
                            de.SystemType,
                            de.DeviceTypeCode,
                            de.DeviceType,
                            de.Operable,
                            AppVersion,
                            AppName,
                            ContainerVersion,
                            IsSetPosition = devc == null ? 2 : 1,
                            GroupList = dglist
                        });
                    }



                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = new { List = devlist, AllPage = allPage, AllCount = allCount };

                }



                return Json(_Result);
            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);
            }

        }

        [HttpPost]
        public async Task<IActionResult> GetTimeSoltList(Input_GetTSList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetTSList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            try
            {
                if (string.IsNullOrEmpty(model.DevCode))
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入设备编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                if (model.EffcTime < DateTime.Now.AddDays(-3).Date)
                {
                    _Result.Code = "510";
                    _Result.Msg = "仅支持最近三天记录查询";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var devcount = await dbContext.Device.Where(i => !i.IsDelete && i.Code == model.DevCode).CountAsync();
                if (devcount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的编码：" + model.DevCode;
                    _Result.Data = "";
                    return Json(_Result);
                }

                //   var tslist = await dbContext.PlayHistory.Where(i => i.DeviceCode == model.DevCode && i.EffcDate == model.EffcTime.Date).Select(s=> new {s.TimeSolt, s.BeginTimeSlot }).Distinct().GroupBy(g=>new { g.TimeSolt,g.BeginTimeSlot}).ToListAsync();


                var tslist = await dbContext.PlayHistory.Where(i => i.DeviceCode == model.DevCode && i.EffcDate == model.EffcTime.Date && i.Type == 1).Select(s => new { s.TimeSolt, s.BeginTimeSlot }).Distinct().OrderBy(o => o.BeginTimeSlot).ToListAsync();

                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = tslist;



                return Json(_Result);
            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);
            }

        }


        [HttpPost]
        public async Task<IActionResult> GetPlayHistory(Input_GetPHList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetPHList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            try
            {
                if (string.IsNullOrEmpty(model.DevCode))
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入设备编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                if (model.EffcTime < DateTime.Now.AddDays(-3).Date)
                {
                    _Result.Code = "510";
                    _Result.Msg = "仅支持最近三天记录查询";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var devcount = await dbContext.Device.Where(i => !i.IsDelete && i.Code == model.DevCode).CountAsync();
                if (devcount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的编码：" + model.DevCode;
                    _Result.Data = "";
                    return Json(_Result);
                }

                var phlist = await dbContext.PlayHistory.Where(i => i.DeviceCode == model.DevCode
        && i.EffcDate == model.EffcTime.Date
        && (String.IsNullOrEmpty(model.TimeSolt) ? true : i.TimeSolt == model.TimeSolt)
        ).Select(s => new
        {
            s.FileCode,
            FilePath = string.IsNullOrEmpty(s.FilePath) ? "" : Method.OSSServer + s.FilePath,
            s.ShopHouseNum,
            s.ShopName,
            IconPath = string.IsNullOrEmpty(s.IconPath) ? "" : Method.OSSServer + s.IconPath,
            s.ScheduleStart,
            s.ScheduleEnd,
            s.ScheduleCode,
            s.Order
        }).Distinct().OrderBy(o => o.ScheduleCode).ThenBy(o => o.Order).Select(s => new
        {
            s.FileCode,
            s.FilePath,
            s.ShopHouseNum,
            s.ShopName,
            s.IconPath,
            s.ScheduleStart,
            s.ScheduleEnd
        }).ToListAsync();

                //   phlist = phlist.Distinct().ToList();
                ArrayList arrayList = new ArrayList();
                foreach (var ph in phlist)
                {
                    var tslist = await dbContext.PlayHistory.Where(i => i.DeviceCode == model.DevCode && i.EffcDate == model.EffcTime.Date && i.FileCode == ph.FileCode && i.Type == 1).Select(s => new { s.TimeSolt, s.BeginTimeSlot }).Distinct().GroupBy(g => new { g.TimeSolt, g.BeginTimeSlot }).ToListAsync();

                    arrayList.Add(new
                    {
                        ph.FilePath,
                        ph.ShopHouseNum,
                        ph.ShopName,
                        ph.IconPath,
                        ph.ScheduleStart,
                        ph.ScheduleEnd,
                        TimeSolt = tslist
                    });
                }


                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = arrayList;



                return Json(_Result);
            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);
            }

        }


        [HttpPost]
        public async Task<IActionResult> GetLocalProgram(Input_GetLocalList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetLocalList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            try
            {
                if (string.IsNullOrEmpty(model.DevCode))
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入设备编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                if (model.EffcTime < DateTime.Now.AddDays(-3).Date)
                {
                    _Result.Code = "510";
                    _Result.Msg = "仅支持最近三天记录查询";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var devcount = await dbContext.Device.Where(i => !i.IsDelete && i.Code == model.DevCode).CountAsync();
                if (devcount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的编码：" + model.DevCode;
                    _Result.Data = "";
                    return Json(_Result);
                }

                //var programlist = await dbContext.DeviceToGroups.Where(i => i.DeviceCode == model.DevCode).Join(dbContext.ProgramDevices, d => d.GroupCode, pd => pd.DeviceGrounpCode,
                //    (p, pd) => new { pd.ProgramGrounpCode }).Join(dbContext.ProgramToGroups, pd => pd.ProgramGrounpCode, p => p.GroupCode, (pd, p) => new { p.ProgramCode }).Join(
                //    dbContext.Programs.Where(i=>i.LaunchTime <= model.EffcTime.Date && i.ExpiryDate >= model.EffcTime.Date.AddDays(1).AddMilliseconds(-1)), pd => pd.ProgramCode, p => p.Code, (pd, p) => new
                //    {
                //        p.LaunchTime,
                //        p.ExpiryDate,
                //        p.Code,
                //        p.ProgType
                //    }

                //    ).Join(dbContext.AssetFiles, p => p.Code, af => af.Code, (p, af) => new
                //    {
                //        p.LaunchTime,
                //        p.ExpiryDate,
                //        FilePath = Method.ServerAddr+  af.FilePath,
                //        p.ProgType

                //    }).ToListAsync();


                var programlist = await dbContext.DeviceToGroup.Where(i => i.DeviceCode == model.DevCode).Join(
                    dbContext.ProgramDevice, d => d.GroupCode, pd => pd.DeviceGrounpCode,
                    (d, pd) => new { pd.ProgramGrounpCode, pd.AddTime }).Join(dbContext.ProgramToGroup, pd => pd.ProgramGrounpCode, p => p.GroupCode, (pd, p) => new { p.ProgramCode, pd.AddTime, p.Order }).Join(
                     dbContext.Programs.Where(i => i.LaunchTime <= model.EffcTime.Date && i.ExpiryDate >= model.EffcTime.Date), pd => pd.ProgramCode, p => p.Code, (pd, p) => new
                     {
                         p.LaunchTime,
                         p.ExpiryDate,
                         p.Code,
                         p.ProgType,
                         pd.AddTime,
                         pd.Order
                     }

                    ).Join(dbContext.AssetFiles, p => p.Code, af => af.Code, (p, af) => new
                    {
                        p.LaunchTime,
                        p.ExpiryDate,
                        FilePath = Method.OSSServer + af.FilePath,
                        p.ProgType,
                        p.AddTime,
                        p.Order

                    }).OrderBy(p => p.AddTime).ThenBy(p => p.Order).ToListAsync();


                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = programlist;



                return Json(_Result);
            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);
            }

        }



        /// <summary>
        ///设置同屏属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SetSynStatus(Input_SetSynStatus model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_SetSynStatus)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            try
            {



                //用户权限判断

                //if (!Method.PermissionCheck(dbContext, this.HttpContext, "DeviceControl"))
                //{
                //    _Result.Code = "1";
                //    _Result.Msg = "您无权更改设备同步属性,请联系超级管理员";
                //    _Result.Data = "";
                //    return Json(_Result);
                //}


                if (string.IsNullOrEmpty(model.Code))
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入设备编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var devcount = await dbContext.Device.Where(i => !i.IsDelete && i.Code == model.Code).CountAsync();
                if (devcount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的编码：" + model.Code;
                    _Result.Data = "";
                    return Json(_Result);
                }
                QMLog log = new QMLog();
                if (model.Status == 0)
                {
                    //设置为非同步屏

                    //从同步设备组中移除
                    var devtogrp = await dbContext.DeviceToGroup.Where(i => i.DeviceCode == model.Code).Join(dbContext.DeviceGroup.Where(i => i.Type == 2), dtg => dtg.GroupCode, dg => dg.Code, (dtg, dg) => dtg).ToListAsync();

                    dbContext.DeviceToGroup.RemoveRange(devtogrp);

                    var dev = await dbContext.Device.Where(i => !i.IsDelete && i.Code == model.Code).FirstOrDefaultAsync();

                    dev.IsSyn = false;

                    var devShop = await dbContext.ShopToDevice.Where(i => i.DeviceCode == model.Code).ToListAsync();
                    dbContext.ShopToDevice.RemoveRange(devShop);

                    dbContext.Device.Update(dev);

                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        /*
                             string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                             string salt = "QianMu";
                             string key = salt + dtime;

                             string _EncryptionKey = Method.StringToPBKDF2Hash(key);
                             var url = Method.MallSite + "API/Device/SetSynStatus";

                             var data = new
                             {
                                 Method.CusID,
                                 CheckTime = dtime,
                                 Token = _EncryptionKey,
                                 model.Code,
                                 model.Status
                             };
                             var param = JsonConvert.SerializeObject(data);
                             param = Base64.EncodeBase64(param);

                             var _r = Method.PostMothsToObj(url, param);
                             if (_r.Code == "200")
                             {
                                 log.WriteLogToFile("设备[" + dev.Code + "]同屏设置成功", "");
                             }
                             else
                             {
                                 log.WriteLogToFile("设备[" + dev.Code + "]同屏设置失败", _r.Msg);
                             }
                             */
                        _Result.Code = "200";
                        _Result.Msg = "设置成功";
                        _Result.Data = "";
                    }
                    else
                    {
                        _Result.Code = "400";
                        _Result.Msg = "设置失败";
                        _Result.Data = "";
                    }



                }

                if (model.Status == 1)
                {
                    //设为同步屏


                    var dev = await dbContext.Device.Where(i => !i.IsDelete && i.Code == model.Code).FirstOrDefaultAsync();
                    //从非同步屏设备组中移除
                    var devtogrp = await dbContext.DeviceToGroup.Where(i => i.DeviceCode == model.Code).Join(dbContext.DeviceGroup.Where(i => i.Type == 1), dtg => dtg.GroupCode, dg => dg.Code, (dtg, dg) => dtg).ToListAsync();
                    dbContext.DeviceToGroup.RemoveRange(devtogrp);
                    //从专属设备中移除 小程序不可见
                    var shopdev = await dbContext.ShopToDevice.Where(i => i.DeviceCode == model.Code).ToListAsync();

                    dbContext.ShopToDevice.RemoveRange(shopdev);

                    dev.IsShow = false;

                    //取消相关排期订单

                    var scddev = await dbContext.ScheduleDevice.Where(i => i.DeviceCode == model.Code).ToListAsync();
                    dbContext.ScheduleDevice.RemoveRange(scddev);

                    dev.IsSyn = true;
                    dbContext.Device.Update(dev);



                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        /*
                             string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                             string salt = "QianMu";
                             string key = salt + dtime;

                             string _EncryptionKey = Method.StringToPBKDF2Hash(key);
                             var url = Method.MallSite + "API/Device/SetSynStatus";

                             var data = new
                             {
                                 Method.CusID,
                                 CheckTime = dtime,
                                 Token = _EncryptionKey,
                                 model.Code,
                                 model.Status
                             };
                             var param = JsonConvert.SerializeObject(data);
                             param = Base64.EncodeBase64(param);

                             var _r = Method.PostMothsToObj(url, param);
                             if (_r.Code == "200")
                             {
                                 log.WriteLogToFile("设备[" + dev.Code + "]同屏设置成功", "");
                             }
                             else
                             {
                                 log.WriteLogToFile("设备[" + dev.Code + "]同屏设置失败", _r.Msg);
                             }
                             */
                        _Result.Code = "200";
                        _Result.Msg = "设置成功";
                        _Result.Data = "";
                    }
                    else
                    {
                        _Result.Code = "400";
                        _Result.Msg = "设置失败";
                        _Result.Data = "";
                    }


                }




                return Json(_Result);
            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);
            }

        }

        /*
             */
        /// <summary>
        ///设置同屏属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SetOperable(Input_SetOperable model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_SetOperable)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            try
            {
                if (string.IsNullOrEmpty(model.Code))
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入设备编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var devcount = await dbContext.Device.Where(i => !i.IsDelete && i.Code == model.Code).CountAsync();
                if (devcount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的编码：" + model.Code;
                    _Result.Data = "";
                    return Json(_Result);
                }
                QMLog log = new QMLog();

                var dev = await dbContext.Device.Where(i => !i.IsDelete && i.Code == model.Code).FirstOrDefaultAsync();


                if (model.Status == 0)
                {
                    dev.Operable = false;
                }

                if (model.Status == 1)
                {
                    dev.Operable = true;
                }
                dbContext.Device.Update(dev);

                if (await dbContext.SaveChangesAsync() >= 0)
                {
                    _Result.Code = "200";
                    _Result.Msg = "设置成功";
                    _Result.Data = "";
                }
                else
                {
                    _Result.Code = "400";
                    _Result.Msg = "设置失败";
                    _Result.Data = "";
                }



                return Json(_Result);
            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);
            }

        }

        /// <summary>
        /// 获取设备组对应的设备信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDeviceByGroupCode(Input_GetDevByGroup model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetDevByGroup)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (uol == null || string.IsNullOrEmpty(uol.UserCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请先登录";
                _Result.Data = "";
                return Json(_Result);
            }
            String MallCode = model.MallCode;
            if (string.IsNullOrEmpty(MallCode))
            {
                MallCode = uol.MallCode;
            }
            try
            {
                var deviceList = await dbContext.Device.Where(i => i.MallCode == MallCode && i.IsSyn == model.IsSyn && !i.IsDelete && i.ScreenInfo == model.ScreenCode).Join(dbContext.ScreenInfo, d => d.ScreenInfo, s => s.Code, (d, s) => new {
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
                }).Join(dbContext.Building, d => d.Building, b => b.Code, (d, b) => new {
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

                }).Join(dbContext.Floor, d => d.Floor, f => f.Code, (d, f) => new {
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
                if (model.IsSyn)
                {
                    var alreadyCodes = await dbContext.DeviceToGroup.Where(i => i.GroupCode != model.GroupCode).Select(i => i.DeviceCode).Distinct().ToListAsync();
                    deviceList = deviceList.Where(i => !alreadyCodes.Contains(i.Code)).ToList();
                }
                if (!string.IsNullOrEmpty(model.GroupCode))
                {
                    var groupDeviceList = deviceList.Join(dbContext.DeviceToGroup.Where(i => i.GroupCode == model.GroupCode), d => d.Code, dg => dg.DeviceCode, (d, dg) => d).ToList();
                    var unGroupDeviceList = deviceList.Except(groupDeviceList).ToList();
                    var group = dbContext.DeviceGroup.Where(i => i.Code == model.GroupCode).FirstOrDefault();
                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = new { UnGroupDevice = unGroupDeviceList, GroupDevice = groupDeviceList, Group = group };
                    return Json(_Result);
                }
                else
                {
                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = new { UnGroupDevice = deviceList, GroupDevice = new ArrayList(), Group = new ArrayList() };
                    return Json(_Result);
                }
            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);
            }
        }


        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetAppDev(Input_GetAppDev model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetAppDev)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (uol == null || string.IsNullOrEmpty(uol.UserCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请先登录";
                _Result.Data = "";
                return Json(_Result);
            }
            String MallCode = model.MallCode;
            if (string.IsNullOrEmpty(MallCode))
            {
                MallCode = uol.MallCode;
            }
            try
            {

                var dcodelist = await dbContext.AppDev.Join(dbContext.ApplicationNew.Where(i => i.MallCode == MallCode), ad => ad.AppCode, an => an.Code, (ad, an) => ad).Join(dbContext.Device.Where(i => i.MallCode == MallCode), ad => ad.DevCode, d => d.Code, (ad, d) => new { ad.DevCode }).ToListAsync();
                dcodelist = dcodelist.Distinct().ToList();
                ArrayList arrayList = new ArrayList();


                if (model.Paging == null)
                {
                    model.Paging = 0;
                }

                if (model.PageIndex == null)
                {
                    model.PageIndex = 1;
                }

                if (model.PageSize == null)
                {
                    model.PageSize = 10;
                }

                if (model.Paging == 0)
                {
                    foreach (var dcode in dcodelist)
                    {
                        var device = await dbContext.Device.Where(i => i.Code == dcode.DevCode).Join(dbContext.Floor, de => de.Floor, fl => fl.Code, (de, fl) => new {
                            de.DevNum,
                            de.IP,
                            FloorName = fl.Name,
                            de.Building
                        }).Join(dbContext.Building, de => de.Building, bu => bu.Code, (de, bu) => new {
                            de.DevNum,
                            de.IP,
                            de.FloorName,
                            Bname = bu.Name

                        }).FirstOrDefaultAsync();
                        var AppCount = await dbContext.AppDev.Where(i => i.DevCode == dcode.DevCode).Join(dbContext.ApplicationNew, ad => ad.AppCode, an => an.Code, (ad, an) => ad).CountAsync();
                        var defaultapp = await dbContext.AppDev.Where(i => i.DevCode == dcode.DevCode && i.Default).Join(dbContext.ApplicationNew, ad => ad.AppCode, an => an.Code, (ad, an) => new {
                            an.Name
                        }).FirstOrDefaultAsync();
                        if (device != null)
                        {

                            arrayList.Add(new
                            {
                                device.DevNum,
                                device.IP,
                                Floor = device.Bname + "/" + device.FloorName,
                                AppCount,
                                DefaultApp = defaultapp == null ? "" : defaultapp.Name,
                                dcode.DevCode

                            });
                        }
                    }
                }
                else
                {

                    int allPage = 1;
                    int allCount = dcodelist.Count();
                    allPage = (int)(allCount / model.PageSize);
                    if (allCount % model.PageSize > 0)
                    {
                        allPage = allPage + 1;
                    }

                    dcodelist = dcodelist.Skip(((int)model.PageIndex - 1) * (int)model.PageSize).Take((int)model.PageSize).ToList();



                    foreach (var dcode in dcodelist)
                    {
                        var device = await dbContext.Device.Where(i => i.Code == dcode.DevCode).Join(dbContext.Floor, de => de.Floor, fl => fl.Code, (de, fl) => new {
                            de.DevNum,
                            de.IP,
                            FloorName = fl.Name,
                            de.Building,

                        }).Join(dbContext.Building, de => de.Building, bu => bu.Code, (de, bu) => new {
                            de.DevNum,
                            de.IP,
                            de.FloorName,
                            Bname = bu.Name

                        }).FirstOrDefaultAsync();
                        var AppCount = await dbContext.AppDev.Where(i => i.DevCode == dcode.DevCode).Join(dbContext.ApplicationNew, ad => ad.AppCode, an => an.Code, (ad, an) => ad).CountAsync();
                        var defaultapp = await dbContext.AppDev.Where(i => i.DevCode == dcode.DevCode && i.Default).Join(dbContext.ApplicationNew, ad => ad.AppCode, an => an.Code, (ad, an) => new {
                            AppName = an.Name
                        }).FirstOrDefaultAsync();
                        if (device != null)
                        {
                            arrayList.Add(new
                            {
                                device.DevNum,
                                device.IP,
                                Floor = device.Bname + "//" + device.FloorName,
                                AppCount,
                                DefaultApp = defaultapp == null ? "" : defaultapp.AppName,
                                dcode.DevCode
                            });
                        }
                    }

                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = new { List = arrayList, AllPage = allPage, AllCount = allCount };

                }





                return Json(_Result);
            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);
            }

        }


        /// <summary>
        /// 获取楼层设备订单数
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDevFlow(Input_GetDevFlow model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetDevFlow)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (uol == null || string.IsNullOrEmpty(uol.UserCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请先登录";
                _Result.Data = "";
                return Json(_Result);
            }
            String MallCode = string.Empty;
            if (string.IsNullOrEmpty(MallCode))
            {
                MallCode = uol.MallCode;
            }
            try
            {
                var floorcount = await dbContext.Floor.Where(i => i.Code == model.Floor).CountAsync();

                if (floorcount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的楼层编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                SqlParameter[] parameters = {
                     new SqlParameter("@MallCode", SqlDbType.VarChar,50),
                   new SqlParameter("@Floor", SqlDbType.VarChar,50),  //自定义参数  与参数类型    
                   new SqlParameter("@BeginDate", SqlDbType.Date),
                    new SqlParameter("@EndDate", SqlDbType.Date) };

                parameters[0].Value = uol.MallCode;
                parameters[1].Value = model.Floor;  //给参数赋值
                parameters[2].Value = DateTime.Now.AddDays(-1).Date;
                parameters[3].Value = DateTime.Now.Date;

                var list = await dbContext.Output_DevFlow.FromSql(@"select a.Code ,a.DevNum,count(b.ScheduleCode) as ScheduleNum ,d.Xaxis,d.Yaxis,d.Angle from  Device a 
                                                                   left join ScheduleDevice b on a.Code = b.DeviceCode 
                                                                   left join ScheduleDate c on b.ScheduleCode = c.ScheduleCode 
                                                                   left join DeviceCoordinate d on a.Code = d.DevCode
                                                                   where a.MallCode=@MallCode and a.IsDelete = 0  and a.[Floor] = @Floor and c.ScheduleDay >= @BeginDate and  c.ScheduleDay <=  @EndDate
                                                                   group by a.Code,a.DevNum,d.Xaxis,d.Yaxis,d.Angle ", parameters).ToListAsync();


                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = list;
                return Json(_Result);
            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);
            }

        }



        /// <summary>
        /// 获取设备订单Top 10
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetTopDevFlow([FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (uol == null || string.IsNullOrEmpty(uol.UserCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请先登录";
                _Result.Data = "";
                return Json(_Result);
            }
            String MallCode = string.Empty;
            if (string.IsNullOrEmpty(MallCode))
            {
                MallCode = uol.MallCode;
            }

            try
            {


                SqlParameter[] parameters = {
                   new SqlParameter("@MallCode", SqlDbType.VarChar,50),
                   new SqlParameter("@BeginDate", SqlDbType.Date),
                    new SqlParameter("@EndDate", SqlDbType.Date) };

                parameters[0].Value = uol.MallCode;
                parameters[1].Value = DateTime.Now.AddDays(-30).Date;
                parameters[2].Value = DateTime.Now.Date;

                var list = await dbContext.Output_TopDevFlow.FromSql(@"select top  10 a.Code ,a.DevNum,count(b.ScheduleCode) as ScheduleNum from  Device a left join ScheduleDevice b on a.Code = b.DeviceCode left join ScheduleDate c on b.ScheduleCode = c.ScheduleCode
                                                                   where a.MallCode=@MallCode and a.IsDelete = 0  and c.ScheduleDay >= @BeginDate and  c.ScheduleDay <=  @EndDate
                                                                   group by a.Code,a.DevNum
                                                                   order by ScheduleNum desc", parameters).ToListAsync();


                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = list;
                return Json(_Result);
            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);
            }

        }


        public async Task<IActionResult> GetServerTime([FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            long currentTicks = DateTime.Now.Ticks;
            DateTime dtFrom = new DateTime(1970, 1, 1, 8, 0, 0, 0);
            long currentMillis = (currentTicks - dtFrom.Ticks) / 10000;
            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = currentMillis;
            return Json(_Result);
        }

        [HttpPost]
        public async Task<IActionResult> ScreenOper(Input_ScreenOper model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_ScreenOper)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


            var device = await dbContext.Device.Where(i => i.Code == model.Code).FirstOrDefaultAsync();
            if (device == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的设备编码";
                _Result.Data = "";
                return Json(_Result);
            }
            if (!device.DeviceOnline)
            {
                _Result.Code = "510";
                _Result.Msg = "设备不在线，不能进行相关操作";
                _Result.Data = "";
                return Json(_Result);
            }
            var Type = model.Type ? "TurnOn" : "TurnOff";
            MsgTemplate msg = new MsgTemplate();
            msg.SenderID = Method.ServerAddr;
            msg.ReceiverID = device.Code;
            msg.MessageType = "json";
            msg.Content = new { Type = Type };
            _Result.Code = "200";
            _Result.Msg = "设置成功";
            _Result.Data = "";
            //return Json(_Result);
            await Method.SendMsgAsync(msg);
            return Json(_Result);
        }



    }
}