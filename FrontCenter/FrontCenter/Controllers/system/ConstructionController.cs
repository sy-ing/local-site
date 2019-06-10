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

namespace FrontCenter.Controllers.system
{
    public class ConstructionController : Controller
    {


        /// <summary>
        /// 获取楼栋列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetBuildingList([FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            //检测用户登录情况
            var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            string mallCode = string.Empty;
            if (string.IsNullOrEmpty(uol.MallCode))
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

            var buildings = await dbContext.MallBuilding.Where(i => i.MallCode == mallCode).Join(dbContext.Building.Where(i => !i.IsDel), mb => mb.BuildingCode, bu => bu.Code, (mb, bu) => new
            {
                bu.AddTime,
                bu.Code,
                bu.ID,
                bu.Name,
                bu.NameEn,
                bu.Order,
                bu.UpdateTime

            }).ToListAsync();
            //   var buildings = await dbContext.Building.ToListAsync();

            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = buildings;

            //var ip = Method.GetUserIp(this.HttpContext);
            //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "建筑模块", LogMsg = username + "获取楼栋列表", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
            //dbContext.SaveChanges();
            return Json(_Result);
        }


        /// <summary>
        /// 获取楼层列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetFloorList(string code, [FromServices] ContextString dbContext)
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



            if (string.IsNullOrEmpty(code))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个楼栋编码";
                _Result.Data = "";

