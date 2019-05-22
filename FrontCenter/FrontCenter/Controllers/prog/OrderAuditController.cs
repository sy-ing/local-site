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

namespace FrontCenter.Controllers.prog
{
    public class OrderAuditController : Controller
    {
        /// <summary>
        /// 获取排期订单审核列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetScheduleAuditList(Input_OrderAuditGetList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //参数转换
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_OrderAuditGetList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


            //数据验证
            if (String.IsNullOrEmpty(model.UserCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入有效的用户编码";
                _Result.Data = "";
                return Json(_Result);
            }
            var _BeginDate = DateTime.MinValue;
            var _EndDate = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(model.BeginTime))
            {
                _BeginDate = Convert.ToDateTime(model.BeginTime);
            }

            if (!string.IsNullOrEmpty(model.EndTime))
            {
                _EndDate = Convert.ToDateTime(model.EndTime);
            }
            var mallUser = await dbContext.Account.Where(i => i.Code == model.UserCode && i.Activity).FirstOrDefaultAsync();
            if (mallUser == null)
            {
                _Result.Code = "510";
                _Result.Msg = "用户编码输入不正确";
                _Result.Data = "";
                return Json(_Result);
            }
            var curDate = DateTime.Now;
            var shops = await dbContext.MallShop.Where(i => i.MallCode == mallUser.MallCode).Join(dbContext.Shops, ms => ms.ShopCode, s => s.Code, (ms, s) => new {
                ms.ShopCode,
                s.HouseNum,
                s.Name
            }).ToListAsync();

            var auditList = await dbContext.OrderAudit.Where(i => i.OperUser == model.UserCode && ((i.AuditStatus == 0 && model.AuditStatus == 0) || (i.AuditStatus != 0 && model.AuditStatus != 0))).ToListAsync();
            var orderList = await dbContext.ScheduleOrder.Join(auditList, so => so.Code, al => al.OrderCode, (so, al) => new {
                so.PlacingNum,
                so.ScreenCode,
                so.ShopCode,
                so.Status,
                so.AddTime,
                so.UpdateTime,
                al.AuditOrder,
                al.AuditStatus,
                al.OperUser,
                al.OrderCode,
                AuditUpdateTime = al.UpdateTime,
                OrderAuditCode = al.Code,
            }).Where(i => i.AddTime > _BeginDate && i.AddTime < _EndDate && (model.AuditStatus == 0 || (model.AuditStatus > 0 && i.Status == model.AuditStatus) || (model.AuditStatus == -1 && i.Status > 0))).ToListAsync();

