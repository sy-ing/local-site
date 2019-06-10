using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FrontCenter.AppCode;
using FrontCenter.Models;
using FrontCenter.Models.Data;
using FrontCenter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontCenter.Controllers.Information
{
    public class ParkingController : Controller
    {



        /// <summary>
        /// 添加停车场
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ParkingLotAdd(string FloorCode, string userNmae, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            if (string.IsNullOrEmpty(userNmae))
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
                    userNmae = uol.UserName;
                }
            }
            if (string.IsNullOrEmpty(FloorCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个楼层编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var floor = await dbContext.Floor.Where(i => i.Code == FloorCode && !i.IsDel).FirstOrDefaultAsync();

            if (floor == null)
            {
                _Result.Code = "510";
                _Result.Msg = "需要有效的楼层ID";
                _Result.Data = "";
                return Json(_Result);
            }

            dbContext.ParkingLot.Add(new ParkingLot
            {
                AddTime = DateTime.Now,
                FloorCode = FloorCode,
                IsDel = false,
                Code = Guid.NewGuid().ToString(),
                UpdateTime = DateTime.Now
            });

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

            var ip = Method.GetUserIp(this.HttpContext);
            dbContext.SysLog.Add(new SysLog { AccountName = userNmae, ModuleName = "停车场管理", LogMsg = userNmae + "将楼层Code为：" + FloorCode + ",楼层名称为：" + floor.Name + "的楼层标记为停车场", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });
            dbContext.SaveChanges();

            return Json(_Result);
        }


        /// <summary>
        /// 删除停车场
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ParkingLotDel(string code, string userNmae, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            if (string.IsNullOrEmpty(userNmae))
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
                    userNmae = uol.UserName;
                }
            }



            if (string.IsNullOrEmpty(code))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个停车场编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var pl = await dbContext.ParkingLot.Where(i => i.Code == code && !i.IsDel).FirstOrDefaultAsync();

            if (pl == null)
            {
                _Result.Code = "510";
                _Result.Msg = "需要有效的停车场ID";
                _Result.Data = "";
                return Json(_Result);
            }

            var count = await dbContext.ParkingSpace.Where(i => i.ParkCode == code && !i.IsDel).CountAsync();
            if (count > 0)
            {
                _Result.Code = "510";
                _Result.Msg = "该停车场有：" + count + "个停车位在使用不可移除";
                _Result.Data = "";
                return Json(_Result);
            }


            pl.IsDel = true;
            pl.UpdateTime = DateTime.Now;
            dbContext.ParkingLot.Update(pl);



            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "删除成功";
                _Result.Data = "";

                var floor = dbContext.Floor.Where(i => i.Code == code).FirstOrDefault();
                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = userNmae, ModuleName = "停车场管理", LogMsg = userNmae + "将楼层编码为：" + code + "楼层名称为：" + floor == null ? "" : floor.Name + "的楼层从停车场移除", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });
                dbContext.SaveChanges();

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
        /// 编辑停车场信息
        /// </summary>
        /// <param name="floorids"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ParkingLotEdit(Input_PLEdit model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_PLEdit)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
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
                    model.UserName = uol.UserName;
                    model.MallCode = uol.MallCode;
                }
            }

            if (model.FloorCodes.Count() <= 0)
            {
                var spacecount = await dbContext.MallBuilding.Where(i => i.MallCode == model.MallCode).Join(
                  dbContext.Building.Where(i => !i.IsDel), mb => mb.BuildingCode, bu => bu.Code, (mb, bu) => bu).Join(
                  dbContext.Floor.Where(i => !i.IsDel), bu => bu.Code, fl => fl.BuildingCode, (bu, fl) => fl).Join(
                  dbContext.ParkingLot.Where(i => !i.IsDel), fl => fl.Code, pl => pl.FloorCode, (fl, pl) => pl).Join(
                  dbContext.ParkingSpace.Where(i => !i.IsDel), pl => pl.Code, ps => ps.ParkCode, (pl, ps) => ps
                  ).AsNoTracking().CountAsync();

                //var spacecount = await dbContext.ParkingSpace.Where(i => !i.IsDel).CountAsync();
                if (spacecount > 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "Erro:有" + spacecount + "个停车位正在被使用不可将停车场清空";
                    _Result.Data = "";
                    return Json(_Result);
                }

                // var pllist = dbContext.ParkingLot.Where(i => !i.IsDel).ToList();


                var pllist = await dbContext.MallBuilding.Where(i => i.MallCode == model.MallCode).Join(
                    dbContext.Building.Where(i => !i.IsDel), mb => mb.BuildingCode, bu => bu.Code, (mb, bu) => bu).Join(
                    dbContext.Floor.Where(i => !i.IsDel), bu => bu.Code, fl => fl.BuildingCode, (bu, fl) => fl).Join(
                    dbContext.ParkingLot.Where(i => !i.IsDel), fl => fl.Code, pl => pl.FloorCode, (fl, pl) => pl).AsNoTracking().ToListAsync();



                foreach (var pl in pllist)
                {
                    pl.IsDel = true;
                    pl.UpdateTime = DateTime.Now;
                }

                dbContext.ParkingLot.UpdateRange(pllist);
                await dbContext.SaveChangesAsync();
                _Result.Code = "200";
                _Result.Msg = "编辑成功";
                _Result.Data = "";

                return Json(_Result);
            }


            foreach (var item in model.FloorCodes)
            {
                if (string.IsNullOrEmpty(item))
                {
                    _Result.Code = "510";
                    _Result.Msg = "Erro:编码不可为空";
                    _Result.Data = "";
                    return Json(_Result);
                }
                var floor = await dbContext.Floor.Where(i => i.Code == item).AsNoTracking().FirstOrDefaultAsync();
                if (floor == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "需要有效的楼层编码";
                    _Result.Data = "";
                    return Json(_Result);
                }
            }


            //   var pls = await dbContext.ParkingLot.Where(i => !i.IsDel).ToListAsync();

            var pls = await dbContext.MallBuilding.Where(i => i.MallCode == model.MallCode).Join(
                dbContext.Building.Where(i => !i.IsDel), mb => mb.BuildingCode, bu => bu.Code, (mb, bu) => bu).Join(
                dbContext.Floor.Where(i => !i.IsDel), bu => bu.Code, fl => fl.BuildingCode, (bu, fl) => fl).Join(
                dbContext.ParkingLot.Where(i => !i.IsDel), fl => fl.Code, pl => pl.FloorCode, (fl, pl) => pl).AsNoTracking().ToListAsync();

            bool haschange = false;
            foreach (var pl in pls)
            {
                //ID不在新的列表中 删除
                if (model.FloorCodes.Where(i => i == pl.FloorCode).Count() <= 0)
                {
                    var scount = await dbContext.ParkingSpace.Where(i => i.ParkCode == pl.Code && !i.IsDel).CountAsync();
                    if (scount > 0)
                    {
                        var floor = await dbContext.Floor.Where(i => i.Code == pl.FloorCode && !i.IsDel).FirstOrDefaultAsync();
                        var bu = await dbContext.Building.Where(i => i.Code == floor.BuildingCode && !i.IsDel).FirstOrDefaultAsync();
                        var plName = bu.Name + floor.Name;
                        _Result.Code = "510";
                        _Result.Msg = "Erro:停车场" + plName + "有" + scount + "个停车位正在被使用,不可移除";
                        _Result.Data = "";
                        return Json(_Result);
                    }

                    pl.IsDel = true;
                    pl.UpdateTime = DateTime.Now;
                    dbContext.ParkingLot.Update(pl);
                    haschange = true;
                }

            }

            foreach (var code in model.FloorCodes)
            {
                //ID不在旧的列表中 添加
                if (pls.Where(i => i.FloorCode == code).Count() <= 0)
                {

                    dbContext.ParkingLot.Add(new ParkingLot
                    {
                        AddTime = DateTime.Now,
                        FloorCode = code,
                        IsDel = false,
                        Code = Guid.NewGuid().ToString(),
                        UpdateTime = DateTime.Now
                    });
                    haschange = true;
                }
            }
            if (!haschange)
            {
                _Result.Code = "200";
                _Result.Msg = "无数据变更";
                _Result.Data = "";
                return Json(_Result);
            }


            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "添加成功";
                _Result.Data = "";

                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = model.UserName, ModuleName = "停车场管理", LogMsg = model.UserName + "编辑停车场信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });
                dbContext.SaveChanges();
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
        /// 获取停车场列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetParkingLotList(Input_PLL model, [FromServices] ContextString dbContext)
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

            var list = await dbContext.MallBuilding.Where(i => i.MallCode == model.MallCode).Join(dbContext.Building.Where(i => !i.IsDel), mb => mb.BuildingCode, bu => bu.Code, (mb, bu) => new
            {

                BName = bu.Name,
                BCode = bu.Code

            }).Join(dbContext.Floor.Where(i => !i.IsDel), bu => bu.BCode, fl => fl.BuildingCode, (bu, fl) => new
            {

                bu.BCode,
                fl.Map,
                Name = bu.BName + fl.Name,
                FCode = fl.Code

            }).Join(dbContext.ParkingLot.Where(i => !i.IsDel), fl => fl.FCode, pl => pl.FloorCode, (fl, pl) => new
            {
                pl.ID,
                pl.Code,
                pl.FloorCode,
                fl.Name,
                fl.Map

            }).AsNoTracking().ToListAsync();

            ArrayList arrayList = new ArrayList();
            foreach (var pl in list)
            {
                var assetFiles = await dbContext.AssetFiles.Where(i => i.Code == pl.Map).FirstOrDefaultAsync();
                arrayList.Add(new
                {
                    pl.ID,
                    pl.Code,
                    pl.FloorCode,
                    pl.Name,
                    Map = assetFiles == null ? "" : Method.OSSServer + assetFiles.FilePath
                });

            }
            //var list = await dbContext.MallBuilding.Where(i => i.MallCode == model.MallCode).Join(dbContext.Building.Where(i => !i.IsDel), mb => mb.BuildingCode, bu => bu.Code, (mb, bu) => new
            //{

            //    BName = bu.Name,
            //    BCode = bu.Code

            //}).Join(dbContext.Floor.Where(i => !i.IsDel), bu => bu.BCode, fl => fl.BuildingCode, (bu, fl) => new
            //{

            //    bu.BCode,
            //    fl.Map,
            //    Name = bu.BName + fl.Name,
            //    FCode = fl.Code

            //}).Join(dbContext.AssetFiles, fl => fl.Map, af => af.Code, (fl, af) => new
            //{
            //    //Map = Method.ServerAddr + "/MallSite/" + af.FilePath,
            //    Map = Method.OSSServer + af.FilePath,
            //    fl.FCode,
            //    fl.Name


            //}).Join(dbContext.ParkingLot.Where(i => !i.IsDel), fl => fl.FCode, pl => pl.FloorCode, (fl, pl) => new
            //{
            //    pl.ID,
            //    pl.Code,
            //    pl.FloorCode,
            //    fl.Name,
            //    fl.Map

            //}).AsNoTracking().ToListAsync();

            /*
            var list = await dbContext.ParkingLot.Where(i=>!i.IsDel).Join(dbContext.Floor, pl => pl.FloorCode, fl => fl.Code, (pl, fl) => new
            {
                pl.ID,
                pl.Code,
                pl.FloorCode,
                fl.Name,
                fl.BuildingCode,
                fl.Map
            }).Join(dbContext.Building, pl => pl.BuildingCode, bu => bu.Code, (pl, bu) => new
            {
                pl.ID,
                pl.Code,
                pl.FloorCode,
                Name = bu.Name + pl.Name,
                pl.Map
            }).Join(dbContext.AssetFiles, pl => pl.Map, af => af.Code, (pl, af) => new {
                pl.ID,
                pl.Code,
                pl.FloorCode,
                pl.Name,
                Map = Method.ServerAddr + af.FilePath
            }).AsNoTracking().ToListAsync();
            */
            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = arrayList;

            //var ip = Method.GetUserIp(this.HttpContext);
            //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "建筑模块", LogMsg = username + "获取停车场列表", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
            //dbContext.SaveChanges();

            return Json(_Result);
        }



        /// <summary>
        /// 添加停车位
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ParkingSpaceAdd(Input_ParkingSpace model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_ParkingSpace)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
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

            if (string.IsNullOrEmpty(model.ParkCode) || string.IsNullOrEmpty(model.Num) ||
                string.IsNullOrEmpty(model.Xaxis) || string.IsNullOrEmpty(model.Yaxis) ||
                string.IsNullOrEmpty(model.NavXaxis) || string.IsNullOrEmpty(model.NavYaxis))
            {
                _Result.Code = "510";
                _Result.Msg = "输入项中存在空值";
                _Result.Data = "";
                return Json(_Result);
            }

            var pk = await dbContext.ParkingLot.Where(i => i.Code == model.ParkCode && !i.IsDel).FirstOrDefaultAsync();

            if (pk == null)
            {
                _Result.Code = "510";
                _Result.Msg = "需要有效的停车场编码";
                _Result.Data = "";
                return Json(_Result);
            }

            model.Num = Regex.Replace(model.Num, "；", ";");
            var numList = model.Num.Split(";");
            var errorAlready = new ArrayList();
            var spaceList = new List<ParkingSpace>();

            foreach (var item in numList)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var numCount = await dbContext.ParkingSpace.Where(i => i.ParkCode == pk.Code && i.Num == item && !i.IsDel).CountAsync();
                    if (numCount > 0)
                    {
                        errorAlready.Add(item);
                    }
                    else
                    {
                        spaceList.Add(new ParkingSpace
                        {
                            AddTime = DateTime.Now,
                            Num = item,
                            ParkCode = pk.Code,
                            Xaxis = model.Xaxis,
                            Yaxis = model.Yaxis,
                            NavXaxis = model.NavXaxis,
                            NavYaxis = model.NavYaxis,
                            Code = Guid.NewGuid().ToString(),
                            IsDel = false,
                            UpdateTime = DateTime.Now

                        });
                    }
                }
            }
            //var pscount = await dbContext.ParkingSpace.Where(i => i.Num == model.Num && !i.IsDel).CountAsync();

            //if (pscount > 0)
            if (errorAlready.Count > 0)
            {
                _Result.Code = "510";
                _Result.Msg = "停车位编码:" + string.Join(";", (string[])errorAlready.ToArray(typeof(string))) + "已存在";
                _Result.Data = "";
                return Json(_Result);
            }
            dbContext.ParkingSpace.AddRange(spaceList);
            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "添加成功";
                _Result.Data = "";

                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = model.UserName, ModuleName = "停车场管理", LogMsg = model.UserName + "添加停车位", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "创建", IP = ip });
                dbContext.SaveChanges();
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
        /// 删除停车位
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ParkingSpaceDel(string code, string userName, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            if (string.IsNullOrEmpty(userName))
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
                    userName = uol.UserName;
                }
            }



            if (string.IsNullOrEmpty(code))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个停车位编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var ps = await dbContext.ParkingSpace.Where(i => i.Code == code && !i.IsDel).FirstOrDefaultAsync();

            if (ps == null)
            {
                _Result.Code = "510";
                _Result.Msg = "需要有效的停车位编码";
                _Result.Data = "";
                return Json(_Result);
            }

            ps.IsDel = true;
            ps.UpdateTime = DateTime.Now;

            dbContext.ParkingSpace.Update(ps);

            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "删除成功";
                _Result.Data = "";


                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = userName, ModuleName = "停车场管理", LogMsg = userName + "删除了编号为：" + ps.Num + "的停车位", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "删除", IP = ip });
                dbContext.SaveChanges();

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
        /// 获取停车位列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetParkingSpaceList(Input_GetPSL model, [FromServices] ContextString dbContext)
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

            var list = await dbContext.ParkingSpace.Where(i => i.ParkCode == model.ParkCode && !i.IsDel).OrderBy(o => o.ParkCode).ThenByDescending(t => t.AddTime).AsNoTracking().ToListAsync();



            //是否分页返回
            if (model.Paging != 0)
            {

                list = list.Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();

            }


            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = list;


            //var ip = Method.GetUserIp(this.HttpContext);
            //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "建筑模块", LogMsg = username + "获取停车位列表", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
            //dbContext.SaveChanges();

            return Json(_Result);
        }


        /// <summary>
        /// 获取停车位信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetParkingSpaceInfo(String SpaceNum, [FromServices] ContextString dbContext)
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

            if (String.IsNullOrEmpty(SpaceNum))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入停车位ID";
                _Result.Data = "";
                return Json(_Result);
            }

            var parkingspace = await dbContext.ParkingSpace.Where(i => i.Num == SpaceNum && !i.IsDel).Join(dbContext.ParkingLot, ps => ps.ParkCode, pl => pl.Code, (ps, pl) => new {
                ps.AddTime,
                ps.ID,
                ps.Num,
                ps.ParkCode,
                ps.Xaxis,
                ps.Yaxis,
                pl.FloorCode,
                ps.NavXaxis,
                ps.NavYaxis
            }).Join(dbContext.Floor, ps => ps.FloorCode, fl => fl.Code, (ps, fl) => new {
                ps.AddTime,
                ps.ID,
                ps.Num,
                ps.ParkCode,
                ps.Xaxis,
                ps.Yaxis,
                ps.NavXaxis,
                ps.NavYaxis,
                FloorOrder = fl.Order
            }).AsNoTracking().FirstOrDefaultAsync();

            if (parkingspace != null)
            {
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = parkingspace;

                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "建筑模块", LogMsg = username + "获取编号为："+SpaceNum+"的停车位信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
                //dbContext.SaveChanges();

            }
            else
            {
                _Result.Code = "510";
                _Result.Msg = "无效的停车位ID";
                _Result.Data = "";
            }



            return Json(_Result);
        }
    }
}