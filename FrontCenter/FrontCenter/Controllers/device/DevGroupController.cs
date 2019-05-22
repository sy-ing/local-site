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

namespace FrontCenter.Controllers.device
{
    public class DevGroupController : Controller
    {
        /// <summary>
        /// 获取设备组列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetList(Input_GetDevGroupList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            UserOnLine uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (uol == null || string.IsNullOrEmpty(uol.UserName))
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
            model = (Input_GetDevGroupList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            try
            {

                var list = await dbContext.DeviceGroup.Where(i => i.MallCode == uol.MallCode && i.GName.Contains(model.SearchKey)).Join(dbContext.ScreenInfo.Where(i => i.MallCode == uol.MallCode), dg => dg.ScreenInfoCode, si => si.Code, (dg, si) => new
                {
                    dg.Code,
                    dg.GName,
                    dg.ScreenInfoCode,
                    dg.Type,
                    si.SName,
                    dg.AddTime,
                    TypeStr = dg.Type == 1 ? "正常" : "同步"
                }).OrderByDescending(i => i.AddTime).ToListAsync();

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

                    foreach (var dg in list)
                    {
                        var DevCount = await dbContext.DeviceToGroup.Where(i => i.GroupCode == dg.Code).AsNoTracking().CountAsync();
                        //var DevCount = await dbContext.DeviceToGroup.Where(i => i.MallCode == uol.MallCode && i.GroupCode == dg.Code).AsNoTracking().CountAsync();
                        var PGCount = await dbContext.ProgramDevice.Where(i => i.DeviceGrounpCode == dg.Code).AsNoTracking().CountAsync();
                        var SubtitleCount = await dbContext.SubtitleToDeviceGroup.Where(i => i.GroupCode == dg.Code).AsNoTracking().CountAsync();
                        arrayList.Add(new
                        {

                            dg.Code,
                            dg.GName,
                            dg.ScreenInfoCode,
                            dg.Type,
                            dg.SName,
                            dg.TypeStr,
                            DevCount,
                            PGCount,
                            SubtitleCount
                        });
                    }
                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = arrayList;

                }
                else
                {

                    int allPage = 1;
                    int allCount = list.Count();
                    allPage = (int)(allCount / model.PageSize);
                    if (allCount % model.PageSize > 0)
                    {
                        allPage = allPage + 1;
                    }

                    list = list.Skip(((int)model.PageIndex - 1) * (int)model.PageSize).Take((int)model.PageSize).ToList();



                    foreach (var dg in list)
                    {
                        var DevCount = await dbContext.DeviceToGroup.Where(i => i.GroupCode == dg.Code).AsNoTracking().CountAsync();
                        // var DevCount = await dbContext.DeviceToGroup.Where(i => i.MallCode == uol.MallCode && i.GroupCode == dg.Code).AsNoTracking().CountAsync();
                        var PGCount = await dbContext.ProgramDevice.Where(i => i.DeviceGrounpCode == dg.Code).AsNoTracking().CountAsync();
                        var SubtitleCount = await dbContext.SubtitleToDeviceGroup.Where(i => !i.IsDel && i.GroupCode == dg.Code).AsNoTracking().CountAsync();
                        arrayList.Add(new
                        {

                            dg.Code,
                            dg.GName,
                            dg.ScreenInfoCode,
                            dg.Type,
                            dg.SName,
                            dg.TypeStr,
                            DevCount,
                            PGCount,
                            SubtitleCount
                        });
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
                _Result.Msg = "" + e.ToString();
                _Result.Data = "";
                return Json(_Result);

            }
        }


        /// <summary>
        /// 获取设备组列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDevGroupList(Input_GetDevGroupListNew model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetDevGroupListNew)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            UserOnLine uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (uol == null || string.IsNullOrEmpty(uol.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
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
                var list = await dbContext.Output_GetDevGroupList.FromSql(@"select 
                                                                                  a.Code
                                                                                  ,a.GName
                                                                                  ,a.ScreenInfoCode
                                                                                  ,a.[Type]
                                                                                  ,b.SName
                                                                                  ,case a.[Type] when 1 then '正常' else '同步' end as TypeStr 
                                                                                  ,count(distinct(c.Code)) as DevCount
                                                                                  ,count(distinct(d.Code)) as PGCount from DeviceGroup a 
                                                                                  left join ScreenInfo b on a.ScreenInfoCode = b.Code 
                                                                                  left join DeviceToGroup c on a.Code = c.GroupCode 
                                                                                  left join ProgramDevice d on d.DeviceGrounpCode = a.Code
                                                                                  where a.MallCode = @MallCode
                                                                                  group by
                                                                                  a.Code
                                                                                  ,a.GName
                                                                                  ,a.ScreenInfoCode
                                                                                  ,a.[Type]
                                                                                  ,b.SName
                                                                                  order by DevCount desc ,PGCount desc
                                                                                  ", new SqlParameter("@MallCode", MallCode)).AsNoTracking().ToListAsync();

                list = list.Where(i => (string.IsNullOrEmpty(model.SearchKey) || i.GName.Contains(model.SearchKey))).ToList();


                if (model.Order.ToUpper() == "DESC")
                {
                    list = list.OrderByDescending(o => o.DevCount).ToList();
                }

                if (model.Order.ToUpper() == "ASC")
                {
                    list = list.OrderBy(o => o.DevCount).ToList();
                }
                //var list = await dbContext.DeviceGroup.Where(i=>i.GName.Contains(model.SearchKey)).Join(dbContext.ScreenInfo,dg=>dg.ScreenInfoCode,si=>si.Code,(dg,si)=>new {
                //    dg.Code,
                //    dg.GName,
                //    dg.ScreenInfoCode,
                //    dg.Type,
                //    si.SName,
                //    dg.AddTime,
                //    TypeStr = dg.Type == 1?"正常":"同步"
                //}).OrderByDescending(i=>i.AddTime).ToListAsync();

                // ArrayList arrayList = new ArrayList();


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

                    //foreach (var dg in list)
                    //{
                    //    var DevCount = await dbContext.DeviceToGroup.Where(i => i.GroupCode == dg.Code).CountAsync();
                    //    var PGCount = await dbContext.ProgramDevice.Where(i => i.DeviceGrounpCode == dg.Code).CountAsync();

                    //    arrayList.Add(new
                    //    {

                    //        dg.Code,
                    //        dg.GName,
                    //        dg.ScreenInfoCode,
                    //        dg.Type,
                    //        dg.SName,
                    //        dg.TypeStr,
                    //        DevCount,
                    //        PGCount
                    //    });
                    //}
                    //_Result.Code = "200";
                    //_Result.Msg = "获取成功";
                    //_Result.Data = arrayList;
                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = list;
                }
                else
                {

                    int allPage = 1;
                    int allCount = list.Count();
                    allPage = (int)(allCount / model.PageSize);
                    if (allCount % model.PageSize > 0)
                    {
                        allPage = allPage + 1;
                    }

                    list = list.Skip(((int)model.PageIndex - 1) * (int)model.PageSize).Take((int)model.PageSize).ToList();



                    //foreach (var dg in list)
                    //{
                    //    var DevCount = await dbContext.DeviceToGroup.Where(i => i.GroupCode == dg.Code).CountAsync();
                    //    var PGCount = await dbContext.ProgramDevice.Where(i => i.DeviceGrounpCode == dg.Code).CountAsync();

                    //    arrayList.Add(new
                    //    {

                    //        dg.Code,
                    //        dg.GName,
                    //        dg.ScreenInfoCode,
                    //        dg.Type,
                    //        dg.SName,
                    //        dg.TypeStr,
                    //        DevCount,
                    //        PGCount
                    //    });
                    //}


                    //_Result.Code = "200";
                    //_Result.Msg = "获取成功";
                    //_Result.Data = new { List = arrayList, AllPage = allPage, AllCount = allCount };
                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = new { List = list, AllPage = allPage, AllCount = allCount };

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

        /// <summary>
        /// 创建设备组
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddDeviceGroup(Input_DeviceGroupNew model, [FromServices] ContextString dbContext)
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

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_DeviceGroupNew)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            UserOnLine uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (uol == null || string.IsNullOrEmpty(uol.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }
            String MallCode = model.MallCode;
            if (string.IsNullOrEmpty(MallCode))
            {
                MallCode = uol.MallCode;
            }
            //检测用户输入格式
            if (!ModelState.IsValid)
            {
                _Result.Code = "510";
                _Result.Msg = "请求信息不正确";
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

            if (string.IsNullOrEmpty(model.GName))
            {
                _Result.Code = "510";
                _Result.Msg = "名称不可为空";
                _Result.Data = "";
                return Json(_Result);
            }

            //判断屏幕类型是否有效

            int sCount = dbContext.ScreenInfo.Where(i => i.Code == model.ScreenInfoCode).Count();
            if (sCount <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "未知的屏幕类型";
                _Result.Data = "";
                return Json(_Result);
            }

            //判断设备组是否已存在
            int count = dbContext.DeviceGroup.Where(i => i.MallCode == MallCode && i.GName == model.GName).Count();
            if (count > 0)
            {
                _Result.Code = "1";
                _Result.Msg = "设备组已存在";
                _Result.Data = "";
                return Json(_Result);
            }

            //检查组中的设备

            if (model.Devices.Count() > 0)
            {
                var errorNoCodeList = new ArrayList();
                var errorScreenList = new ArrayList();
                var errorSyncList = new ArrayList();
                var errorExistList = new ArrayList();
                //判断ID是否都为有效设备
                foreach (var item in model.Devices)
                {
                    var dev = await dbContext.Device.Where(i => i.Code == item).FirstOrDefaultAsync();
                    if (dev == null)
                    {
                        errorNoCodeList.Add(item);
                    }
                    if (dev.ScreenInfo.ToLower() != model.ScreenInfoCode.ToLower())
                    {
                        errorScreenList.Add(dev.IP);
                    }
                    if (dev.IsSyn != model.IsSync)
                    {
                        errorSyncList.Add(dev.DevNum);
                    }

                    if (model.IsSync)
                    {
                        var dgnum = await dbContext.DeviceToGroup.Where(i => i.DeviceCode == item).CountAsync();
                        if (dgnum > 0)
                        {
                            errorExistList.Add(dev.DevNum);
                        }
                    }
                    else
                    {
                        var dgnum = await dbContext.DeviceToGroup.Where(i => i.DeviceCode == item).Join(dbContext.DeviceGroup.Where(i => i.MallCode == MallCode && i.Type == 2), d => d.GroupCode, g => g.Code, (d, g) => d).CountAsync();
                        if (dgnum > 0)
                        {
                            errorExistList.Add(dev.DevNum);
                        }
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
                    _Result.Msg = "IP为：" + string.Join(";", (string[])errorScreenList.ToArray(typeof(string))) + "的设备屏幕分辨率与设备组分辨率不一致";
                    _Result.Data = "";
                    return Json(_Result);
                }
                if (errorSyncList.Count > 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "设备名：" + string.Join(";", (string[])errorSyncList.ToArray(typeof(string))) + "的设备同步属性与设备组属性不一致";
                    _Result.Data = "";
                    return Json(_Result);
                }
                if (errorExistList.Count > 0)
                {
                    _Result.Code = "510";
                    if (model.IsSync)
                    {
                        _Result.Msg = "设备名为：" + string.Join(";", (string[])errorExistList.ToArray(typeof(string))) + "的设备已存在于其他设备组";
                    }
                    else
                    {
                        _Result.Msg = "设备名为：" + string.Join(";", (string[])errorExistList.ToArray(typeof(string))) + "的设备已存在于其他同步设备组";
                    }
                    _Result.Data = "";
                    return Json(_Result);
                }
            }


            //添加设备组
            DeviceGroup _DG = new DeviceGroup();
            _DG.MallCode = MallCode;
            _DG.AddTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _DG.ScreenInfoCode = model.ScreenInfoCode;
            _DG.GName = model.GName;
            _DG.Code = Guid.NewGuid().ToString();
            _DG.UpdateTime = DateTime.Now;
            _DG.Type = model.IsSync ? 2 : 1;
            dbContext.DeviceGroup.Add(_DG);


            if (await dbContext.SaveChangesAsync() > 0)
            {
                //添加设备到设备组
                if (model.Devices.Count > 0)
                {
                    foreach (var item in model.Devices)
                    {
                        dbContext.DeviceToGroup.Add(new DeviceToGroup { AddTime = DateTime.Now, DeviceCode = item, GroupCode = _DG.Code, Code = Guid.NewGuid().ToString(), UpdateTime = DateTime.Now });
                    }

                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        _Result.Code = "200";
                        _Result.Msg = "添加成功";
                        _Result.Data = "";
                        // var _thisjobId = BackgroundJob.Schedule(() => SynData.SendDevInfo(), TimeSpan.FromSeconds(1));
                        // var ip = Method.GetUserIp(this.HttpContext);
                        //  dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "设备组模块", LogMsg = username + "添加了设备组：" + model.GName, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "创建", IP = ip });
                        //dbContext.SaveChanges();
                    }
                    else
                    {
                        _Result.Code = "4";
                        _Result.Msg = "添加设备到设备组组失败";
                        _Result.Data = "";
                    }
                }
                else
                {
                    _Result.Code = "200";
                    _Result.Msg = "添加成功";
                    _Result.Data = "";
                    // var _thisjobId = BackgroundJob.Schedule(() => SynData.SendDevInfo(), TimeSpan.FromSeconds(1));
                }



            }
            else
            {
                _Result.Code = "5";
                _Result.Msg = "添加设备组失败";
                _Result.Data = "";
            }
            return Json(_Result);
        }

        /// <summary>
        /// 获取设备组信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetGroupInfo(Input_GetDevGroupInfo model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetDevGroupInfo)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            //判断输入条件
            if (string.IsNullOrEmpty(model.Code))
            {
                _Result.Code = "510";
                _Result.Msg = "未能检测到编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var grounps = await dbContext.DeviceGroup.Where(i => i.Code == model.Code).Join(dbContext.ScreenInfo, dg => dg.ScreenInfoCode, s => s.Code, (dg, s) => new {
                dg.GName,
                dg.ScreenInfoCode,
                IsSync = dg.Type == 1 ? false : true,
                s.SName,
                TypeStr = dg.Type == 1 ? "正常" : "同步"
            }).FirstOrDefaultAsync();
            if (grounps != null)
            {
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = grounps;
            }
            else
            {
                _Result.Code = "1";
                _Result.Msg = "无效的设备组";
                _Result.Data = "";

            }
            return Json(_Result);
        }