            var list = orderList.Join(shops, o => o.ShopCode, s => s.ShopCode, (o, s) => new {
                ShopName = s.Name,
                s.HouseNum,
                s.ShopCode,
                o.PlacingNum,
                o.Status,
                o.ScreenCode,
                o.AuditUpdateTime,
                o.AddTime,
                o.OrderCode,
                o.OrderAuditCode
            }).Where(i => (i.ShopName.Contains(model.SearchName) || i.HouseNum.Contains(model.SearchName) || i.PlacingNum.Contains(model.SearchName))).ToList();
            if (model.AuditStatus == 0)
            {
                list = list.OrderBy(i => i.AddTime).ToList();
            }
            else
            {

                var passOrderList = list.Where(i => i.Status == 2).OrderBy(i => i.AddTime).ToList();
                var ScheduleDateList = await dbContext.ScheduleDate.Join(passOrderList, sd => sd.ScheduleCode, s => s.OrderCode, (sd, s) => sd).ToListAsync();

                var auditOverList = passOrderList.Join(ScheduleDateList.Where(i => i.ScheduleDay.AddDays(1) > DateTime.Now), s => s.OrderCode, sc => sc.ScheduleCode, (s, sc) => s).Join(ScheduleDateList.Where(i => i.ScheduleDay <= DateTime.Now), s => s.OrderCode, sc => sc.ScheduleCode, (s, sc) => s).OrderBy(i => i.AddTime).ToList();
                auditOverList.AddRange(passOrderList.Except(auditOverList).OrderBy(i => i.AddTime).ToList());
                auditOverList.AddRange(list.Where(i => i.Status == 1).OrderBy(i => i.AddTime).ToList());
                auditOverList.AddRange(list.Where(i => i.Status > 2).OrderBy(i => i.Status).ThenBy(i => i.AddTime).ToList());
                list = auditOverList;
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
                ArrayList array = new ArrayList();
                foreach (var item in list)
                {
                    var DeviceCount = await dbContext.ScheduleDevice.Where(i => i.ScheduleCode == item.OrderCode).CountAsync();
                    var ScheduleList = await dbContext.ScheduleDate.Where(i => i.ScheduleCode == item.OrderCode).OrderBy(i => i.ScheduleDay).ToListAsync();
                    var BeginScheduleDate = "";
                    var EndScheduleDate = "";
                    var ScheduleStatusName = "--";
                    if (ScheduleList.Count > 0)
                    {
                        BeginScheduleDate = ScheduleList[0].ScheduleDay.ToString("yyyy-MM-dd");
                        EndScheduleDate = ScheduleList[ScheduleList.Count - 1].ScheduleDay.ToString("yyyy-MM-dd");
                        if (item.Status == 2)
                        {
                            ScheduleStatusName = ScheduleList[0].ScheduleDay > curDate ? "未开始" : ScheduleList[ScheduleList.Count - 1].ScheduleDay.AddDays(1) < curDate ? "已结束" : "排期内";
                        }
                    }
                    array.Add(new { item.Status, item.AddTime, item.HouseNum, item.OrderCode, item.PlacingNum, item.ScreenCode, item.ShopCode, item.ShopName, DeviceCount, BeginScheduleDate, EndScheduleDate, ScheduleStatusName, item.OrderAuditCode });
                }
                _Result.Data = array;
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
                ArrayList array = new ArrayList();
                foreach (var item in list)
                {
                    var DeviceCount = await dbContext.ScheduleDevice.Where(i => i.ScheduleCode == item.OrderCode).CountAsync();
                    var ScheduleList = await dbContext.ScheduleDate.Where(i => i.ScheduleCode == item.OrderCode).OrderBy(i => i.ScheduleDay).ToListAsync();
                    var BeginScheduleDate = "";
                    var EndScheduleDate = "";
                    var ScheduleStatusName = "--";
                    if (ScheduleList.Count > 0)
                    {
                        BeginScheduleDate = ScheduleList[0].ScheduleDay.ToString("yyyy-MM-dd");
                        EndScheduleDate = ScheduleList[ScheduleList.Count - 1].ScheduleDay.ToString("yyyy-MM-dd");
                        if (item.Status == 2)
                        {
                            ScheduleStatusName = ScheduleList[0].ScheduleDay > curDate ? "未开始" : ScheduleList[ScheduleList.Count - 1].ScheduleDay.AddDays(1) < curDate ? "已结束" : "排期内";
                        }
                    }
                    array.Add(new { item.Status, item.AddTime, item.HouseNum, item.OrderCode, item.PlacingNum, item.ScreenCode, item.ShopCode, item.ShopName, DeviceCount, BeginScheduleDate, EndScheduleDate, ScheduleStatusName, item.OrderAuditCode });
                }

                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { List = array, AllPage = allPage, AllCount = allCount };
            }




            return Json(_Result);
        }

        /// <summary>
        /// 获取排期订单审核列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetProgramAuditList(Input_OrderAuditGetList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //参数转换
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_OrderAuditGetList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


            //数据验证
            if (String.IsNullOrEmpty(model.UserCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入有效的用户编码";
                _Result.Data = "";
                return Json(_Result);
            }
            var _BeginDate = DateTime.MinValue;
            var _EndDate = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(model.BeginTime))
            {
                _BeginDate = Convert.ToDateTime(model.BeginTime);
            }

            if (!string.IsNullOrEmpty(model.EndTime))
            {
                _EndDate = Convert.ToDateTime(model.EndTime);
            }
            var mallUser = await dbContext.Account.Where(i => i.Code == model.UserCode && i.Activity).FirstOrDefaultAsync();
            if (mallUser == null)
            {
                _Result.Code = "510";
                _Result.Msg = "用户编码输入不正确";
                _Result.Data = "";
                return Json(_Result);
            }
            var curDate = DateTime.Now;
            var shops = await dbContext.MallShop.Where(i => i.MallCode == mallUser.MallCode).Join(dbContext.Shops, ms => ms.ShopCode, s => s.Code, (ms, s) => new {
                ms.ShopCode,
                s.HouseNum,
                s.Name
            }).ToListAsync();

