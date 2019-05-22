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

namespace FrontCenter.Controllers.prog
{
    public class SubtitleController : Controller
    {
        /// <summary>
        /// 设置字幕
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Set(Input_SetSubtitle model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (Input_SetSubtitle)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
                var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
                if (string.IsNullOrEmpty(uol.UserName))
                {
                    _Result.Code = "401";
                    _Result.Msg = "请登陆后再进行操作";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var mall = await dbContext.Mall.Where(i => i.Code == uol.MallCode).FirstOrDefaultAsync();

                if (mall == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的商场编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                if (string.IsNullOrEmpty(model.Location))
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入字幕位置";
                    _Result.Data = "";
                    return Json(_Result);
                }

                if (string.IsNullOrEmpty(model.Name))
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入字幕名称";
                    _Result.Data = "";
                    return Json(_Result);
                }

                switch (model.Type.ToLower())
                {
                    case "regular":
                        if (model.BeginTime > model.EndTime)
                        {
                            _Result.Code = "510";
                            _Result.Msg = "开始时间不能大于结束时间";
                            _Result.Data = "";
                            return Json(_Result);
                        }
                        //修正JS传回的标准时  
                        model.BeginTime = model.BeginTime.AddHours(8);
                        model.EndTime = model.EndTime.AddHours(8);

                        model.Duration = 0;
                        break;
                    case "immediate":

                        if (model.Duration <= 0)
                        {
                            _Result.Code = "510";
                            _Result.Msg = "时长需要大于0";
                            _Result.Data = "";
                            return Json(_Result);
                        }
                        model.BeginTime = DateTime.MinValue;
                        model.EndTime = DateTime.MaxValue;
                        break;
                    default:
                        _Result.Code = "510";
                        _Result.Msg = "请输入显示模式";
                        _Result.Data = "";
                        return Json(_Result);
                }



                var _subtitle = await dbContext.Subtitle.Where(i => !i.IsDel && i.MallCode == uol.MallCode && i.Name == model.Name).AsNoTracking().FirstOrDefaultAsync();

                if (_subtitle != null)
                {
                    if (_subtitle.Code != model.Code || string.IsNullOrEmpty(model.Code))
                    {
                        _Result.Code = "510";
                        _Result.Msg = "字幕名称重复";
                        _Result.Data = "";
                        return Json(_Result);
                    }
                }


                if (!string.IsNullOrEmpty(model.Code))
                {
                    var subtitle = await dbContext.Subtitle.Where(i => !i.IsDel && i.Code == model.Code).FirstOrDefaultAsync();
                    if (subtitle == null)
                    {
                        _Result.Code = "510";
                        _Result.Msg = "无效的字幕编码：" + model.Code;
                        _Result.Data = "";
                        return Json(_Result);
                    }
                    else
                    {
                        subtitle.BeginTime = model.BeginTime;
                        subtitle.EndTime = model.EndTime;
                        subtitle.Text = model.Text;
                        subtitle.Location = model.Location;
                        subtitle.UpdateTime = DateTime.Now;
                        subtitle.Type = model.Type;
                        subtitle.Name = model.Name;
                        subtitle.Duration = model.Duration;
                        dbContext.Subtitle.Update(subtitle);
                    }
                }
                else
                {
                    var subtitle = new Subtitle()
                    {
                        AddTime = DateTime.Now,
                        MallCode = uol.MallCode,
                        BeginTime = model.BeginTime,
                        Code = Guid.NewGuid().ToString(),
                        Duration = model.Duration,
                        Type = model.Type,
                        EndTime = model.EndTime,
                        IsDel = false,
                        Location = model.Location,
                        Text = model.Text,
                        UpdateTime = DateTime.Now,
                        Name = model.Name,

                    };
                    dbContext.Subtitle.Add(subtitle);
                }





                if (await dbContext.SaveChangesAsync() > 0)
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
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "代码错误";
                _Result.Data = e.ToString();
                throw;
            }



            return Json(_Result);
        }


