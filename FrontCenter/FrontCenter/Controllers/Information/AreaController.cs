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

namespace FrontCenter.Controllers.Information
{
    public class AreaController : Controller
    {

        /// <summary>
        /// 创建区域
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(Input_AreaAdd model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_AreaAdd)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



            if (String.IsNullOrEmpty(model.AreaName))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入区域名称";
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
            var areacount = await dbContext.MallArea.Where(i => i.MallCode == model.MallCode).Join(dbContext.AreaInfo.Where(i => i.AreaName.ToLower() == model.AreaName.ToLower() && !i.IsDel), ma => ma.AreaCode, ai => ai.Code, (ma, ai) => new
            {
                ai.AreaName

            }).CountAsync();

            //var area = await dbContext.AreaInfo.Where(i => i.AreaName == model.AreaName).CountAsync();

            if (areacount > 0)
            {
                _Result.Code = "510";
                _Result.Msg = "区域名称已存在";
                _Result.Data = "";
                return Json(_Result);
            }

            int order = 0;


            var nowcount = await dbContext.MallArea.Where(i => i.MallCode == model.MallCode).Join(dbContext.AreaInfo.Where(i => !i.IsDel), ma => ma.AreaCode, ai => ai.Code, (ma, ai) => new
            {
                ai.AreaName

            }).CountAsync();

            if (nowcount <= 0)
            {
                order = 0;
            }
            else
            {
                //order = dbContext.AreaInfo.Max(m => m.Order) + 1;

                order = dbContext.MallArea.Where(i => i.MallCode == model.MallCode).Join(dbContext.AreaInfo.Where(i => !i.IsDel), ma => ma.AreaCode, ai => ai.Code, (ma, ai) => new
                {
                    ai.AreaName,
                    ai.Order
                }).Max(m => m.Order) + 1;
            }

            AreaInfo areaInfo = new AreaInfo
            {
                AddTime = DateTime.Now,
                Code = Guid.NewGuid().ToString(),
                UpdateTime = DateTime.Now,
                AreaName = model.AreaName,
                Order = order

            };
            dbContext.AreaInfo.Add(areaInfo);
            dbContext.MallArea.Add(new MallArea
            {
                AddTime = DateTime.Now,
                Code = Guid.NewGuid().ToString(),
                UpdateTime = DateTime.Now,
                AreaCode = areaInfo.Code,
                MallCode = model.MallCode
            });

            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "添加成功";
                _Result.Data = "";
            }

