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
    public class TimeAxisController : Controller
    {

        /// <summary>
        /// 获取时间轴--时间段数据
        /// </summary>
        /// <param name="mallCode">商场编码</param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetInfo(string mallCode, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            if (string.IsNullOrEmpty(mallCode))
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
                    mallCode = uol.MallCode;
                }
            }
            if (String.IsNullOrEmpty(mallCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入店铺编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var timeAxis = await dbContext.TimeAxis.Where(i => i.MallCode == mallCode).FirstOrDefaultAsync();
            if (timeAxis != null)
            {
                var timeRelate = await dbContext.TimeRelate.Where(i => i.TimeAxisCode == timeAxis.Code).Join(dbContext.TimeSlot, tr => tr.TimeSlotCode, ts => ts.Code, (tr, ts) => new
                {
                    tr.TimeAxisCode,
                    TimeRelateCode = tr.Code,
                    ts.BeginTimeSlot,
                    ts.EndTimeSlot,
                    tr.OpenTime,
                    tr.TimeSlotCode,
                    ts.ID
                }).OrderBy(i => i.BeginTimeSlot).ToListAsync();
                _Result.Data = new { TimeAxis = timeAxis, TimeRelateList = timeRelate };
            }
            else
            {
                _Result.Data = null;
            }

            _Result.Code = "200";
            _Result.Msg = "获取成功";
            return Json(_Result);
        }

        [HttpPost]
        public async Task<IActionResult> SetTimeAxis(Input_TimeAxis model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            //检测用户登录情况
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_TimeAxis)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

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

            if (model.BeginAxis == null)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入时间轴开始时间";
                _Result.Data = "";
                return Json(_Result);
            }

            if (model.EndAxis == null)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入时间轴结束时间";
                _Result.Data = "";
                return Json(_Result);
            }

            if (model.EndAxis <= model.BeginAxis)
            {
                _Result.Code = "510";
                _Result.Msg = "结束时间必须大于开始时间";
                _Result.Data = "";
                return Json(_Result);
            }
            var TimeAxis = await dbContext.TimeAxis.Where(i => i.Code == model.TimeAxisCode).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(model.TimeAxisCode))
            {
                if (TimeAxis == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "时间轴编码输入有误";
                    _Result.Data = "";
                    return Json(_Result);
                }
            }
            else
            {
                TimeAxis = new Models.TimeAxis();
                TimeAxis.AddTime = DateTime.Now;
                TimeAxis.Code = Guid.NewGuid().ToString();
                TimeAxis.MallCode = model.MallCode;
                TimeAxis.UpdateTime = DateTime.Now;
                dbContext.TimeAxis.Add(TimeAxis);
            }
            var curDate = DateTime.Now;
            for (int i = model.BeginAxis.Value; i <= model.EndAxis.Value; i++)
            {
                var TimeSlot = await dbContext.TimeSlot.Where(t => t.ID == (i + 1)).FirstOrDefaultAsync();
                if (TimeSlot != null)
                {
                    var timeRelate = await dbContext.TimeRelate.Where(t => t.TimeAxisCode == TimeAxis.Code && t.TimeSlotCode == TimeSlot.Code).FirstOrDefaultAsync();
                    if (timeRelate == null)
                    {
                        timeRelate = new Models.TimeRelate();
                        timeRelate.Code = Guid.NewGuid().ToString();
                        timeRelate.AddTime = DateTime.Now;
                        timeRelate.OpenTime = 0;
                        timeRelate.TimeAxisCode = TimeAxis.Code;
                        timeRelate.TimeSlotCode = TimeSlot.Code;
                        timeRelate.UpdateTime = curDate;
                        dbContext.TimeRelate.Add(timeRelate);
                    }
                    else
                    {
                        timeRelate.UpdateTime = curDate;
                        dbContext.TimeRelate.Update(timeRelate);
                    }
                }
            }
            int resultCount = await dbContext.SaveChangesAsync();
            var removeRelateList = await dbContext.TimeRelate.Where(i => i.UpdateTime < curDate && i.TimeAxisCode == TimeAxis.Code).ToListAsync();
            dbContext.TimeRelate.RemoveRange(removeRelateList);
            resultCount += await dbContext.SaveChangesAsync();
            if (resultCount > 0)
            {
                var relateList = await dbContext.TimeRelate.Where(i => i.TimeAxisCode == TimeAxis.Code).Join(dbContext.TimeSlot, tr => tr.TimeSlotCode, ts => ts.Code, (tr, ts) => new
                {
                    tr.TimeAxisCode,
                    TimeRelateCode = tr.Code,
                    ts.BeginTimeSlot,
                    ts.EndTimeSlot,
                    tr.OpenTime,
                    tr.TimeSlotCode,
                    ts.ID
                }).OrderBy(i => i.ID).ToListAsync();
                _Result.Code = "200";
                _Result.Msg = "设置成功";
                _Result.Data = new { TimeRelateList = relateList, TimeAxis };
            }
            else
            {
                _Result.Code = "501";
                _Result.Msg = "设置失败";
                _Result.Data = null;
            }
            return Json(_Result);
        }

        [HttpPost]
        public async Task<IActionResult> SetTimeRelate(Input_TimeRelate model, [FromServices] ContextString dbContext)
        {

            QianMuResult _Result = new QianMuResult();

            //检测用户登录情况
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_TimeRelate)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            foreach (var item in model.TimeRelateList)
            {
                var timeRelate = await dbContext.TimeRelate.Where(t => t.Code == item.TimeRelateCode).FirstOrDefaultAsync();
                if (timeRelate != null)
                {
                    timeRelate.OpenTime = item.OpenTime;
                    timeRelate.UpdateTime = DateTime.Now;
                    dbContext.TimeRelate.Update(timeRelate);
                }
            }

            if (await dbContext.SaveChangesAsync() > 0)
            {
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
    }
}