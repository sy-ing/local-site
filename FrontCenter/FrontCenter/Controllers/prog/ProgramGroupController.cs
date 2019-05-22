using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

namespace FrontCenter.Controllers.prog
{
    public class ProgramGroupController : Controller
    {
        /// <summary>
        /// 返回节目组列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetProgramGroupList(string ScreenCode, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                //  var list = await dbContext.ProgramGroup.Select(s => new { s.ID, s.GroupName }).ToListAsync();
                //var list = await dbContext.ProgramGroupList.FromSql(@"select a.ID,a.GroupName,b.SName as ScreenInfo,COUNT(c.ID) as  ProgramCount  from  ProgramGroup a left join ScreenInfo b on a.ScreenInfoID = b.ID left join ProgramToGroup c on a.ID = c.GroupID
                //group by a.ID,a.GroupName,b.SName").ToListAsync();



                ////检测用户登录情况
                UserOnLine uol = Method.GetLoginUserName(dbContext, this.HttpContext);
                if (uol == null || string.IsNullOrEmpty(uol.UserName))
                {
                    _Result.Code = "401";
                    _Result.Msg = "请登陆后再进行操作";
                    _Result.Data = "";
                    return Json(_Result);
                }

                //查询数据
                var list = await dbContext.ProgramGroup.Join(dbContext.ScreenInfo, pg => pg.ScreenInfoCode, si => si.Code, (pg, si) => new {
                    pg.ID,
                    pg.Code,
                    pg.GroupName,
                    ScreenInfo = si.SName,
                    pg.ScreenInfoCode,
                    pg.AddTime
                }).OrderByDescending(o => o.AddTime).ToListAsync();

                //过滤数据
                if (!string.IsNullOrEmpty(ScreenCode))
                {
                    list = list.Where(i => i.ScreenInfoCode == ScreenCode).ToList();
                }
                //获取行数
                ArrayList array = new ArrayList();
                foreach (var item in list)
                {
                    var ProgramCount = dbContext.ProgramToGroup.Where(i => i.GroupCode == item.Code).Count();
                    array.Add(new { item.ID, item.Code, item.GroupName, item.ScreenInfo, item.ScreenInfoCode, ProgramCount });
                    //item.DeviceCount =  dbContext.DeviceToGroups.Where(i => i.GroupID == item.ID).Count();
                }


