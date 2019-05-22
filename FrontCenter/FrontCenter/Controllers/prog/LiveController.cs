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
    public class LiveController : Controller
    {
        /// <summary>
        /// 添加直播
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(Input_LiveAdd model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_LiveAdd)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
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


            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Url) || string.IsNullOrEmpty(model.ScreenCode))
            {
                _Result.Code = "510";
                _Result.Msg = "参数不完整";
                _Result.Data = "";
                return Json(_Result);
            }




            var scr = dbContext.ScreenInfo.Where(i => i.Code == model.ScreenCode).FirstOrDefault();


            if (scr == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的分辨率类型编码";
                _Result.Data = "";
                return Json(_Result);
            }



            Live live = new Live
            {
                AddTime = DateTime.Now,
                Code = Guid.NewGuid().ToString(),
                UpdateTime = DateTime.Now,
                MallCode = uol.MallCode,
                Name = model.Name,
                Url = model.Url,
                ScreenCode = model.ScreenCode,
                IsDel = false,
                BeingUsed = false

            };
            dbContext.Live.Add(live);





            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "添加成功";
                _Result.Data = "";
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
        /// 删除直播
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Del(Input_LiveDel model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_LiveDel)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            if (model.Code.Count() <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入编码";
                _Result.Data = "";
                return Json(_Result);
            }
            foreach (var c in model.Code)
            {
                var live = dbContext.Live.Where(i => i.Code == c).FirstOrDefault();
                if (live == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的编码:" + c;
                    _Result.Data = "";
                    return Json(_Result);
                }
                if (live.BeingUsed)
                {
                    var ldlist = await dbContext.LiveToDev.Where(i => !i.IsDel && i.LiveCode == c).ToListAsync();

                    foreach (var ld in ldlist)
                    {
                        ld.IsDel = true;
                        ld.UpdateTime = DateTime.Now;
                        dbContext.LiveToDev.Update(ld);

                        //to do
                        //发送删除通知到前端  取消删除任务的关联性
                        var dev = await dbContext.Device.Where(i => i.Code == ld.DevCode).FirstOrDefaultAsync();
                        MsgTemplate msg = new MsgTemplate();
                        msg.SenderID = Method.ServerAddr;
                        msg.ReceiverID = dev.Code;
                        msg.MessageType = "json";
                        msg.Content = new { Type = "Live", Data = new { Command = "cancel", Code = ld.LiveCode, live.Url } };
                        await Method.SendMsgAsync(msg);
                    }
                }
                else
                {
                    var ldlist = await dbContext.LiveToDev.Where(i => !i.IsDel && i.LiveCode == c).ToListAsync();

                    foreach (var ld in ldlist)
                    {
                        ld.IsDel = true;
                        ld.UpdateTime = DateTime.Now;
                        dbContext.LiveToDev.Update(ld);
                    }
                }
                live.BeingUsed = false;
                live.IsDel = true;
                live.UpdateTime = DateTime.Now;
                dbContext.Live.Update(live);





            }



            if (await dbContext.SaveChangesAsync() > 0)
            {

                _Result.Code = "200";
                _Result.Msg = "删除成功";
                _Result.Data = "";
            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "删除失败";
                _Result.Data = "";
            }

            return Json(_Result);
        }

        /// <summary>
        /// 更新直播
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Input_LiveEdit model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_LiveEdit)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


            if (string.IsNullOrEmpty(model.Code))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入直播编码";
                _Result.Data = "";
                return Json(_Result);
            }


            if (string.IsNullOrEmpty(model.Url))
            {
                _Result.Code = "510";
                _Result.Msg = "直播路径不可为空";
                _Result.Data = "";
                return Json(_Result);
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                _Result.Code = "510";
                _Result.Msg = "直播名称不可为空";
                _Result.Data = "";
                return Json(_Result);
            }
            var live = dbContext.Live.Where(i => i.Code == model.Code).FirstOrDefault();

            if (live == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的直播编码";
                _Result.Data = "";
                return Json(_Result);
            }

            if (live.Url != model.Url && live.BeingUsed)
            {

                var ldlist = await dbContext.LiveToDev.Where(i => !i.IsDel && i.LiveCode == model.Code).ToListAsync();

                foreach (var ld in ldlist)
                {
                    ld.IsDel = true;
                    ld.UpdateTime = DateTime.Now;
                    dbContext.LiveToDev.Update(ld);


                    //发送删除通知到前端  取消删除任务的关联性
                    var dev = await dbContext.Device.Where(i => i.Code == ld.DevCode).FirstOrDefaultAsync();
                    MsgTemplate msg = new MsgTemplate();
                    msg.SenderID = Method.ServerAddr;
                    msg.ReceiverID = dev.Code;
                    msg.MessageType = "json";
                    msg.Content = new { Type = "Live", Data = new { Command = "update", Code = ld.LiveCode, live.Url } };
                    await Method.SendMsgAsync(msg);
                }

            }





            live.Name = model.Name;
            live.Url = model.Url;
            live.UpdateTime = DateTime.Now;
            dbContext.Live.Update(live);



            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "更新成功";
                _Result.Data = "";
            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "更新失败";
                _Result.Data = "";
            }

            return Json(_Result);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetList(Input_GetLiveList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetLiveList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
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

                var mall = await dbContext.Mall.Where(i => i.Code == uol.MallCode).FirstOrDefaultAsync();

                if (mall == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的商场编码";
                    _Result.Data = "";
                    return Json(_Result);
                }


                var list = await dbContext.Live.Where(i => !i.IsDel && i.MallCode == uol.MallCode && (string.IsNullOrEmpty(model.Name) || i.Name.Contains(model.Name))).Join(dbContext.ScreenInfo.Where(i => (i.Code == model.ScreenCode || string.IsNullOrEmpty(model.ScreenCode))), l => l.ScreenCode, scr => scr.Code, (l, scr) => new
                {
                    l.Name,
                    l.Code,
                    l.BeingUsed,
                    l.ScreenCode,
                    l.Url,
                    Screen = scr.SName
                }).GroupJoin(dbContext.LiveToDev.Where(i => !i.IsDel), l => l.Code, ld => ld.LiveCode, (l, ld) => new
                {
                    l.Name,
                    l.Code,
                    l.BeingUsed,
                    l.ScreenCode,
                    l.Screen,
                    l.Url,
                    DevCode = string.IsNullOrEmpty(ld.FirstOrDefault().DevCode) ? "" : ld.FirstOrDefault().DevCode
                }).GroupBy(g => new
                {
                    g.Name,
                    g.Code,
                    g.BeingUsed,
                    g.ScreenCode,
                    g.Screen,
                    g.Url


                }).Select(
                    s => new {
                        DevCount = s.Count(i => !string.IsNullOrEmpty(i.DevCode)),
                        s.Key.Name,
                        s.Key.Code,
                        s.Key.BeingUsed,
                        s.Key.ScreenCode,
                        s.Key.Screen,
                        s.Key.Url
                    }
                    ).AsNoTracking().ToListAsync();

                //排序
                if (model.Order.ToLower() == "desc")
                {
                    list = list.OrderByDescending(o => o.DevCount).ToList();
                }
                if (model.Order.ToLower() == "asc")
                {
                    list = list.OrderBy(o => o.DevCount).ToList();
                }

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
                _Result.Msg = e.ToString();
                _Result.Data = "";
                //throw;
            }




            return Json(_Result);
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetInfo(Input_GetLiveInfo model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetLiveInfo)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


            if (string.IsNullOrEmpty(model.Code))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入编码";
                _Result.Data = "";
                return Json(_Result);
            }




            var live = await dbContext.Live.Where(i => !i.IsDel && i.Code == model.Code).Join(dbContext.ScreenInfo,

                l => l.ScreenCode, scr => scr.Code, (l, scr) => new {
                    l.AddTime,
                    l.BeingUsed,
                    l.Code,
                    l.ID,
                    l.IsDel,
                    l.Name,
                    l.ScreenCode,
                    l.UpdateTime,
                    l.Url,
                    Screen = scr.SName

                }).AsNoTracking().FirstOrDefaultAsync();

            if (live == null)
            {

                _Result.Code = "510";
                _Result.Msg = "无效的直播编码";
                _Result.Data = "";

            }
            else
            {
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = live;
            }






            return Json(_Result);
        }


        /// <summary>
        /// 直播控制
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LiveCtr(Input_LiveCtr model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_LiveCtr)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            try
            {
                if (string.IsNullOrEmpty(model.Code))
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var live = await dbContext.Live.Where(i => !i.IsDel && i.Code == model.Code).AsNoTracking().FirstOrDefaultAsync();

                if (live == null)
                {

                    _Result.Code = "510";
                    _Result.Msg = "无效的直播编码";
                    _Result.Data = "";
                    return Json(_Result);
                }
                var ldlist = await dbContext.LiveToDev.Where(i => !i.IsDel).ToListAsync();
                switch (model.Command.ToLower())
                {
                    case "start":
                        live.BeingUsed = true;
                        live.UpdateTime = DateTime.Now;
                        dbContext.Live.Update(live);
                        foreach (var ld in ldlist)
                        {
                            //发送命令到机器
                            var device = await dbContext.Device.Where(i => i.Code == ld.DevCode).FirstOrDefaultAsync();
                            if (device != null)
                            {
                                MsgTemplate msg = new MsgTemplate();
                                msg.SenderID = Method.ServerAddr;
                                msg.ReceiverID = device.Code;
                                msg.MessageType = "json";
                                msg.Content = new { Type = "Live", Data = new { Command = "start", Code = model.Code, live.Url } };
                                await Method.SendMsgAsync(msg);
                            }

                        }
                        if (await dbContext.SaveChangesAsync() > 0)
                        {
                            _Result.Code = "200";
                            _Result.Msg = "设置成功";
                            _Result.Data = "";
                        }
                        break;
                    case "cancel":
                        live.BeingUsed = false;
                        live.UpdateTime = DateTime.Now;
                        dbContext.Live.Update(live);
                        foreach (var ld in ldlist)
                        {
                            //发送命令到机器
                            var device = await dbContext.Device.Where(i => i.Code == ld.DevCode).FirstOrDefaultAsync();
                            if (device != null)
                            {
                                MsgTemplate msg = new MsgTemplate();
                                msg.SenderID = Method.ServerAddr;
                                msg.ReceiverID = device.Code;
                                msg.MessageType = "json";
                                msg.Content = new { Type = "Live", Data = new { Command = "cancel", Code = model.Code, live.Url } };
                                await Method.SendMsgAsync(msg);
                            }

                        }
                        if (await dbContext.SaveChangesAsync() > 0)
                        {
                            _Result.Code = "200";
                            _Result.Msg = "设置成功";
                            _Result.Data = "";
                        }
                        break;
                    default:
                        _Result.Code = "510";
                        _Result.Msg = "无效的命令：" + model.Command;
                        _Result.Data = "";
                        break;
                }
            }
            catch (Exception e)
            {

                _Result.Code = "500";
                _Result.Msg = e.ToString();
                _Result.Data = "";
                throw;
            }








            return Json(_Result);
        }
    }
}