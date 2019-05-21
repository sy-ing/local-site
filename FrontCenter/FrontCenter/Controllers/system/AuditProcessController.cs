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
    public class AuditProcessController : Controller
    {

        /// <summary>
        /// 添加审核步骤
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(Input_APAdd model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_APAdd)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



            if (String.IsNullOrEmpty(model.OperUser))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入审核人";
                _Result.Data = "";
                return Json(_Result);
            }


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

            if (model.ModuleType == null)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入审核类型";
                _Result.Data = "";
                return Json(_Result);
            }
            else
            {
                if (model.ModuleType < 1 || model.ModuleType > 2)
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入有效的审核类型";
                    _Result.Data = "";
                    return Json(_Result);
                }
            }
            var muser = await dbContext.Account.Where(i => i.Code == model.OperUser && i.Activity).FirstOrDefaultAsync();

            if (muser == null)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入有效的审核人";
                _Result.Data = "";
                return Json(_Result);
            }

            int order = 1;


            var nowcount = await dbContext.AuditProcess.Where(i => i.MallCode == model.MallCode && i.ModuleType == model.ModuleType).CountAsync();

            if (nowcount >= 3)
            {
                _Result.Code = "510";
                _Result.Msg = "最多只能有三个审核步骤";
                _Result.Data = "";
                return Json(_Result);
            }

            if (nowcount <= 0)
            {
                order = 1;
            }
            else
            {
                //order = dbContext.AreaInfo.Max(m => m.Order) + 1;

                order = dbContext.AuditProcess.Where(i => i.MallCode == model.MallCode && i.ModuleType == model.ModuleType).Max(m => m.Order) + 1;
            }

            AuditProcess auditProcess = new AuditProcess
            {
                AddTime = DateTime.Now,
                Code = Guid.NewGuid().ToString(),
                UpdateTime = DateTime.Now,
                MallCode = model.MallCode,
                ModuleType = (int)model.ModuleType,
                OperUser = model.OperUser,
                Order = order
            };


            dbContext.AuditProcess.Add(auditProcess);

            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "添加成功";
                _Result.Data = "";
            }

            return Json(_Result);
        }


        /// <summary>
        /// 删除审核步骤
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Del(Input_APDel model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_APDel)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());




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

            if (model.Code.Count() <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个记录编码";
                _Result.Data = "";
                return Json(_Result);
            }

            foreach (var c in model.Code)
            {
                var apcount = await dbContext.AuditProcess.Where(i => i.Code == c).CountAsync();
                if (apcount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的记录编码:" + c;
                    _Result.Data = "";
                    return Json(_Result);
                }
            }
            bool haschange = false;
            foreach (var c in model.Code)
            {
                var ap = await dbContext.AuditProcess.Where(i => i.Code == c).FirstOrDefaultAsync();
                var list = await dbContext.AuditProcess.Where(i => i.MallCode == ap.MallCode && i.ModuleType == ap.ModuleType && i.Order > ap.Order).ToListAsync();
                foreach (var l in list)
                {
                    l.Order = l.Order - 1;
                }

                dbContext.AuditProcess.Remove(ap);
                dbContext.AuditProcess.UpdateRange(list);
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    haschange = true;
                }
            }




            if (haschange)
            {
                _Result.Code = "200";
                _Result.Msg = "删除成功";
                _Result.Data = "";
            }

            return Json(_Result);
        }



        /// <summary>
        /// 修改审核步骤排序
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> OrderEdit(Input_APOrderEdit model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_APOrderEdit)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


            if (model.Order == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的排序";
                _Result.Data = "";
                return Json(_Result);
            }

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



            var ap = await dbContext.AuditProcess.Where(i => i.Code == model.Code).FirstOrDefaultAsync();

            if (ap == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的记录编码:" + model.Code;
                _Result.Data = "";
                return Json(_Result);
            }


            var allcount = await dbContext.AuditProcess.Where(i => i.MallCode == ap.MallCode && i.ModuleType == ap.ModuleType).CountAsync();


            if (model.Order < 1 || model.Order > allcount)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的排序";
                _Result.Data = "";
                return Json(_Result);
            }

            if (ap.Order == model.Order)
            {
                _Result.Code = "200";
                _Result.Msg = "无需更改";
                _Result.Data = "";

            }

            if (ap.Order > model.Order)
            {
                var list = await dbContext.AuditProcess.Where(i => i.MallCode == ap.MallCode && i.ModuleType == ap.ModuleType && i.Order >= model.Order && i.Order < ap.Order).ToListAsync();

                foreach (var l in list)
                {
                    l.Order = l.Order + 1;
                }
                ap.Order = (int)model.Order;
                dbContext.AuditProcess.Update(ap);
                dbContext.AuditProcess.UpdateRange(list);
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    _Result.Code = "200";
                    _Result.Msg = "编辑成功";
                    _Result.Data = "";
                }
            }
            else
            {
                var list = await dbContext.AuditProcess.Where(i => i.MallCode == ap.MallCode && i.ModuleType == ap.ModuleType && i.Order > ap.Order && i.Order <= model.Order).ToListAsync();

                foreach (var l in list)
                {
                    l.Order = l.Order - 1;
                }
                ap.Order = (int)model.Order;
                dbContext.AuditProcess.Update(ap);
                dbContext.AuditProcess.UpdateRange(list);
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    _Result.Code = "200";
                    _Result.Msg = "编辑成功";
                    _Result.Data = "";
                }
            }



            return Json(_Result);


        }

        /// <summary>
        /// 获取已设置列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetList(Input_APGetList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_APGetList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


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

            if (model.ModuleType == null)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入有效的审核类型";
                _Result.Data = "";
                return Json(_Result);
            }
            if (model.ModuleType < 1 || model.ModuleType > 2)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入有效的审核类型";
                _Result.Data = "";
                return Json(_Result);
            }

            var list = await dbContext.AuditProcess.Where(i => i.ModuleType == model.ModuleType && i.MallCode == model.MallCode).Join(dbContext.Account.Where(i => i.MallCode == model.MallCode && i.Activity), ap => ap.OperUser, mu => mu.Code, (ap, mu) => new {
                ap.AddTime,
                ap.Code,
                ap.ID,
                ap.MallCode,
                ap.ModuleType,
                ap.OperUser,
                ap.Order,
                ap.UpdateTime,
                mu.AccountName
            }).OrderBy(i => i.Order).ToListAsync();


            _Result.Code = "200";
            _Result.Msg = "编辑成功";
            _Result.Data = list;



            return Json(_Result);


        }
    }
}