                _Result.Code = "200";
                _Result.Msg = "获取成功";
                // _Result.Data = list;
                _Result.Data = array;


                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "节目模块", LogMsg = username + "查询节目组列表", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
                //dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "Erro:" + e.ToString();
                _Result.Data = "";

            }
            return Json(_Result);
        }

        /// <summary>
        /// 返回节目组列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetProgramGroupListByPage(Input_ProgramGroupQuery model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                UserOnLine uol = Method.GetLoginUserName(dbContext, this.HttpContext);
                if (uol == null || string.IsNullOrEmpty(uol.UserName))
                {
                    _Result.Code = "401";
                    _Result.Msg = "请登陆后再进行操作";
                    _Result.Data = "";
                    return Json(_Result);
                }

                //查询数据
                var list = await dbContext.ProgramGroup.Join(dbContext.ScreenInfo, pg => pg.ScreenInfoCode, si => si.Code, (pg, si) => new
                {
                    pg.ID,
                    pg.Code,
                    pg.GroupName,
                    ScreenInfo = si.SName,
                    pg.ScreenInfoCode,
                    pg.AddTime
                }).OrderByDescending(o => o.AddTime).ToListAsync();

                //过滤数据
                if (!string.IsNullOrEmpty(model.ScreenCode))
                {
                    list = list.Where(i => i.ScreenInfoCode == model.ScreenCode).ToList();
                }

                int allPage = 1;
                var allCount = list.Count();
                model.Paging = model.Paging.HasValue ? model.Paging.Value : 0;
                if (model.Paging == 1)
                {
                    if (model.PageIndex == null)
                    {
                        model.PageIndex = 1;
                    }
                    if (model.PageSize == null)
                    {
                        model.PageSize = 10;
                    }
                    allPage = (int)(allCount / model.PageSize);
                    if (allCount % model.PageSize > 0)
                    {
                        allPage = allPage + 1;
                    }
                    list = list.Skip(((int)model.PageIndex - 1) * (int)model.PageSize).Take((int)model.PageSize).ToList();

                }

                //获取行数
                ArrayList array = new ArrayList();
                foreach (var item in list)
                {
                    var ProgramCount = dbContext.ProgramToGroup.Where(i => i.GroupCode == item.Code).Count();
                    array.Add(new { item.ID, item.Code, item.GroupName, item.ScreenInfo, item.ScreenInfoCode, ProgramCount });
                    //item.DeviceCount =  dbContext.DeviceToGroups.Where(i => i.GroupID == item.ID).Count();
                }

                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { List = array, AllPage = allPage, AllCount = allCount };



                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "节目模块", LogMsg = username + "查询节目组列表", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
                //dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "Erro:" + e.ToString();
                _Result.Data = "";

            }
            return Json(_Result);
        }



        /// <summary>
        /// 返回节目组列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetProgGroupsByPage(Input_ProgramGroupQueryNew model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (Input_ProgramGroupQueryNew)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

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

                SqlParameter[] parameters = {
                    new SqlParameter("@ScreenCode", SqlDbType.VarChar,50),//自定义参数  与参数类型    
                     new SqlParameter("@MallCode", SqlDbType.VarChar,50)

                                               };

                parameters[0].Value = model.ScreenCode;  //给参数赋值
                parameters[1].Value = model.MallCode;  //给参数赋值

                var list = await dbContext.Output_ProgramGroupQuery.FromSql(@"select 
                                                                                     a.ID
                                                                                     ,a.Code
                                                                                     ,a.GroupName
                                                                                     ,a.ScreenInfoCode
                                                                                     ,a.AddTime
                                                                                     ,b.SName as ScreenInfo
                                                                                     ,count(distinct(c.Code)) as ProgramCount
                                                                                     from ProgramGroup a 
                                                                                     left join ScreenInfo b on a.ScreenInfoCode = b.Code
                                                                                     left join ProgramToGroup c on a.Code = c.GroupCode
                                                                                     where 
                                                                                     (@ScreenCode = '' or a.ScreenInfoCode = @ScreenCode) and a.MallCode = @MallCode
                                                                                     group by 
                                                                                     a.ID
                                                                                     ,a.Code
                                                                                     ,a.GroupName
                                                                                     ,a.ScreenInfoCode
                                                                                     ,a.AddTime
                                                                                     ,b.SName 
                                                                                     order by ProgramCount desc ", parameters).ToListAsync();

                list = list.Where(i => (string.IsNullOrEmpty(model.SearchKey) || i.GroupName.Contains(model.SearchKey))).ToList();

                if (model.Order.ToUpper() == "DESC")
                {
                    list = list.OrderByDescending(o => o.ProgramCount).ToList();
                }

                if (model.Order.ToUpper() == "ASC")
                {
                    list = list.OrderBy(o => o.ProgramCount).ToList();
                }
                ////查询数据
                //var list = await dbContext.ProgramGroup.Join(dbContext.ScreenInfo, pg => pg.ScreenInfoCode, si => si.Code, (pg, si) => new {
                //    pg.ID,
                //    pg.Code,
                //    pg.GroupName,
                //    ScreenInfo = si.SName,
                //    pg.ScreenInfoCode,
                //    pg.AddTime
                //}).OrderByDescending(o => o.AddTime).ToListAsync();

                ////过滤数据
                //if (!string.IsNullOrEmpty(model.ScreenCode))
                //{
                //    list = list.Where(i => i.ScreenInfoCode == model.ScreenCode).ToList();
                //}

                int allPage = 1;
                var allCount = list.Count();
                model.Paging = model.Paging.HasValue ? model.Paging.Value : 0;
                if (model.Paging == 1)
                {
                    if (model.PageIndex == null)
                    {
                        model.PageIndex = 1;
                    }
                    if (model.PageSize == null)
                    {
                        model.PageSize = 10;
                    }
                    allPage = (int)(allCount / model.PageSize);
                    if (allCount % model.PageSize > 0)
                    {
                        allPage = allPage + 1;
                    }
                    list = list.Skip(((int)model.PageIndex - 1) * (int)model.PageSize).Take((int)model.PageSize).ToList();

                }

                ////获取行数
                //ArrayList array = new ArrayList();
                //foreach (var item in list)
                //{
                //    var ProgramCount = dbContext.ProgramToGroup.Where(i => i.GroupCode == item.Code).Count();
                //    array.Add(new { item.ID, item.Code, item.GroupName, item.ScreenInfo, item.ScreenInfoCode, ProgramCount });
                //    //item.DeviceCount =  dbContext.DeviceToGroups.Where(i => i.GroupID == item.ID).Count();
                //}

                //_Result.Code = "200";
                //_Result.Msg = "获取成功";
                //_Result.Data = new { List = array, AllPage = allPage, AllCount = allCount };

                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { List = list, AllPage = allPage, AllCount = allCount };

                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "节目模块", LogMsg = username + "查询节目组列表", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
                //dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "Erro:" + e.ToString();
                _Result.Data = "";

            }
            return Json(_Result);
        }


        /// <summary>
        /// 添加一个节目组
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddProgramGroup(Input_ProgramGroup model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_ProgramGroup)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

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
            var mall = await dbContext.Mall.Where(i => i.Code == model.MallCode).FirstOrDefaultAsync();

            if (mall == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的商场编码";
                _Result.Data = "";
                return Json(_Result);
            }



            if (string.IsNullOrEmpty(model.GroupName))
            {
                _Result.Code = "510";
                _Result.Msg = "名称不可为空";
                _Result.Data = "";
                return Json(_Result);
            }


            int sCount = dbContext.ScreenInfo.Where(i => i.Code == model.ScreenInfoCode).Count();
            if (sCount <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "未知的屏幕类型";
                _Result.Data = "";
                return Json(_Result);
            }


            //判断节目组是否已存在
            int count = dbContext.ProgramGroup.Where(i => i.MallCode == model.MallCode && i.GroupName == model.GroupName).Count();
            if (count > 0)
            {
                _Result.Code = "1";
                _Result.Msg = "节目组已存在";
                _Result.Data = "";
                return Json(_Result);
            }


            //检查组中的节目

            if (model.Programs.Count() > 0)
            {



                //判断ID是否都为有效节目
                foreach (var item in model.Programs)
                {
                    var q = await dbContext.Programs.Where(i => i.Code == item).CountAsync();
                    if (q <= 0)
                    {
                        _Result.Code = "3";
                        _Result.Msg = "Erro:没有编码为：" + item + "的节目";
                        _Result.Data = "";
                        return Json(_Result);
                    }

                    var pro = await dbContext.Programs.Where(i => i.Code == item).FirstOrDefaultAsync();

                    if (pro.ProgScreenInfo != model.ScreenInfoCode)
                    {
                        _Result.Code = "3";
                        _Result.Msg = "节目：" + pro.ProgramName + "的分辨率与节目组分辨率不一致";
                        _Result.Data = "";
                        return Json(_Result);
                    }
                }
            }



            //添加节目组


            ProgramGroup pg = new ProgramGroup();
            pg.MallCode = model.MallCode;
            pg.AddTime = DateTime.Now;
            pg.GroupName = model.GroupName;
            pg.ScreenInfoCode = model.ScreenInfoCode;
            pg.Code = Guid.NewGuid().ToString();
            pg.UpdateTime = DateTime.Now;
            dbContext.ProgramGroup.Add(pg);

            if (await dbContext.SaveChangesAsync() > 0)
            {
                //添加节目到节目组
                var ts = 0;
                if (model.Programs.Count > 0)
                {
                    int index = 1;
                    foreach (var item in model.Programs)
                    {
                        var prog = await dbContext.Programs.Where(i => i.Code == item).FirstOrDefaultAsync();

                        if (prog.LaunchTime <= DateTime.Now && DateTime.Now <= prog.ExpiryDate)
                        {
                            ts += prog.SwitchTime;
                        }

                        dbContext.ProgramToGroup.Add(new ProgramToGroup { AddTime = DateTime.Now, ProgramCode = item, GroupCode = pg.Code, Order = index, Code = Guid.NewGuid().ToString(), UpdateTime = DateTime.Now });
                        index++;
                    }

                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        _Result.Code = "200";
                        _Result.Msg = "添加成功";
                        _Result.Data = "";

                        //var ip = Method.GetUserIp(this.HttpContext);
                        //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "节目组模块", LogMsg = username + "创建了节目组：" + model.GroupName, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "创建", IP = ip });

                        //dbContext.SaveChanges();
                    }
                    else
                    {
                        _Result.Code = "4";
                        _Result.Msg = "添加节目到节目组失败";
                        _Result.Data = "";
                    }
                }
                else
                {
                    _Result.Code = "200";
                    _Result.Msg = "添加成功";
                    _Result.Data = "";

                    //var ip = Method.GetUserIp(this.HttpContext);
                    //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "节目组模块", LogMsg = username + "创建了节目组：" + model.GroupName, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "创建", IP = ip });

                    //dbContext.SaveChanges();
                }

                /*
                //添加节目组同步策略
                dbContext.DevSyn.Add(new DevSyn
                {
                    AddTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    Code = Guid.NewGuid().ToString(),
                    EffectTime = DateTime.Now.Date,
                    LastAcceptTime = DateTime.Now.Date,
                    ProgramGroupCode = pg.Code,
                    TimeSpan = ts
                });
                */
            }
            else
            {
                _Result.Code = "5";
                _Result.Msg = "添加节目组失败";
                _Result.Data = "";
            }
            return Json(_Result);

        }



        /// <summary>
        /// 删除节目组
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DelProgramGroup(Input_PGDel model, [FromServices] ContextString dbContext)
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
            model = (Input_PGDel)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            //判断字符串是否合法
            if (model.Codes.Count() <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "未能检测到编码";
                _Result.Data = "";
                return Json(_Result);
            }





            //判断ID是否都为有效节目组
            foreach (var item in model.Codes)
            {
                var q = await dbContext.ProgramGroup.Where(i => i.Code == item).CountAsync();
                if (q <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "Erro:没有编码为：" + item + "的节目组";
                    _Result.Data = "";
                    return Json(_Result);
                }
            }
            var pgname = string.Empty;
            foreach (var item in model.Codes)
            {
                //删除节目组到设备组
                dbContext.ProgramDevice.RemoveRange(dbContext.ProgramDevice.Where(i => i.ProgramGrounpCode == item));

                //删除节目到节目组关系
                dbContext.ProgramToGroup.RemoveRange(dbContext.ProgramToGroup.Where(i => i.GroupCode == item));


                //删除节目组
                var pg = dbContext.ProgramGroup.Where(i => i.Code == item).SingleOrDefault();
                pgname += pg.GroupName + ",";
                dbContext.ProgramGroup.Remove(pg);

            }

            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "删除成功";
                _Result.Data = "";

                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "节目组模块", LogMsg = username + "删除了名称为：" + pgname.TrimEnd(',') + "的节目组", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "删除", IP = ip });

                //  dbContext.SaveChanges();

            }
            else
            {
                _Result.Code = "200";
                _Result.Msg = "删除失败";
                _Result.Data = "";
            }
            return Json(_Result);

        }




        /// <summary>
        /// 更新节目组信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateGroupInfo(Input_ProgramGroupInfo model, [FromServices] ContextString dbContext)
        {



            QianMuResult _Result = new QianMuResult();

            try
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



                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (Input_ProgramGroupInfo)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());






                //判断节目组是否有效
                var grounps = await dbContext.ProgramGroup.Where(i => i.Code == model.Code).ToListAsync();

                //更新信息
                if (grounps.Count > 0)
                {
                    var group = grounps.FirstOrDefault();


                    //检查组中的节目

                    if (model.Programs.Count() > 0)
                    {



                        //判断ID是否都为有效节目
                        foreach (var item in model.Programs)
                        {
                            var q = await dbContext.Programs.Where(i => i.Code == item).CountAsync();
                            if (q <= 0)
                            {
                                _Result.Code = "2";
                                _Result.Msg = "Erro:没有编码为：" + item + "的节目";
                                _Result.Data = "";
                                return Json(_Result);
                            }


                            var pro = await dbContext.Programs.Where(i => i.Code == item).FirstOrDefaultAsync();

                            if (pro.ProgScreenInfo != group.ScreenInfoCode)
                            {
                                _Result.Code = "3";
                                _Result.Msg = "节目：" + pro.ProgramName + "的分辨率与节目组分辨率不一致";
                                _Result.Data = "";
                                return Json(_Result);
                            }




                        }
                    }

                    //获取节目组 到节目关系列表
                    var groupprograms = await dbContext.ProgramToGroup.Where(i => i.GroupCode == model.Code).ToListAsync();

                    //将原有的全部删除
                    dbContext.ProgramToGroup.RemoveRange(groupprograms);

                    List<ProgramToGroup> _NewPTG = new List<ProgramToGroup>();//待添加的节目

                    //将新的重新添加
                    int index = 1;
                    foreach (var program in model.Programs)
                    {
                        _NewPTG.Add(new ProgramToGroup { AddTime = DateTime.Now, ProgramCode = program, GroupCode = group.Code, Order = index, Code = Guid.NewGuid().ToString(), UpdateTime = DateTime.Now });
                        index++;
                    }


                    //修改设备组名称
                    group.GroupName = model.GroupName;
                    dbContext.ProgramGroup.Update(group);
                    //添加新的节目
                    dbContext.ProgramToGroup.AddRange(_NewPTG);

                    //操作成功
                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        _Result.Code = "200";
                        _Result.Msg = "更新节目组信息成功";
                        _Result.Data = "";



                        //var ip = Method.GetUserIp(this.HttpContext);
                        //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "节目组模块", LogMsg = username + "更新节目组信息,访问数据：" + JsonConvert.SerializeObject(model).ToString(), AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });

                        // dbContext.SaveChanges();

                    }
                    else
                    {
                        _Result.Code = "3";
                        _Result.Msg = "更新节目组信息失败";
                        _Result.Data = "";
                    }


                }
                else
                {
                    _Result.Code = "4";
                    _Result.Msg = "无效的节目组";
                    _Result.Data = "";

                }
            }
            catch (Exception e)
            {
                QMLog qMLog = new QMLog();
                qMLog.WriteLogToFile("UpdateGroupInfo", e.ToString());
                throw;
            }
            //返回操作结果
            return Json(_Result);

        }





    }
}