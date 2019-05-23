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
    public class BuildingController : Controller
    {
        /// <summary>
        /// 创建楼栋
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(Input_BuildingAdd model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_BuildingAdd)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



            if (String.IsNullOrEmpty(model.BuildingName))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入楼栋名称";
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
            var Buildingcount = await dbContext.MallBuilding.Where(i => i.MallCode == model.MallCode).Join(dbContext.Building.Where(i => i.Name.ToLower() == model.BuildingName.ToLower() && !i.IsDel), ma => ma.BuildingCode, ai => ai.Code, (ma, ai) => new
            {
                ai.Name

            }).CountAsync();

            //var Building = await dbContext.BuildingInfo.Where(i => i.BuildingName == model.BuildingName).CountAsync();

            if (Buildingcount > 0)
            {
                _Result.Code = "510";
                _Result.Msg = "楼栋名称已存在";
                _Result.Data = "";
                return Json(_Result);
            }

            int order = 0;


            var nowcount = await dbContext.MallBuilding.Where(i => i.MallCode == model.MallCode).Join(dbContext.Building.Where(i => !i.IsDel), ma => ma.BuildingCode, ai => ai.Code, (ma, ai) => new
            {
                ai.Name

            }).CountAsync();

            if (nowcount <= 0)
            {
                order = 0;
            }
            else
            {
                //order = dbContext.BuildingInfo.Max(m => m.Order) + 1;

                order = dbContext.MallBuilding.Where(i => i.MallCode == model.MallCode).Join(dbContext.Building.Where(i => !i.IsDel), ma => ma.BuildingCode, ai => ai.Code, (ma, ai) => new
                {
                    ai.Name,
                    ai.Order
                }).Max(m => m.Order) + 1;
            }

            Building BuildingInfo = new Building
            {
                AddTime = DateTime.Now,
                Code = Guid.NewGuid().ToString(),
                UpdateTime = DateTime.Now,
                Name = model.BuildingName,
                NameEn = string.Empty,
                Order = order

            };







            dbContext.Building.Add(BuildingInfo);
            dbContext.MallBuilding.Add(new MallBuilding
            {
                AddTime = DateTime.Now,
                Code = Guid.NewGuid().ToString(),
                UpdateTime = DateTime.Now,
                BuildingCode = BuildingInfo.Code,
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
        /// 修改楼栋名称
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Input_BuildingEdit model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_BuildingEdit)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



            if (String.IsNullOrEmpty(model.BuildingName))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入楼栋名称";
                _Result.Data = "";
                return Json(_Result);
            }

            if (String.IsNullOrEmpty(model.BuildingCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入楼栋编码";
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

            var mallbuild = await dbContext.MallBuilding.Where(i => i.BuildingCode == model.BuildingCode).FirstOrDefaultAsync();

            var count = await dbContext.MallBuilding.Where(i => i.MallCode == mallbuild.MallCode).Join(dbContext.Building.Where(i => i.Name == model.BuildingName && i.Code != model.BuildingCode && !i.IsDel), ma => ma.BuildingCode, bu => bu.Code, (ma, bu) => bu).CountAsync();
            if (count > 0)
            {
                _Result.Code = "510";
                _Result.Msg = "楼栋名称重复";
                _Result.Data = "";
                return Json(_Result);
            }


            var Building = await dbContext.Building.Where(i => i.Code == model.BuildingCode).FirstOrDefaultAsync();

            //var Building = await dbContext.BuildingInfo.Where(i => i.BuildingName == model.BuildingName).CountAsync();

            if (Building == null)
            {
                _Result.Code = "510";
                _Result.Msg = "楼栋不存在";
                _Result.Data = "";
                return Json(_Result);
            }

            Building.Name = model.BuildingName;
            Building.UpdateTime = DateTime.Now;
            dbContext.Building.Update(Building);


            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "更新成功";
                _Result.Data = "";
            }

            return Json(_Result);
        }



        /// <summary>
        /// 删除楼栋
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Del(Input_BuildingDel model, [FromServices] ContextString dbContext)
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
            model = (Input_BuildingDel)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



            if (model.Code.Count() <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入楼栋编码";
                _Result.Data = "";
                return Json(_Result);
            }

            foreach (var c in model.Code)
            {
                var Building = await dbContext.Building.Where(i => i.Code == c).FirstOrDefaultAsync();
                if (Building == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的编码：" + c;
                    _Result.Data = "";
                    return Json(_Result);
                }

                var floorcount = await dbContext.Floor.Where(i => i.BuildingCode == c && !i.IsDel).CountAsync();
                var shopcount = await dbContext.Shops.Where(i => i.BuildingCode == c && !i.IsDel).CountAsync();
                var devcount = await dbContext.Device.Where(i => i.Building == c).CountAsync();

                var allcount = floorcount + shopcount + devcount;
                if (allcount > 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "楼栋:" + Building.Name + "正在使用中不可删除。";
                    _Result.Data = "";
                    return Json(_Result);
                }


                var mallBuilding = await dbContext.MallBuilding.Where(i => i.BuildingCode == c).FirstOrDefaultAsync();
                Building.IsDel = true;
                Building.UpdateTime = DateTime.Now;
                dbContext.Building.Update(Building);
                //dbContext.MallBuilding.Remove(mallBuilding);
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
        /// 获取楼栋列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetList(Input_GetBuildingList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetBuildingList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


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




            // var Buildinglist = await dbContext.BuildingInfo.Where(i => i.Code == model.BuildingCode).FirstOrDefaultAsync();

            var Buildinglist = await dbContext.MallBuilding.Where(i => i.MallCode == model.MallCode).Join(dbContext.Building.Where(i => !i.IsDel), ma => ma.BuildingCode, ai => ai.Code, (ma, ai) => new
            {
                ai.Name,
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
                _Result.Data = Buildinglist;
            }
            else
            {

                int allPage = 1;
                int allCount = Buildinglist.Count();
                allPage = (int)(allCount / model.PageSize);

                if (allCount % model.PageSize > 0)
                {
                    allPage = allPage + 1;
                }
                Buildinglist = Buildinglist.Skip(((int)model.PageIndex - 1) * (int)model.PageSize).Take((int)model.PageSize).ToList();
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { List = Buildinglist, AllPage = allPage, AllCount = allCount };
            }

            return Json(_Result);
        }

        /// <summary>
        /// 获取楼栋列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetInfo(Input_GetBuildingInfo model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetBuildingInfo)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


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



            if (string.IsNullOrEmpty(model.BuildingCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个楼栋编码";
                _Result.Data = "";
                return Json(_Result);
            }
            // var Buildinglist = await dbContext.BuildingInfo.Where(i => i.Code == model.BuildingCode).FirstOrDefaultAsync();

            var Buildingcount = await dbContext.Building.Where(i => i.Code == model.BuildingCode).CountAsync();
            if (Buildingcount <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的楼栋编码：" + model.BuildingCode;
                _Result.Data = "";
                return Json(_Result);
            }

            var Building = await dbContext.Building.Where(i => i.Code == model.BuildingCode).Select(s => new
            {
                s.Name,
                AddTime = s.AddTime.ToString("yyyy-MM-dd HH:mm:ss"),
                s.Code,
                s.ID,
                s.Order,
                UpdateTime = s.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            }).FirstOrDefaultAsync();

            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = Building;

            return Json(_Result);
        }

        /// <summary>
        /// 获取楼栋列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetListByUnionID(Input_GetBuildingListByUnionID model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetBuildingListByUnionID)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


            if (String.IsNullOrEmpty(model.UnionID))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入有效的身份编码";
                _Result.Data = "";
                return Json(_Result);
            }


            var sacount = await dbContext.ShopAccount.Where(i => i.UnionID == model.UnionID).FirstOrDefaultAsync();

            if (sacount == null)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入有效的身份编码";
                _Result.Data = "";
                return Json(_Result);
            }



            var mallshop = await dbContext.MallShop.Where(i => i.ShopCode == sacount.ShopCode).FirstOrDefaultAsync();

            // var Buildinglist = await dbContext.BuildingInfo.Where(i => i.Code == model.BuildingCode).FirstOrDefaultAsync();

            var Buildinglist = await dbContext.MallBuilding.Where(i => i.MallCode == mallshop.MallCode).Join(dbContext.Building.Where(i => !i.IsDel), ma => ma.BuildingCode, ai => ai.Code, (ma, ai) => new
            {
                ai.Name,
                AddTime = ai.AddTime.ToString("yyyy-MM-dd HH:mm:ss"),
                ai.Code,
                ai.ID,
                ai.Order,
                UpdateTime = ai.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")


            }).ToListAsync();

            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = Buildinglist;

            return Json(_Result);
        }

    }
}