                return Json(_Result);
            }

            var floors = await dbContext.Floor.Where(i => i.BuildingCode == code && !i.IsDel).OrderBy(o => o.Order).ToListAsync();
            ArrayList list = new ArrayList();
            foreach (var floor in floors)
            {
                var map = await dbContext.AssetFiles.Where(i => i.Code == floor.Map).FirstOrDefaultAsync();
                if (map == null)
                {

                    list.Add(new
                    {
                        floor.AddTime,
                        floor.Name,
                        floor.Order,
                        floor.ID,
                        Map = "",
                        MapWidth = 0,
                        MapHeigth = 0,
                        FloorCode = floor.Code

                    });
                }
                else
                {
                    list.Add(new
                    {
                        floor.AddTime,
                        floor.Name,
                        floor.Order,
                        floor.ID,
                        //Map = Method.ServerAddr + "/MallSite/" + map.FilePath,
                        Map = Method.OSSServer + map.FilePath,
                        MapWidth = map.Width,
                        MapHeigth = map.Height,
                        FloorCode = floor.Code

                    });

                }
            }

            //var floors = await dbContext.Floors.Where(i => i.BuildingID == (int)id).Join(dbContext.AssetFiles, fl => fl.Map, af => af.ID, (fl, af) => new {
            //    fl.AddTime,
            //    fl.Name,
            //    fl.Order,
            //    fl.ID,
            //    Map =Method.ServerAddr+ af.FilePath

            //}).ToListAsync();
            if (floors != null)
            {
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = list;

                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "建筑模块", LogMsg = username + "获取楼层列表", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
                //dbContext.SaveChanges();

            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "无效的楼栋ID";
                _Result.Data = "";
            }


            return Json(_Result);
        }

        /// <summary>
        /// 获取建筑信息
        /// </summary>
        /// <param name="ConstructionInfo"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetConstructionList(string mallCode, [FromServices] ContextString dbContext)
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
                    //   model.UserName = uol.UserName;
                }
            }


            if (!string.IsNullOrEmpty(mallCode))
            {
                var mallbuildings = await dbContext.Building.Where(i => !i.IsDel).Join(dbContext.MallBuilding.Where(i => i.MallCode == mallCode), bu => bu.Code, mb => mb.BuildingCode, (bu, mb) => new
                {
                    bu.AddTime,
                    bu.Code,
                    bu.ID,
                    bu.Name,
                    bu.NameEn,
                    bu.Order,
                    bu.UpdateTime
                }).OrderBy(o => o.Order).ToListAsync();


                ArrayList Buildings = new ArrayList();
                foreach (var b in mallbuildings)
                {

                    var floors = await dbContext.Floor.Where(i => i.BuildingCode == b.Code && !i.IsDel).OrderBy(o => o.Order).AsNoTracking().ToListAsync();
                    ArrayList arrlist = new ArrayList();
                    foreach (var f in floors)
                    {
                        string Map = string.Empty;
                        var mapfile = await dbContext.AssetFiles.Where(i => i.Code == f.Map).FirstOrDefaultAsync();
                        if (mapfile != null)
                        {
                            //Map = Method.ServerAddr + "/MallSite/" + mapfile.FilePath;
                            Map = Method.OSSServer + mapfile.FilePath;
                        }

                        arrlist.Add(new { f.ID, f.Name, f.NameEn, Map = Map, f.Order, f.AddTime, f.Code });
                    }
                    Buildings.Add(new { b.ID, b.Name, b.NameEn, Order = b.Order, b.AddTime, b.Code, Floors = arrlist });
                }


                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = Buildings;


            }



            //var ip = Method.GetUserIp(this.HttpContext);
            //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "建筑模块", LogMsg = username + "获取建筑信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
            //dbContext.SaveChanges();

            return Json(_Result);
        }


        /// <summary>
        /// 编辑 楼层地图
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FloorEdit(Input_FloorEdit model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();



            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_FloorEdit)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            if (string.IsNullOrEmpty(model.UserName))
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
                    model.UserName = uol.UserName;
                }
            }

            //if (string.IsNullOrEmpty(model.Code) || string.IsNullOrEmpty(model.Code))
            if (string.IsNullOrEmpty(model.Code))
            {

                _Result.Code = "510";
                _Result.Msg = "请输入一个楼层编码";
                _Result.Data = "";
                return Json(_Result);
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                _Result.Code = "510";
                _Result.Msg = "楼层名称不可为空";
                _Result.Data = "";
                return Json(_Result);
            }
            var floor = await dbContext.Floor.Where(i => i.Code == model.Code && !i.IsDel).FirstOrDefaultAsync();
            if (floor == null)
            {
                _Result.Code = "510";
                _Result.Msg = "楼层不存在";
                _Result.Data = "";
                return Json(_Result);
            }
            if (!string.IsNullOrEmpty(model.Code))
            {
                var file = await dbContext.AssetFiles.Where(i => i.Code == model.Code).FirstOrDefaultAsync();

                if (file == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的文件ID";
                    _Result.Data = "";
                    return Json(_Result);
                }
                FileTypeJudgment fimg = new FileTypeJudgment() { TypeTarget = new TypeImg() };
                if (!fimg.Judge(file.FileExtName))
                {
                    _Result.Code = "510";
                    _Result.Msg = "地图文件不是图片";
                    _Result.Data = "";
                    return Json(_Result);
                }
                floor.Map = model.Code;
            }
            floor.Name = model.Name;
            floor.UpdateTime = DateTime.Now;
            dbContext.Floor.Update(floor);
            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "修改成功";
                _Result.Data = "";

                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = model.UserName, ModuleName = "楼层管理", LogMsg = model.UserName + "修改了名称：" + model.Name + "的楼层地图", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });
                dbContext.SaveChanges();

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
        /// 获取所有楼层列表供停车场使用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetAllFloorList(string mallCode, [FromServices] ContextString dbContext)
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
                    //   model.UserName = uol.UserName;
                }
            }

            //检测用户登录情况
            //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
            //if (string.IsNullOrEmpty(username))
            //{
            //    _Result.Code = "401";
            //    _Result.Msg = "请登陆后再进行操作";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}
            var mallbuildings = await dbContext.MallBuilding.Where(i => i.MallCode == mallCode).ToListAsync();
            var floors = await dbContext.Floor.Where(i => !i.IsDel).Join(mallbuildings, f => f.BuildingCode, mb => mb.BuildingCode, (f, mb) => new {
                f.Code,
                f.Name,
                f.Order,
                f.ID,
                f.BuildingCode
            }).OrderBy(o => o.BuildingCode).ThenBy(t => t.Order).ToListAsync();
            ArrayList list = new ArrayList();
            foreach (var floor in floors)
            {
                var bu = await dbContext.Building.Where(i => i.Code == floor.BuildingCode && !i.IsDel).FirstOrDefaultAsync();
                list.Add(new { FloorID = floor.ID, FloorCode = floor.Code, BuildingCode = floor.BuildingCode, Name = bu.Name + floor.Name, Order = floor.Order });
            }

            if (floors != null)
            {
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                //  _Result.Data = floors;
                _Result.Data = list;


                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "建筑模块", LogMsg = username + "获取所有楼层列表供停车场使用", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
                //dbContext.SaveChanges();
            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "无效的楼栋ID";
                _Result.Data = "";
            }


            return Json(_Result);
        }


        /// <summary>
        /// 获取楼层信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetFloorInfo(Input_GetFloorInfo_Local model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            //Stream stream = HttpContext.Request.Body;
            //byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            //stream.Read(buffer, 0, buffer.Length);
            //string inputStr = Encoding.UTF8.GetString(buffer);
            //model = (Input_GetFloorInfo_Local)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            ////检测用户登录情况
            //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
            //if (string.IsNullOrEmpty(username))
            //{
            //    _Result.Code = "401";
            //    _Result.Msg = "请登陆后再进行操作";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}

            if (string.IsNullOrEmpty(model.Code))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个楼层编码";
                _Result.Data = "";

                return Json(_Result);
            }

            var floor = await dbContext.Floor.Where(i => i.Code == model.Code).FirstOrDefaultAsync();
            if (floor == null)
            {
                _Result.Code = "2";
                _Result.Msg = "无效的楼栋ID";
                _Result.Data = "";
                return Json(_Result);
            }
            var map = await dbContext.AssetFiles.Where(i => i.Code == floor.Map).FirstOrDefaultAsync();
            if (map == null)
            {

                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { floor.AddTime, floor.Name, floor.Order, floor.ID, floor.Code, Map = "" };
            }
            else
            {
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new
                {
                    floor.AddTime,
                    floor.Name,
                    floor.Order,
                    floor.ID,
                    floor.Code,
                    //Map = Method.ServerAddr + "/MallSite/" + map.FilePath,
                    Map = Method.OSSServer + map.FilePath,
                    MapCode = map.Code
                };
            }

            //var ip = Method.GetUserIp(this.HttpContext);
            //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "建筑模块", LogMsg = username + "获取ID为："+ id + "的楼层信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
            //dbContext.SaveChanges();

            return Json(_Result);
        }

        /// <summary>
        /// 获取建筑信息
        /// </summary>
        /// <param name="ConstructionInfo"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetConstList(string code, [FromServices] ContextString dbContext)
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


            var buildings = await dbContext.Building.Where(i => !i.IsDel).AsNoTracking().ToListAsync();
            if (!string.IsNullOrEmpty(code))
            {
                buildings = buildings.Where(i => i.Code == code).ToList();
            }

            ArrayList Buildings = new ArrayList();
            foreach (var b in buildings)
            {

                var floors = await dbContext.Floor.Where(i => i.BuildingCode == b.Code && !i.IsDel).OrderBy(o => o.Order).AsNoTracking().ToListAsync();
                ArrayList arrlist = new ArrayList();
                foreach (var f in floors)
                {
                    string Map = string.Empty;
                    var mapfile = await dbContext.AssetFiles.Where(i => i.Code == f.Map).FirstOrDefaultAsync();
                    if (mapfile != null)
                    {
                        Map = mapfile.FilePath;
                    }

                    arrlist.Add(new { f.ID, f.Code, f.Name, f.NameEn, Map = Map, f.Order, f.AddTime });
                }


                Buildings.Add(new { b.ID, b.Code, b.Name, b.NameEn, Order = b.Order, b.AddTime, Floors = arrlist });
            }


            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = Buildings;

            //var ip = Method.GetUserIp(this.HttpContext);
            //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "建筑模块", LogMsg = username + "获取建筑信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
            //dbContext.SaveChanges();
            return Json(_Result);
        }

        /// <summary>
        /// 获取区域信息
        /// </summary>
        /// <param name="ConstructionInfo"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetAreaList(Input_GetAreaList_Local model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            if (string.IsNullOrEmpty(model.MallCode))
            {

                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (Input_GetAreaList_Local)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



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
            var Areas = await dbContext.AreaInfo.Where(i => !i.IsDel).Join(dbContext.MallArea.Where(i => i.MallCode == model.MallCode), ai => ai.Code, ma => ma.AreaCode, (ai, ma) => new {
                ai.ID,
                ai.Code,
                ai.AreaName,
                ai.Order
            }).AsNoTracking().ToListAsync();
            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = Areas;

            //var ip = Method.GetUserIp(this.HttpContext);
            //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "建筑模块", LogMsg = username + "获取区域信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
            //dbContext.SaveChanges();
            return Json(_Result);
        }
    }
}