        /// <summary>
        /// 删除字幕
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Del(Input_DelSubtitle model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (Input_DelSubtitle)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



                if (model.Code.Count() <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入字幕编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                foreach (var c in model.Code)
                {
                    var subtitle = await dbContext.Subtitle.Where(i => !i.IsDel && i.Code == c).FirstOrDefaultAsync();

                    if (subtitle != null)
                    {
                        subtitle.IsDel = true;
                        subtitle.UpdateTime = DateTime.Now;
                        dbContext.Subtitle.Update(subtitle);
                        var std = await dbContext.SubtitleToDeviceGroup.Where(i => !i.IsDel && i.SubtitleCode == c).ToListAsync();
                        foreach (var item in std)
                        {
                            item.IsDel = true;
                            item.UpdateTime = DateTime.Now;
                            dbContext.SubtitleToDeviceGroup.Update(item);
                        }
                    }
                    else
                    {
                        _Result.Code = "510";
                        _Result.Msg = "无效的字幕编码：" + c;
                        _Result.Data = "";
                        return Json(_Result);
                    }
                }

                if (await dbContext.SaveChangesAsync() > 0)
                {
                    _Result.Code = "200";
                    _Result.Msg = "删除成功";
                    _Result.Data = "";

                }
                else
                {
                    _Result.Code = "400";
                    _Result.Msg = "删除失败";
                    _Result.Data = "";


                }






            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "代码错误";
                _Result.Data = e.ToString();
                throw;
            }



            return Json(_Result);
        }


        /// <summary>
        /// 获取字幕信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Get(Input_GetSubtitle model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (Input_GetSubtitle)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