            return Json(_Result);
        }

        /// <summary>
        /// 修改区域名称
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Input_AreaEdit model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_AreaEdit)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



            if (String.IsNullOrEmpty(model.AreaName))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入区域名称";
                _Result.Data = "";
                return Json(_Result);
            }

            // var count = await dbContext.AreaInfo.Where(i => i.AreaName == model.AreaName && i.Code != model.AreaCode).CountAsync();
            var mallarea = await dbContext.MallArea.Where(i => i.AreaCode == model.AreaCode).FirstOrDefaultAsync();

            var count = await dbContext.MallArea.Where(i => i.MallCode == mallarea.MallCode).Join(dbContext.AreaInfo.Where(i => i.AreaName == model.AreaName && i.Code != model.AreaCode && !i.IsDel), ma => ma.AreaCode, ai => ai.Code, (ma, ai) => ai).CountAsync();
            if (count > 0)
            {
                _Result.Code = "510";
                _Result.Msg = "区域名称重复";
                _Result.Data = "";
                return Json(_Result);
            }

            if (String.IsNullOrEmpty(model.AreaCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入区域编码";
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


            var area = await dbContext.AreaInfo.Where(i => i.Code == model.AreaCode).FirstOrDefaultAsync();

            //var area = await dbContext.AreaInfo.Where(i => i.AreaName == model.AreaName).CountAsync();

            if (area == null)
            {
                _Result.Code = "510";
                _Result.Msg = "区域不存在";
                _Result.Data = "";
                return Json(_Result);
            }

            area.AreaName = model.AreaName;
            area.UpdateTime = DateTime.Now;
            dbContext.AreaInfo.Update(area);


            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "更新成功";
                _Result.Data = "";
            }

            return Json(_Result);
        }



        /// <summary>
        /// 删除区域
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Del(Input_AreaDel model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //检测用户登录情况
            var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (string.IsNullOrEmpty(uol.UserName))
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
            model = (Input_AreaDel)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



            if (model.Code.Count() <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入区域编码";
                _Result.Data = "";
                return Json(_Result);
            }

            foreach (var c in model.Code)
            {
                var area = await dbContext.AreaInfo.Where(i => i.Code == c).FirstOrDefaultAsync();
                if (area == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的编码：" + c;
                    _Result.Data = "";
                    return Json(_Result);
                }

                var usecount = await dbContext.Shops.Where(i => i.AreaCode == c && !i.IsDel).CountAsync();
                if (usecount > 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "区域：" + area.AreaName + "正在使用，不可删除。";
                    _Result.Data = "";
                    return Json(_Result);
                }


                var mallarea = await dbContext.MallArea.Where(i => i.AreaCode == c).FirstOrDefaultAsync();
                area.IsDel = true;
                area.UpdateTime = DateTime.Now;
                dbContext.AreaInfo.Update(area);
                //dbContext.MallArea.Remove(mallarea);
            }

            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "删除成功";
                _Result.Data = "";
            }

            return Json(_Result);
        }


        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetList(Input_GetAreaList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetAreaList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


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
                    model.UserName = uol.UserName;
                }
            }




            // var arealist = await dbContext.AreaInfo.Where(i => i.Code == model.AreaCode).FirstOrDefaultAsync();

            var arealist = await dbContext.MallArea.Where(i => i.MallCode == model.MallCode).Join(dbContext.AreaInfo.Where(i => !i.IsDel), ma => ma.AreaCode, ai => ai.Code, (ma, ai) => new
            {
                ai.AreaName,
                AddTime = ai.AddTime.ToString("yyyy-MM-dd HH:mm:ss"),
                ai.Code,
                ai.ID,
                ai.Order,
                UpdateTime = ai.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")


            }).ToListAsync();

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
                _Result.Data = arealist;
            }
            else
            {

                int allPage = 1;
                int allCount = arealist.Count();
                allPage = (int)(allCount / model.PageSize);

                if (allCount % model.PageSize > 0)
                {
                    allPage = allPage + 1;
                }
                arealist = arealist.Skip(((int)model.PageIndex - 1) * (int)model.PageSize).Take((int)model.PageSize).ToList();
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { List = arealist, AllPage = allPage, AllCount = allCount };
            }

            return Json(_Result);
        }

        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetInfo(Input_GetAreaInfo model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetAreaInfo)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


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
                    model.UserName = uol.UserName;
                }
            }



            if (string.IsNullOrEmpty(model.AreaCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个区域编码";
                _Result.Data = "";
                return Json(_Result);
            }
            // var arealist = await dbContext.AreaInfo.Where(i => i.Code == model.AreaCode).FirstOrDefaultAsync();

            var areacount = await dbContext.AreaInfo.Where(i => i.Code == model.AreaCode).CountAsync();
            if (areacount <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的区域编码：" + model.AreaCode;
                _Result.Data = "";
                return Json(_Result);
            }

            var area = await dbContext.AreaInfo.Where(i => i.Code == model.AreaCode).Select(s => new
            {
                s.AreaName,
                AddTime = s.AddTime.ToString("yyyy-MM-dd HH:mm:ss"),
                s.Code,
                s.ID,
                s.Order,
                UpdateTime = s.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            }).FirstOrDefaultAsync();

            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = area;

            return Json(_Result);
        }
    }
}