        /// <summary>
        /// 更新设备组信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateGroupInfo(Input_GroupInfoNew model, [FromServices] ContextString dbContext)
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

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GroupInfoNew)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            //判断设备组是否有效
            var grounps = await dbContext.DeviceGroup.Where(i => i.Code == model.Code).ToListAsync();

            //更新信息
            if (grounps.Count > 0)
            {
                var group = grounps.FirstOrDefault();

                //检查组中的设备

                if (model.Devices.Count() > 0)
                {
                    var errorNoCodeList = new ArrayList();
                    var errorScreenList = new ArrayList();
                    var errorSyncList = new ArrayList();
                    var errorExistList = new ArrayList();
                    //判断ID是否都为有效设备
                    foreach (var item in model.Devices)
                    {
                        var dev = await dbContext.Device.Where(i => i.Code == item).FirstOrDefaultAsync();
                        if (dev == null)
                        {
                            errorNoCodeList.Add(item);
                        }
                        if (dev.ScreenInfo.ToLower() != group.ScreenInfoCode.ToLower())
                        {
                            errorScreenList.Add(dev.IP);
                        }
                        if (dev.IsSyn != model.IsSync)
                        {
                            errorSyncList.Add(dev.DevNum);
                        }

                        if (model.IsSync)
                        {
                            var dgnum = await dbContext.DeviceToGroup.Where(i => i.DeviceCode == item && i.GroupCode != model.Code).CountAsync();
                            if (dgnum > 0)
                            {
                                errorExistList.Add(dev.DevNum);
                            }
                        }
                        else
                        {
                            var dgnum = await dbContext.DeviceToGroup.Where(i => i.DeviceCode == item && i.GroupCode != model.Code).Join(dbContext.DeviceGroup.Where(i => i.Type == 2 && i.Code != model.Code), d => d.GroupCode, g => g.Code, (d, g) => d).CountAsync();
                            if (dgnum > 0)
                            {
                                errorExistList.Add(dev.DevNum);
                            }
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
                        _Result.Msg = "IP为：" + string.Join(";", (string[])errorScreenList.ToArray(typeof(string))) + "的设备屏幕分辨率与设备组分辨率不一致";
                        _Result.Data = "";
                        return Json(_Result);
                    }
                    if (errorSyncList.Count > 0)
                    {
                        _Result.Code = "510";
                        _Result.Msg = "设备名：" + string.Join(";", (string[])errorSyncList.ToArray(typeof(string))) + "的设备同步属性与设备组属性不一致";
                        _Result.Data = "";
                        return Json(_Result);
                    }
                    if (errorExistList.Count > 0)
                    {
                        _Result.Code = "510";
                        if (model.IsSync)
                        {
                            _Result.Msg = "设备名为：" + string.Join(";", (string[])errorExistList.ToArray(typeof(string))) + "的设备已存在于其他设备组";
                        }
                        else
                        {
                            _Result.Msg = "设备名为：" + string.Join(";", (string[])errorExistList.ToArray(typeof(string))) + "的设备已存在于其他同步设备组";
                        }
                        _Result.Data = "";
                        return Json(_Result);
                    }
                }

                //获取设备组 到设备关系列表
                var groupdevices = await dbContext.DeviceToGroup.Where(i => i.GroupCode == model.Code).ToListAsync();


                List<DeviceToGroup> _OldDTG = new List<DeviceToGroup>();//待移除的设备
                List<string> olddevice = new List<string>();//数据库中已包含的设备
                List<DeviceToGroup> _NewDTG = new List<DeviceToGroup>();//待添加的设备

                //列表中不包含 移除
                foreach (var gd in groupdevices)
                {

                    if (!model.Devices.Contains(gd.DeviceCode))
                    {
                        _OldDTG.Add(gd);
                    }
                    else
                    {
                        olddevice.Add(gd.DeviceCode);
                    }
                }

                //列表中不存在 添加
                foreach (var device in model.Devices)
                {

                    if (!olddevice.Contains(device))
                    {
                        _NewDTG.Add(new DeviceToGroup { AddTime = DateTime.Now, DeviceCode = device, GroupCode = group.Code, Code = Guid.NewGuid().ToString(), UpdateTime = DateTime.Now });
                    }
                }

                //修改设备组名称
                group.GName = model.GName;
                dbContext.DeviceGroup.Update(group);
                dbContext.DeviceToGroup.RemoveRange(_OldDTG);
                dbContext.DeviceToGroup.AddRange(_NewDTG);

                //操作成功
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    _Result.Code = "200";
                    _Result.Msg = "更新设备组信息成功";
                    _Result.Data = "";
                    //var ip = Method.GetUserIp(this.HttpContext);
                    //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "设备组模块", LogMsg = username + "更新设备组：" + group.GName + "的信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });

                    // dbContext.SaveChanges();
                    // var _thisjobId = BackgroundJob.Schedule(() => SynData.SendDevInfo(), TimeSpan.FromSeconds(1));

                }
                else
                {
                    _Result.Code = "3";
                    _Result.Msg = "更新设备组信息失败";
                    _Result.Data = "";
                }


            }
            else
            {
                _Result.Code = "4";
                _Result.Msg = "无效的设备组";
                _Result.Data = "";

            }