                if (string.IsNullOrEmpty(model.Code))
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入字幕编码";
                    _Result.Data = "";
                    return Json(_Result);
                }




                var subtitle = await dbContext.Subtitle.Where(i => !i.IsDel && i.Code == model.Code).Select(s => new {
                    s.Code,
                    BeginTime = s.BeginTime == DateTime.MinValue ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : s.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    s.Type,
                    s.Location,
                    s.Text,
                    EndTime = s.EndTime == DateTime.MaxValue ? DateTime.Now.AddDays(1).AddMilliseconds(-1).ToString("yyyy-MM-dd HH:mm:ss") : s.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    s.Duration,
                    s.Name

                }).FirstOrDefaultAsync();

                if (subtitle != null)
                {
                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = subtitle;
                }
                else
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的字幕编码：" + model.Code;
                    _Result.Data = "";
                    return Json(_Result);
                }


            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "代码错误";
                _Result.Data = e.ToString();
                throw;
            }



            return Json(_Result);
        }

        /// <summary>
        /// 获取字幕列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetList(Input_GetSubtitleList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (Input_GetSubtitleList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
                var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
                if (string.IsNullOrEmpty(uol.UserName))
                {
                    _Result.Code = "401";
                    _Result.Msg = "请登陆后再进行操作";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var mall = await dbContext.Mall.Where(i => i.Code == uol.MallCode).FirstOrDefaultAsync();

                if (mall == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的商场编码";
                    _Result.Data = "";
                    return Json(_Result);
                }
                var list = await dbContext.Subtitle.Where(i => !i.IsDel && i.MallCode == uol.MallCode && (string.IsNullOrEmpty(model.Name) || i.Name.Contains(model.Name))).Select(s => new {
                    s.Code,
                    BeginTime = s.BeginTime == DateTime.MinValue ? "" : s.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    s.Type,
                    s.Location,
                    s.Text,
                    s.Name,
                    EndTime = s.EndTime == DateTime.MinValue ? "" : s.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    s.Duration,
                    AddTime = s.AddTime.ToString("yyyy-MM-dd"),
                    State = s.Type.ToLower() == "regular" ?
                    (DateTime.Now < s.BeginTime ? "未开始" : (DateTime.Now > s.EndTime ? "已结束" : "播放中")) :
                    (DateTime.Now > s.UpdateTime.AddMinutes(s.Duration) ? "已结束" : "播放中")

                }).AsNoTracking().ToListAsync();


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



                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = new { List = list, AllPage = allPage, AllCount = allCount };

                }



            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "代码错误";
                _Result.Data = e.ToString();
                throw;
            }



            return Json(_Result);
        }


        /// <summary>
        /// 获取字幕列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetListByGroupCode(Input_GetSubtitleListByGroupCode model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (Input_GetSubtitleListByGroupCode)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());




                if (string.IsNullOrEmpty(model.GroupCode))
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入设备组编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var group = await dbContext.DeviceGroup.Where(i => i.Code == model.GroupCode).FirstOrDefaultAsync();
                if (group == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的设备组编码：" + model.GroupCode;
                    _Result.Data = "";
                    return Json(_Result);
                }
                var list = await dbContext.SubtitleToDeviceGroup.Where(i => !i.IsDel && i.GroupCode == model.GroupCode).Join(
                    dbContext.Subtitle.Where(i => !i.IsDel), std => std.SubtitleCode, s => s.Code, (std, s) => new
                    {
                        LinkCode = std.Code,
                        s.Code,
                        BeginTime = s.BeginTime == DateTime.MinValue ? "" : s.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        s.Type,
                        s.Location,
                        s.Text,
                        s.Name,
                        EndTime = s.EndTime == DateTime.MinValue ? "" : s.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        s.Duration,
                        AddTime = s.AddTime.ToString("yyyy-MM-dd")
                    }
                    ).ToListAsync();



                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = list;



            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "代码错误";
                _Result.Data = e.ToString();
                throw;
            }



            return Json(_Result);
        }




        /// <summary>
        /// 设置字幕
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Publish(Input_SubtitlePublish model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
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

                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (Input_SubtitlePublish)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

                if (string.IsNullOrEmpty(model.Code))
                {
                    _Result.Code = "510";
                    _Result.Msg = "字幕编码不可为空";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var subtitle = await dbContext.Subtitle.Where(i => !i.IsDel && i.Code == model.Code).FirstOrDefaultAsync();
                if (subtitle == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的字幕编码：" + model.Code;
                    _Result.Data = "";
                    return Json(_Result);
                }



                //获取设备组 到设备关系列表
                var deviceGroups = await dbContext.SubtitleToDeviceGroup.Where(i => !i.IsDel && i.SubtitleCode == model.Code).ToListAsync();


                List<SubtitleToDeviceGroup> _OldDTG = new List<SubtitleToDeviceGroup>();//待移除的设备
                List<string> olddevice = new List<string>();//数据库中已包含的设备
                List<SubtitleToDeviceGroup> _NewDTG = new List<SubtitleToDeviceGroup>();//待添加的设备
                var dgname = string.Empty;
                //列表中不包含 移除
                foreach (var gd in deviceGroups)
                {
                    //dgname += gd.GName + ",";
                    var devg = await dbContext.DeviceGroup.Where(i => i.Code == gd.GroupCode).FirstOrDefaultAsync();
                    dgname += devg.GName + ",";
                    if (!model.GroupCode.Contains(gd.GroupCode))
                    {
                        _OldDTG.Add(gd);
                    }
                    else
                    {
                        olddevice.Add(gd.GroupCode);
                    }
                }

                //列表中不存在 添加
                foreach (var device in model.GroupCode)
                {

                    if (!olddevice.Contains(device))
                    {
                        _NewDTG.Add(new SubtitleToDeviceGroup { AddTime = DateTime.Now, GroupCode = device, SubtitleCode = model.Code, IsDel = false, Code = Guid.NewGuid().ToString(), UpdateTime = DateTime.Now });
                    }
                }


                dbContext.SubtitleToDeviceGroup.RemoveRange(_OldDTG);
                dbContext.SubtitleToDeviceGroup.AddRange(_NewDTG);
                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "发布模块", LogMsg = username + "发布字幕：" + subtitle.Name + "到设备组：" + dgname.TrimEnd(',') + "中。", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });

                await dbContext.SaveChangesAsync();
                _Result.Code = "200";
                _Result.Msg = "发布成功";
                _Result.Data = "";

                if (await dbContext.SaveChangesAsync() > 0)
                {
                    _Result.Code = "200";
                    _Result.Msg = "发布成功";
                    _Result.Data = "";
                }

            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "代码错误";
                _Result.Data = e.ToString();
                throw;
            }



            return Json(_Result);
        }

        /// <summary>
        /// 设置字幕
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CancelPublish(Input_SubtitlePublishCancel model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (Input_SubtitlePublishCancel)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

                if (model.Code.Count() <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "编码不可为空";
                    _Result.Data = "";
                    return Json(_Result);
                }
                foreach (var c in model.Code)
                {
                    var std = await dbContext.SubtitleToDeviceGroup.Where(i => !i.IsDel && i.Code == c).FirstOrDefaultAsync();
                    if (std != null)
                    {
                        std.IsDel = true;
                        std.UpdateTime = DateTime.Now;
                        dbContext.SubtitleToDeviceGroup.Update(std);
                    }
                    else
                    {
                        _Result.Code = "510";
                        _Result.Msg = "无效的关联编码：" + c;
                        _Result.Data = "";
                        return Json(_Result);
                    }

                }


                if (await dbContext.SaveChangesAsync() > 0)
                {
                    _Result.Code = "200";
                    _Result.Msg = "取消成功";
                    _Result.Data = "";
                }

            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "代码错误";
                _Result.Data = e.ToString();
                throw;
            }



            return Json(_Result);
        }




        /// <summary>
        /// 获取设备字幕
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDevSubtitle(Input_GetDevSubtitle model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (Input_GetDevSubtitle)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



                if (string.IsNullOrEmpty(model.Code))
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入设备IP";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var dev = await dbContext.Device.Where(i => !i.IsDelete && i.Code == model.Code).FirstOrDefaultAsync();


                var subtitle = await dbContext.DeviceToGroup.Where(i => i.DeviceCode == dev.Code).Join(
                    dbContext.SubtitleToDeviceGroup.Where(i => !i.IsDel),
                    dtg => dtg.GroupCode,
                    std => std.GroupCode, (dtg, std) => std).Join(
                    dbContext.Subtitle.Where(i => !i.IsDel
                    && (
                    (i.BeginTime <= DateTime.Now && i.EndTime >= DateTime.Now && i.Type.ToLower() == "regular")
                    ||
                    (i.Type.ToLower() == "immediate" && DateTime.Now <= i.AddTime.AddMinutes(i.Duration))
                    )),
                    std => std.SubtitleCode,
                    s => s.Code,
                    (std, s) => new {
                        s.Code,
                        BeginTime = s.BeginTime == DateTime.MinValue ? "" : s.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        s.Type,
                        s.Location,
                        s.Text,
                        s.Name,
                        EndTime = s.EndTime == DateTime.MinValue ? "" : s.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        s.Duration,
                        AddTime = s.AddTime.ToString("yyyy-MM-dd HH:mm:ss")

                    }

                    ).ToListAsync();


                if (subtitle != null)
                {
                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = new { List = subtitle };
                }
                else
                {
                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = "";
                }


            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "代码错误";
                _Result.Data = e.ToString();
                throw;
            }



            return Json(_Result);
        }


        /// <summary>
        /// 获取字幕所在设备组信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetGroupList(Input_GetSubtitleGroupList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (Input_GetSubtitleGroupList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



                var subtitle = await dbContext.Subtitle.Where(i => !i.IsDel && i.Code == model.Code).Select(s => new {
                    s.Code,
                    s.Duration,
                    s.Location,
                    s.Name,
                    s.Text,
                    s.Type,
                    s.MallCode

                }).FirstOrDefaultAsync();
                if (subtitle == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "字幕编码输入错误";
                    _Result.Data = null;
                    return Json(_Result);
                }
                List<Output_DeviceGroup> list = new List<Output_DeviceGroup>();
                var deviceGroupAllList = await dbContext.DeviceGroup.Where(i => i.MallCode == subtitle.MallCode).ToListAsync();
                foreach (var item in deviceGroupAllList)
                {
                    var deviceCount = await dbContext.DeviceToGroup.Where(i => i.GroupCode == item.Code).CountAsync();
                    Output_DeviceGroup _DG = new Output_DeviceGroup();
                    _DG.Code = item.Code;
                    _DG.DeviceCount = deviceCount;
                    _DG.GName = item.GName;
                    _DG.Type = item.Type;
                    list.Add(_DG);
                }
                var deviceGroup = list.Join(dbContext.SubtitleToDeviceGroup.Where(i => !i.IsDel && i.SubtitleCode == model.Code), dg => dg.Code, s => s.GroupCode, (dg, s) => dg).ToList();
                var undDeviceGroup = list.Except(deviceGroup).ToList();
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { UndDeviceGroup = undDeviceGroup, DeviceGroup = deviceGroup, Subtitle = subtitle };




            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "代码错误";
                _Result.Data = e.ToString();
            }



            return Json(_Result);
        }




    }
}