            var auditList = await dbContext.OrderAudit.Where(i => i.OperUser == model.UserCode && ((i.AuditStatus == 0 && model.AuditStatus == 0) || (i.AuditStatus != 0 && model.AuditStatus != 0))).ToListAsync();
            var orderList = await dbContext.ProgramOrder.Join(auditList, so => so.Code, al => al.OrderCode, (so, al) => new {
                so.PlacingNum,
                so.ScreenCode,
                so.ShopCode,
                so.Status,
                so.AddTime,
                so.UpdateTime,
                al.AuditOrder,
                al.AuditStatus,
                al.OperUser,
                al.OrderCode,
                OrderAuditCode = al.Code,
                AuditUpdateTime = al.UpdateTime
            }).Where(i => i.AddTime > _BeginDate && i.AddTime < _EndDate && (model.AuditStatus == 0 || (model.AuditStatus > 0 && i.Status == model.AuditStatus) || (model.AuditStatus == -1 && i.Status > 0))).ToListAsync();

            var list = orderList.Join(shops, o => o.ShopCode, s => s.ShopCode, (o, s) => new {
                ShopName = s.Name,
                s.HouseNum,
                s.ShopCode,
                o.PlacingNum,
                o.Status,
                o.ScreenCode,
                o.AuditUpdateTime,
                o.AddTime,
                o.OrderCode,
                o.OrderAuditCode
            }).Where(i => (i.ShopName.Contains(model.SearchName) || i.HouseNum.Contains(model.SearchName) || i.PlacingNum.Contains(model.SearchName))).ToList();
            if (model.AuditStatus == 0)
            {
                list = list.OrderBy(i => i.AddTime).ToList();
            }
            else
            {
                var auditOverList = list.Where(i => i.Status == 2).OrderBy(i => i.AddTime).ToList();
                auditOverList.AddRange(list.Where(i => i.Status == 1).OrderBy(i => i.AddTime).ToList());
                auditOverList.AddRange(list.Where(i => i.Status > 2).OrderBy(i => i.Status).ThenBy(i => i.AddTime).ToList());
                list = auditOverList;
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
                ArrayList array = new ArrayList();
                foreach (var item in list)
                {
                    var MaterialCount = await dbContext.ProgramMaterial.Where(i => i.ProgramOrderCode == item.OrderCode).CountAsync();
                    array.Add(new { item.Status, item.AddTime, item.HouseNum, item.OrderCode, item.PlacingNum, item.ScreenCode, item.ShopCode, item.ShopName, MaterialCount, item.OrderAuditCode });
                }
                _Result.Data = array;
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
                ArrayList array = new ArrayList();
                foreach (var item in list)
                {
                    var MaterialCount = await dbContext.ProgramMaterial.Where(i => i.ProgramOrderCode == item.OrderCode).CountAsync();
                    array.Add(new { item.Status, item.AddTime, item.HouseNum, item.OrderCode, item.PlacingNum, item.ScreenCode, item.ShopCode, item.ShopName, MaterialCount, item.OrderAuditCode });
                }

                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { List = array, AllPage = allPage, AllCount = allCount };
            }
            return Json(_Result);
        }

        /// <summary>
        /// 获取排期订单审核列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> OrderAudit(Input_OrderAuditAdd model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //参数转换
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_OrderAuditAdd)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            if (string.IsNullOrEmpty(model.MallCode))
            {
                //检测用户登录情况
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
            if (string.IsNullOrEmpty(model.OrderAuditCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入订单审核编码";
                _Result.Data = "";
                return Json(_Result);
            }
            var orderAudit = await dbContext.OrderAudit.Where(i => i.Code == model.OrderAuditCode).FirstOrDefaultAsync();
            if (orderAudit == null)
            {
                _Result.Code = "510";
                _Result.Msg = "订单审核编码输入有误";
                _Result.Data = "";
                return Json(_Result);
            }

            if (string.IsNullOrEmpty(model.OrderCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入订单编码";
                _Result.Data = "";
                return Json(_Result);
            }
            if (model.AuditStatus == 2 && string.IsNullOrEmpty(model.AuditOpinion))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入拒绝理由";
                _Result.Data = "";
                return Json(_Result);
            }
            int resultData = 0;
            if (model.AuditStatus == 2)
            {
                orderAudit.AuditStatus = model.AuditStatus;
                orderAudit.AuditOpinion = model.AuditOpinion;
                orderAudit.UpdateTime = DateTime.Now;
                dbContext.OrderAudit.Update(orderAudit);
                if (model.AuditType == 1)
                {
                    var schedule = await dbContext.ScheduleOrder.Where(i => i.Code == model.OrderCode).FirstOrDefaultAsync();
                    schedule.Status = 3;
                    schedule.UpdateTime = DateTime.Now;
                    dbContext.ScheduleOrder.Update(schedule);
                    WeChatNotice.SendNoticeBySchedule(schedule, dbContext);
                }
                else
                {
                    var order = await dbContext.ProgramOrder.Where(i => i.Code == model.OrderCode).FirstOrDefaultAsync();
                    order.Status = 3;
                    order.UpdateTime = DateTime.Now;
                    dbContext.ProgramOrder.Update(order);
                    WeChatNotice.SendNoticeByProgram(order, dbContext);
                }


            }
            else
            {
                orderAudit.AuditStatus = model.AuditStatus;
                orderAudit.UpdateTime = DateTime.Now;
                dbContext.OrderAudit.Update(orderAudit);
                var nextAudit = await dbContext.AuditProcess.Where(i => i.ModuleType == model.AuditType && i.MallCode == model.MallCode && i.Order == (orderAudit.AuditOrder + 1)).FirstOrDefaultAsync();
                if (nextAudit == null)
                {
                    if (model.AuditType == 1)
                    {
                        var schedule = await dbContext.ScheduleOrder.Where(i => i.Code == model.OrderCode).FirstOrDefaultAsync();
                        schedule.Status = 2;
                        schedule.UpdateTime = DateTime.Now;
                        dbContext.ScheduleOrder.Update(schedule);
                        WeChatNotice.SendNoticeBySchedule(schedule, dbContext);
                    }
                    else
                    {
                        var order = await dbContext.ProgramOrder.Where(i => i.Code == model.OrderCode).FirstOrDefaultAsync();
                        order.Status = 2;
                        order.UpdateTime = DateTime.Now;
                        dbContext.ProgramOrder.Update(order);
                        WeChatNotice.SendNoticeByProgram(order, dbContext);
                    }
                    resultData = 1;
                }
                else
                {
                    var orderAuditNext = new OrderAudit();
                    orderAuditNext.AddTime = DateTime.Now;
                    orderAuditNext.AuditOpinion = "";
                    orderAuditNext.AuditOrder = orderAudit.AuditOrder + 1;
                    orderAuditNext.AuditStatus = 0;
                    orderAuditNext.Code = Guid.NewGuid().ToString();
                    orderAuditNext.OperUser = nextAudit.OperUser;
                    orderAuditNext.OrderCode = model.OrderCode;
                    orderAuditNext.UpdateTime = DateTime.Now;
                    dbContext.OrderAudit.Add(orderAuditNext);
                }
            }

            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "审核成功";
                _Result.Data = resultData;
            }
            return Json(_Result);
        }

        [HttpPost]
        public async Task<IActionResult> GetProgramOrderAudit(Input_OrderAuditGet model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            //参数转换
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_OrderAuditGet)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            if (string.IsNullOrEmpty(model.MallCode))
            {
                //检测用户登录情况
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

            //if (string.IsNullOrEmpty(model.OrderAuditCode))
            //{
            //    _Result.Code = "510";
            //    _Result.Msg = "请输入订单审核编码";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}
            var orderAudit = await dbContext.OrderAudit.Where(i => i.Code == model.OrderAuditCode).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(model.OrderAuditCode) && orderAudit == null)
            {
                _Result.Code = "510";
                _Result.Msg = "订单审核编码输入有误";
                _Result.Data = "";
                return Json(_Result);
            }
            if (string.IsNullOrEmpty(model.OrderCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入订单编码";
                _Result.Data = "";
                return Json(_Result);
            }
            var programOrder = await dbContext.ProgramOrder.Where(i => i.Code == model.OrderCode).Join(dbContext.Shops, p => p.ShopCode, s => s.Code, (p, s) => new {
                p.Code,
                p.AddTime,
                p.Info,
                p.PlacingNum,
                p.Status,
                p.UpdateTime,
                p.ScreenCode,
                s.Name,
                s.HouseNum
            }).Join(dbContext.ScreenInfo.Where(i => i.MallCode == model.MallCode), p => p.ScreenCode, s => s.Code, (p, s) => new {
                p.Code,
                p.AddTime,
                p.Info,
                p.PlacingNum,
                p.Status,
                p.UpdateTime,
                p.ScreenCode,
                p.Name,
                p.HouseNum,
                s.SName
            }).FirstOrDefaultAsync();
            if (programOrder == null)
            {
                _Result.Code = "510";
                _Result.Msg = "订单编码输入错误";
                _Result.Data = "";
                return Json(_Result);
            }
            var auditList = await dbContext.OrderAudit.Where(i => i.OrderCode == model.OrderCode).Join(dbContext.Account.Where(i => i.MallCode == model.MallCode && i.Activity), oa => oa.OperUser, mu => mu.Code, (oa, mu) => new {
                oa.AuditOpinion,
                oa.AuditOrder,
                oa.AuditStatus,
                oa.OperUser,
                UpdateTime = oa.UpdateTime.ToString("yyyy-MM-dd HH:mm"),
                mu.NickName,
                mu.AccountName
            }).OrderBy(i => i.AuditOrder).ToListAsync();

            var noAuditList = await dbContext.AuditProcess.Where(i => i.ModuleType == 2 && i.Order > auditList.Count).Join(dbContext.Account.Where(i => i.MallCode == model.MallCode && i.Activity), ap => ap.OperUser, mu => mu.Code, (ap, mu) => new {
                AuditOpinion = "",
                AuditOrder = ap.Order,
                AuditStatus = 0,
                ap.OperUser,
                UpdateTime = "",
                mu.NickName,
                mu.AccountName
            }).OrderBy(i => i.AuditOrder).ToListAsync();
            auditList.AddRange(noAuditList);

            var files = await dbContext.ProgramMaterial.Where(i => i.ProgramOrderCode == model.OrderCode).Join(dbContext.AssetFiles, p => p.FileCode, f => f.FileGUID, (p, f) => new {
                p.PreviewFileCode,
                p.ProgramOrderCode,
                p.ProgType,
                p.UpdateTime,
                p.ID,
                f.FileName,
                //FilePath = Method.ServerAddr+"MallSite/" + f.FilePath,
                FilePath = Method.OSSServer + f.FilePath,
                p.IsSuspicious,
                f.FileSize,
                f.Height,
                f.Width
            }).OrderBy(i => i.ID).ToListAsync();
            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = new { CurAuditOrder = orderAudit, TreeAuditOrder = auditList, ProgramOrder = programOrder, ProgramMaterial = files };
            return Json(_Result);

        }

        [HttpPost]
        public async Task<IActionResult> GetScheduleOrderAudit(Input_OrderAuditGet model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            //参数转换
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_OrderAuditGet)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            if (string.IsNullOrEmpty(model.MallCode))
            {
                //检测用户登录情况
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
            //if (string.IsNullOrEmpty(model.OrderAuditCode))
            //{
            //    _Result.Code = "510";
            //    _Result.Msg = "请输入订单审核编码";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}
            var orderAudit = await dbContext.OrderAudit.Where(i => i.Code == model.OrderAuditCode).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(model.OrderAuditCode) && orderAudit == null)
            {
                _Result.Code = "510";
                _Result.Msg = "订单审核编码输入有误";
                _Result.Data = "";
                return Json(_Result);
            }
            if (string.IsNullOrEmpty(model.OrderCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入订单编码";
                _Result.Data = "";
                return Json(_Result);
            }
            var scheduleOrder = await dbContext.ScheduleOrder.Where(i => i.Code == model.OrderCode).Join(dbContext.Shops, p => p.ShopCode, s => s.Code, (p, s) => new
            {
                p.Code,
                p.AddTime,
                p.Info,
                p.PlacingNum,
                p.Status,
                p.UpdateTime,
                p.ScreenCode,
                s.Name,
                s.HouseNum
            }).Join(dbContext.ScreenInfo.Where(i => i.MallCode == model.MallCode), p => p.ScreenCode, s => s.Code, (p, s) => new
            {
                p.Code,
                p.AddTime,
                p.Info,
                p.PlacingNum,
                p.Status,
                p.UpdateTime,
                p.ScreenCode,
                p.Name,
                p.HouseNum,
                s.SName
            }).FirstOrDefaultAsync();
            if (scheduleOrder == null)
            {
                _Result.Code = "510";
                _Result.Msg = "订单编码输入错误";
                _Result.Data = "";
                return Json(_Result);
            }
            var auditList = await dbContext.OrderAudit.Where(i => i.OrderCode == model.OrderCode).Join(dbContext.Account.Where(i => i.MallCode == model.MallCode && i.Activity), oa => oa.OperUser, mu => mu.Code, (oa, mu) => new
            {
                oa.AuditOpinion,
                oa.AuditOrder,
                oa.AuditStatus,
                oa.OperUser,
                UpdateTime = oa.UpdateTime.ToString("yyyy-MM-dd HH:mm"),
                mu.NickName,
                mu.AccountName
            }).OrderBy(i => i.AuditOrder).ToListAsync();

            var noAuditList = await dbContext.AuditProcess.Where(i => i.ModuleType == 1 && i.Order > auditList.Count).Join(dbContext.Account.Where(i => i.MallCode == model.MallCode && i.Activity), ap => ap.OperUser, mu => mu.Code, (ap, mu) => new
            {
                AuditOpinion = "",
                AuditOrder = ap.Order,
                AuditStatus = 0,
                ap.OperUser,
                UpdateTime = "",
                mu.NickName,
                mu.AccountName
            }).OrderBy(i => i.AuditOrder).ToListAsync();
            auditList.AddRange(noAuditList);

            var scheduleDateList = await dbContext.ScheduleDate.Where(i => i.ScheduleCode == model.OrderCode).OrderBy(i => i.ScheduleDay).ToListAsync();
            var BeginScheduleDate = scheduleDateList[0].ScheduleDay.ToString("yyyy-MM-dd");
            var EndScheduleDate = scheduleDateList[scheduleDateList.Count - 1].ScheduleDay.ToString("yyyy-MM-dd");
            var isPass = DateTime.Now.Date > scheduleDateList[scheduleDateList.Count - 1].ScheduleDay ? 1 : 0;
            var schedulePeriodList = await dbContext.SchedulePeriod.Where(i => i.ScheduleCode == model.OrderCode).Join(dbContext.TimeSlot, sp => sp.TimeSlotCode, ts => ts.Code, (sp, ts) => new
            {
                sp.Code,
                ts.BeginTimeSlot,
                ts.EndTimeSlot
            }).ToListAsync();
            var scheduleDevice = await dbContext.ScheduleDevice.Where(i => i.ScheduleCode == model.OrderCode).Join(dbContext.Device, s => s.DeviceCode, d => d.Code, (s, d) => new
            {
                d.Building,
                d.DevNum,
                d.Floor,
                d.IP,
                d.MAC,
                d.Mark,
                d.Position,
                d.Version
            }).Join(dbContext.Building, d => d.Building, b => b.Code, (d, b) => new
            {
                d.Building,
                d.DevNum,
                d.Floor,
                d.IP,
                d.MAC,
                d.Mark,
                d.Position,
                d.Version,
                BuildName = b.Name
            }).Join(dbContext.Floor, d => d.Floor, f => f.Code, (d, f) => new
            {
                d.Building,
                d.DevNum,
                d.Floor,
                d.IP,
                d.MAC,
                d.Mark,
                d.Position,
                d.Version,
                d.BuildName,
                FloorName = f.Name
            }).OrderBy(i => i.Floor).ThenBy(i => i.IP).ToListAsync();
            _Result.Code = "200";
            _Result.Msg = "获取成功";
            //_Result.Data = new { CurAuditOrder = orderAudit, TreeAuditOrder = auditList, ScheduleOrder = scheduleOrder, BeginScheduleDate, EndScheduleDate, SchedulePeriodList = schedulePeriodList,IsPass=isPass };
            _Result.Data = new { CurAuditOrder = orderAudit, TreeAuditOrder = auditList, ScheduleOrder = scheduleOrder, BeginScheduleDate, EndScheduleDate, SchedulePeriodList = schedulePeriodList, ScheduleDevice = scheduleDevice, IsPass = isPass };
            return Json(_Result);

        }
    }
}