            //返回操作结果
            return Json(_Result);

        }

        /// <summary>
        /// 创建设备组
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PublishProgramToDevice(Input_ProgramToDevice model, [FromServices] ContextString dbContext)
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

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_ProgramToDevice)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            //检测用户输入格式
            if (!ModelState.IsValid)
            {
                _Result.Code = "510";
                _Result.Msg = "请求信息不正确";
                _Result.Data = "";
                return Json(_Result);
            }

            if (string.IsNullOrEmpty(model.GName))
            {
                _Result.Code = "510";
                _Result.Msg = "名称不可为空";
                _Result.Data = "";
                return Json(_Result);
            }

            //判断屏幕类型是否有效

            int sCount = dbContext.ScreenInfo.Where(i => i.Code == model.ScreenInfoCode).Count();
            if (sCount <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "未知的屏幕类型";
                _Result.Data = "";
                return Json(_Result);
            }

            //判断设备组是否已存在
            int count = dbContext.DeviceGroup.Where(i => i.GName == model.GName).Count();
            if (count > 0)
            {
                _Result.Code = "1";
                _Result.Msg = "设备组已存在";
                _Result.Data = "";
                return Json(_Result);
            }

            //检查组中的设备

            if (model.Devices.Count() > 0)
            {
                var errorNoCodeList = new ArrayList();
                var errorScreenList = new ArrayList();
                var errorSyncList = new ArrayList();
                var errorExistList = new ArrayList();
                //判断ID是否都为有效设备
                foreach (var item in model.Devices)
                {
                    var dev = await dbContext.Device.Where(i => i.Code == item).FirstOrDefaultAsync();
                    if (dev == null)
                    {
                        errorNoCodeList.Add(item);
                    }
                    if (dev.ScreenInfo.ToLower() != model.ScreenInfoCode.ToLower())
                    {
                        errorScreenList.Add(dev.IP);
                    }
                    if (dev.IsSyn != model.IsSync)
                    {
                        errorSyncList.Add(dev.DevNum);
                    }

                    if (model.IsSync)
                    {
                        var dgnum = await dbContext.DeviceToGroup.Where(i => i.DeviceCode == item).CountAsync();
                        if (dgnum > 0)
                        {
                            errorExistList.Add(dev.DevNum);
                        }
                    }
                    else
                    {
                        var dgnum = await dbContext.DeviceToGroup.Where(i => i.DeviceCode == item).Join(dbContext.DeviceGroup.Where(i => i.Type == 2), d => d.GroupCode, g => g.Code, (d, g) => d).CountAsync();
                        if (dgnum > 0)
                        {
                            errorExistList.Add(dev.DevNum);
                        }
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
                    _Result.Msg = "IP为：" + string.Join(";", (string[])errorScreenList.ToArray(typeof(string))) + "的设备屏幕分辨率与设备组分辨率不一致";
                    _Result.Data = "";
                    return Json(_Result);
                }
                if (errorSyncList.Count > 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "设备名：" + string.Join(";", (string[])errorSyncList.ToArray(typeof(string))) + "的设备同步属性与设备组属性不一致";
                    _Result.Data = "";
                    return Json(_Result);
                }
                if (errorExistList.Count > 0)
                {
                    _Result.Code = "510";
                    if (model.IsSync)
                    {
                        _Result.Msg = "设备名为：" + string.Join(";", (string[])errorExistList.ToArray(typeof(string))) + "的设备已存在于其他设备组";
                    }
                    else
                    {
                        _Result.Msg = "设备名为：" + string.Join(";", (string[])errorExistList.ToArray(typeof(string))) + "的设备已存在于其他同步设备组";
                    }
                    _Result.Data = "";
                    return Json(_Result);
                }
            }


            //添加设备组
            DeviceGroup _DG = new DeviceGroup();
            _DG.AddTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _DG.ScreenInfoCode = model.ScreenInfoCode;
            _DG.GName = model.GName;
            _DG.Code = Guid.NewGuid().ToString();
            _DG.UpdateTime = DateTime.Now;
            _DG.Type = model.IsSync ? 2 : 1;
            dbContext.DeviceGroup.Add(_DG);


            if (await dbContext.SaveChangesAsync() > 0)
            {
                //var count=dbContext.ProgramDevice.Where(i=>i.)
                ProgramDevice _PD = new ProgramDevice();
                _PD.Code = Guid.NewGuid().ToString();
                _PD.AddTime = DateTime.Now;
                _PD.DeviceGrounpCode = _DG.Code;
                _PD.ProgramGrounpCode = model.ProgramGroupCode;
                _PD.UpdateTime = DateTime.Now;
                dbContext.ProgramDevice.Add(_PD);
                //添加设备到设备组
                if (model.Devices.Count > 0)
                {
                    foreach (var item in model.Devices)
                    {
                        dbContext.DeviceToGroup.Add(new DeviceToGroup { AddTime = DateTime.Now, DeviceCode = item, GroupCode = _DG.Code, Code = Guid.NewGuid().ToString(), UpdateTime = DateTime.Now });
                    }

                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        _Result.Code = "200";
                        _Result.Msg = "添加成功";
                        _Result.Data = "";
                        //var _thisjobId = BackgroundJob.Schedule(() => SynData.SendDevInfo(), TimeSpan.FromSeconds(1));
                        //var ip = Method.GetUserIp(this.HttpContext);
                        //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "设备组模块", LogMsg = username + "添加了设备组：" + model.GName, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "创建", IP = ip });

                        //  dbContext.SaveChanges();
                    }
                    else
                    {
                        _Result.Code = "4";
                        _Result.Msg = "添加设备到设备组组失败";
                        _Result.Data = "";
                    }
                }
                else
                {
                    _Result.Code = "200";
                    _Result.Msg = "添加成功";
                    _Result.Data = "";
                    // var _thisjobId = BackgroundJob.Schedule(() => SynData.SendDevInfo(), TimeSpan.FromSeconds(1));
                }



            }
            else
            {
                _Result.Code = "5";
                _Result.Msg = "添加设备组失败";
                _Result.Data = "";
            }
            return Json(_Result);
        }
    }
}