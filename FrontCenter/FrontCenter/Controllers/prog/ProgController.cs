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
    public class ProgController : Controller
    {
        /// <summary>
        /// 添加节目
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(Input_Prog model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_Prog)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            UserOnLine uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (uol == null || string.IsNullOrEmpty(uol.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }
            String MallCode = model.MallCode;
            if (string.IsNullOrEmpty(MallCode))
            {
                MallCode = uol.MallCode;
            }
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

                var mall = await dbContext.Mall.Where(i => i.Code == MallCode).FirstOrDefaultAsync();

                if (mall == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的商场编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var LaunchTime = Convert.ToDateTime(model.LaunchTime);
                var ExpiryDate = Convert.ToDateTime(model.ExpiryDate);

                var scount = await dbContext.ScreenInfo.Where(i => i.Code == model.ScreenInfo).CountAsync();
                if (scount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的屏幕类型";
                    _Result.Data = "";
                    return Json(_Result);
                }


                if (model.ProgFiles.Count() <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "素材数量不应为空";
                    _Result.Data = "";
                    return Json(_Result);
                }
                foreach (var pf in model.ProgFiles)
                {

                    Programs p = new Programs();

                    if (string.IsNullOrEmpty(pf.ProgramName))
                    {
                        _Result.Code = "510";
                        _Result.Msg = "节目名称不可为空";
                        _Result.Data = "";
                        return Json(_Result);
                    }

                    var file = await dbContext.AssetFiles.Where(i => i.FileGUID == pf.FileGUID).FirstOrDefaultAsync();
                    if (file == null)
                    {

                        _Result.Code = "510";
                        _Result.Msg = "无效的素材编码：" + pf.FileGUID;
                        _Result.Data = "";
                        return Json(_Result);
                    }

                    if (string.IsNullOrEmpty(pf.PreviewFileGUID))
                    {
                        pf.PreviewFileGUID = pf.FileGUID;
                        p.PreviewSrc = file.FilePath;
                    }
                    else
                    {
                        var pfile = await dbContext.AssetFiles.Where(i => i.FileGUID == pf.PreviewFileGUID).FirstOrDefaultAsync();
                        if (pfile == null)
                        {

                            _Result.Code = "510";
                            _Result.Msg = "无效的素材编码：" + pf.PreviewFileGUID;
                            _Result.Data = "";
                            return Json(_Result);
                        }
                        p.PreviewSrc = pfile.FilePath;
                    }
                    p.MallCode = MallCode;

                    p.ExpiryDate = ExpiryDate;
                    p.LaunchTime = LaunchTime;
                    p.ProgScreenInfo = model.ScreenInfo;
                    p.ScreenMatch = model.ScreenMatch;
                    p.SwitchMode = model.SwitchMode;
                    p.SwitchTime = model.SwitchTime;
                    p.ProgramName = pf.ProgramName;
                    p.AddTime = DateTime.Now;

                    p.ProgSrc = file.FilePath;
                    p.ProgType = file.FileType;

                    p.Code = Guid.NewGuid().ToString();
                    p.UpdateTime = DateTime.Now;

                    p.FileGuid = pf.FileGUID;
                    p.PreviewFileGuid = pf.PreviewFileGUID;


                    dbContext.Programs.Add(p);

                }


                if (await dbContext.SaveChangesAsync() >= 0)
                {
                    _Result.Code = "200";
                    _Result.Msg = "添加成功";
                    _Result.Data = "";
                }


            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";

            }
            return Json(_Result);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Input_EditProg model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_EditProg)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            try
            {

                //检测用户登录情况
                UserOnLine uol = Method.GetLoginUserName(dbContext, this.HttpContext);
                if (uol == null || string.IsNullOrEmpty(uol.UserName))
                {
                    _Result.Code = "401";
                    _Result.Msg = "请登陆后再进行操作";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var LaunchTime = Convert.ToDateTime(model.LaunchTime);
                var ExpiryDate = Convert.ToDateTime(model.ExpiryDate);

                var scount = await dbContext.ScreenInfo.Where(i => i.Code == model.ScreenInfo).CountAsync();
                if (scount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的屏幕类型";
                    _Result.Data = "";
                    return Json(_Result);
                }


                var p = await dbContext.Programs.Where(i => i.Code == model.Code).FirstOrDefaultAsync();
                if (p == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的节目编码：" + model.Code;
                    _Result.Data = "";
                    return Json(_Result);
                }



                if (string.IsNullOrEmpty(model.ProgramName))
                {
                    _Result.Code = "510";
                    _Result.Msg = "节目名称不可为空";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var file = await dbContext.AssetFiles.Where(i => i.FileGUID == model.FileGUID).FirstOrDefaultAsync();
                if (file == null)
                {

                    _Result.Code = "510";
                    _Result.Msg = "无效的素材编码：" + model.FileGUID;
                    _Result.Data = "";
                    return Json(_Result);
                }

                if (string.IsNullOrEmpty(model.PreviewFileGUID))
                {
                    model.PreviewFileGUID = model.FileGUID;
                    p.PreviewSrc = file.FilePath;
                }
                else
                {
                    var pfile = await dbContext.AssetFiles.Where(i => i.FileGUID == model.PreviewFileGUID).FirstOrDefaultAsync();
                    if (pfile == null)
                    {

                        _Result.Code = "510";
                        _Result.Msg = "无效的素材编码：" + model.PreviewFileGUID;
                        _Result.Data = "";
                        return Json(_Result);
                    }
                    p.PreviewSrc = pfile.FilePath;
                }


                p.ExpiryDate = ExpiryDate;
                p.LaunchTime = LaunchTime;
                p.ProgScreenInfo = model.ScreenInfo;
                p.ScreenMatch = model.ScreenMatch;
                p.SwitchMode = model.SwitchMode;
                p.SwitchTime = model.SwitchTime;
                p.ProgramName = model.ProgramName;


                p.ProgSrc = file.FilePath;
                p.ProgType = file.FileType;
                p.UpdateTime = DateTime.Now;

                p.FileGuid = model.FileGUID;
                p.PreviewFileGuid = model.PreviewFileGUID;


                dbContext.Programs.Update(p);




                if (await dbContext.SaveChangesAsync() >= 0)
                {
                    _Result.Code = "200";
                    _Result.Msg = "添加成功";
                    _Result.Data = "";
                }


            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";

            }
            return Json(_Result);
        }



        /// <summary>
        /// 获取节目列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetList(Input_GetProgList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetProgList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            UserOnLine uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (uol == null || string.IsNullOrEmpty(uol.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }
            String MallCode = model.MallCode;
            if (string.IsNullOrEmpty(MallCode))
            {
                MallCode = uol.MallCode;
            }
            try
            {

                ////检测用户登录情况
                //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
                //if (string.IsNullOrEmpty(username))
                //{
                //    _Result.Code = "401";
                //    _Result.Msg = "请登陆后再进行操作";
                //    _Result.Data = "";
                //    return Json(_Result);
                //}

                var mall = await dbContext.Mall.Where(i => i.Code == MallCode).FirstOrDefaultAsync();

                if (mall == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的商场编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var list = await dbContext.Programs.Where(i => i.MallCode == MallCode
                         && (string.IsNullOrEmpty(model.SearchKey) || i.ProgramName.Contains(model.SearchKey))
                         && (string.IsNullOrEmpty(model.ScreenCode) || i.ProgScreenInfo == model.ScreenCode)
                         && (string.IsNullOrEmpty(model.ProgType) || i.ProgType == model.ProgType)
                         && (string.IsNullOrEmpty(model.SwitchMode) || i.SwitchMode == model.SwitchMode)
                         && (string.IsNullOrEmpty(model.ScreenMatch) || i.ScreenMatch == model.ScreenMatch)
                ).Join(dbContext.ScreenInfo, p => p.ProgScreenInfo, si => si.Code, (p, si) => new
                {
                    p.Code,
                    p.FileGuid,
                    p.PreviewFileGuid,
                    PreviewSrc = p.PreviewSrc,
                    ProgSrc = p.ProgSrc,
                    p.ProgramName,
                    p.ProgType,
                    p.ProgScreenInfo,
                    si.SName,
                    LaunchTime = p.LaunchTime.ToString("yyyy-MM-dd"),
                    ExpiryDate = p.ExpiryDate.ToString("yyyy-MM-dd"),
                    p.SwitchMode,
                    p.SwitchTime,
                    p.ScreenMatch,
                    p.AddTime
                }).Join(dbContext.AssetFiles, p => p.FileGuid, af => af.FileGUID, (p, af) => new {
                    p.Code,
                    p.FileGuid,
                    p.PreviewFileGuid,
                    p.PreviewSrc,
                    p.ProgSrc,
                    p.ProgramName,
                    p.ProgType,
                    p.ProgScreenInfo,
                    p.SName,
                    p.LaunchTime,
                    p.ExpiryDate,
                    p.SwitchMode,
                    p.SwitchTime,
                    p.ScreenMatch,
                    p.AddTime,
                    af.FileSize,
                    af.Height,
                    af.Width,
                    af.Duration
                }).OrderByDescending(i => i.AddTime).ToListAsync();



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

                    ArrayList arrayList = new ArrayList();
                    foreach (var p in list)
                    {
                        var Status = string.Empty;
                        var _LaunchTime = Convert.ToDateTime(p.LaunchTime);
                        var _ExpiryDate = Convert.ToDateTime(p.ExpiryDate).AddDays(1);
                        if (DateTime.Now < _LaunchTime)
                        {
                            Status = "未开始";
                        }

                        if (_LaunchTime <= DateTime.Now && DateTime.Now < _ExpiryDate)
                        {
                            Status = "排期中";
                        }

                        if (_ExpiryDate <= DateTime.Now)
                        {
                            Status = "已过期";
                        }

                        arrayList.Add(new
                        {
                            p.Code,
                            p.FileGuid,
                            p.PreviewFileGuid,
                            PreviewSrc = Method.OSSServer + p.PreviewSrc,
                            ProgSrc = Method.OSSServer + p.ProgSrc,
                            p.ProgramName,
                            p.ProgType,
                            p.ProgScreenInfo,
                            p.SName,
                            p.LaunchTime,
                            p.ExpiryDate,
                            p.SwitchMode,
                            p.SwitchTime,
                            p.ScreenMatch,
                            p.Duration,
                            p.FileSize,
                            p.Height,
                            p.Width,
                            Status
                        });
                    }

                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = arrayList;
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



                    ArrayList arrayList = new ArrayList();
                    foreach (var p in list)
                    {
                        var Status = string.Empty;
                        var _LaunchTime = Convert.ToDateTime(p.LaunchTime);
                        var _ExpiryDate = Convert.ToDateTime(p.ExpiryDate).AddDays(1);
                        if (DateTime.Now < _LaunchTime)
                        {
                            Status = "未开始";
                        }

                        if (_LaunchTime <= DateTime.Now && DateTime.Now < _ExpiryDate)
                        {
                            Status = "排期中";
                        }

                        if (_ExpiryDate <= DateTime.Now)
                        {
                            Status = "已过期";
                        }

                        arrayList.Add(new
                        {
                            p.Code,
                            p.FileGuid,
                            p.PreviewFileGuid,
                            PreviewSrc = Method.OSSServer + p.PreviewSrc,
                            ProgSrc = Method.OSSServer + p.ProgSrc,
                            p.ProgramName,
                            p.ProgType,
                            p.ProgScreenInfo,
                            p.SName,
                            p.LaunchTime,
                            p.ExpiryDate,
                            p.SwitchMode,
                            p.SwitchTime,
                            p.ScreenMatch,
                            p.Duration,
                            p.FileSize,
                            p.Height,
                            p.Width,
                            Status
                        });
                    }


                    _Result.Code = "200";
                    _Result.Msg = "获取成功";
                    _Result.Data = new { List = arrayList, AllPage = allPage, AllCount = allCount };

                }





            }
            catch (Exception e)
            {


                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";

            }
            return Json(_Result);
        }



        /// <summary>
        /// 删除节目
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DelProgram(Input_ProgramDel model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //检测用户登录情况
            UserOnLine uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (uol == null || string.IsNullOrEmpty(uol.UserName))
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
            model = (Input_ProgramDel)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            //判断字符串是否合法
            if (model.Codes.Count() <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "未能检测到编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var pnames = string.Empty;
            //删除节目及节目组关系
            try
            {
                List<Programs> list = new List<Programs>();
                List<ProgramToGroup> _pgList = new List<ProgramToGroup>();
                foreach (var item in model.Codes)
                {

                    string ProgSrc = await dbContext.Programs.Where(i => i.Code == item).Select(s => s.ProgSrc).FirstOrDefaultAsync();
                    if (ProgSrc == null)
                    {
                        ProgSrc = string.Empty;
                    }
                    var path = string.Empty;
                    string[] pathArry = ProgSrc.Split('\\');
                    for (int i = 0; i < pathArry.Length - 1; i++)
                    {
                        path += @"\" + pathArry[i];
                    }
                    if (!string.IsNullOrEmpty(path))
                    {
                        path = Method._hostingEnvironment.WebRootPath + path;

                        //删除文件夹及其中的文件
                        if (Directory.Exists(path))
                        {
                            DirectoryInfo di = new DirectoryInfo(path);
                            di.Delete(true);

                        }
                    }

                    var pro = dbContext.Programs.Where(i => i.Code == item).FirstOrDefault();
                    pnames += pro.ProgramName + ",";
                    list.Add(pro);
                    _pgList.AddRange(dbContext.ProgramToGroup.Where(i => i.ProgramCode == item));



                }
                dbContext.Programs.RemoveRange(list);
                dbContext.ProgramToGroup.RemoveRange(_pgList);
                await dbContext.SaveChangesAsync();
                _Result.Code = "200";
                _Result.Msg = "删除成功";
                _Result.Data = "";


                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "节目模块", LogMsg = username + "删除名称为：" + pnames.TrimEnd(',') + "的节目", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "删除", IP = ip });

                // dbContext.SaveChanges();

                return Json(_Result);
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);
                throw;
            }

        }


        #region 获取有效节目列表方法
        /// <summary>
        /// 获取最近5天的节目
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private async Task<QianMuResult> GetEffectiveLastProgramList(string OrderBy, ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                DateTime dtime = DateTime.Now.AddDays(-5).Date;


                var list = await dbContext.Programs.Where(i => i.AddTime >= dtime && i.LaunchTime <= DateTime.Now && i.ExpiryDate >= DateTime.Now).Join(dbContext.ScreenInfo, pr => pr.ProgScreenInfo, si => si.Code, (pr, si) => new
                {

                    ID = pr.ID,
                    pr.Code,
                    PreviewSrc = pr.PreviewSrc == "" ? "" : Method.OSSServer + pr.PreviewSrc,
                    ProgramName = pr.ProgramName,
                    ProgType = pr.ProgType,
                    ProgScreenInfo = si.SName,
                    ScreenCode = si.Code,
                    LaunchTime = pr.LaunchTime.ToString("yyyy-MM-dd"),
                    ExpiryDate = pr.ExpiryDate.ToString("yyyy-MM-dd"),
                    SwitchMode = pr.SwitchMode,
                    SwitchTime = pr.SwitchTime,
                    ScreenMatch = pr.ScreenMatch,
                    AddTime = pr.AddTime
                }).Select(s => new {
                    s.ID,
                    s.Code,
                    s.PreviewSrc,
                    s.ProgramName,
                    s.ProgType,
                    s.ProgScreenInfo,
                    s.LaunchTime,
                    s.ExpiryDate,
                    s.SwitchMode,
                    s.SwitchTime,
                    s.ScreenMatch,
                    s.ScreenCode,
                    AddTime = s.AddTime
                }).ToListAsync();

                if (!string.IsNullOrEmpty(OrderBy))
                {
                    if (OrderBy.ToLower() == "timeasc")
                    {
                        list = list.OrderBy(o => o.AddTime).ToList();
                    }

                    if (OrderBy.ToLower() == "timedesc")
                    {
                        list = list.OrderByDescending(o => o.AddTime).ToList();
                    }

                }
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = list;
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "Erro:" + e.ToString();
                _Result.Data = "";

            }
            return _Result;
        }

        /// <summary>
        /// 获取所有节目
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private async Task<QianMuResult> GetEffectiveAllProgramList(string OrderBy, ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                var list = await dbContext.Programs.Where(i => i.LaunchTime <= DateTime.Now && i.ExpiryDate >= DateTime.Now).Join(dbContext.ScreenInfo, pr => pr.ProgScreenInfo, si => si.Code, (pr, si) => new
                {

                    ID = pr.ID,
                    pr.Code,
                    PreviewSrc = pr.PreviewSrc == "" ? "" : Method.OSSServer + pr.PreviewSrc,
                    ProgramName = pr.ProgramName,
                    ProgType = pr.ProgType,
                    ProgScreenInfo = si.SName,
                    ScreenCode = si.Code,
                    LaunchTime = pr.LaunchTime.ToString("yyyy-MM-dd "),
                    ExpiryDate = pr.ExpiryDate.ToString("yyyy-MM-dd "),
                    SwitchMode = pr.SwitchMode,
                    SwitchTime = pr.SwitchTime,
                    ScreenMatch = pr.ScreenMatch,
                    AddTime = pr.AddTime
                }).ToListAsync();


                if (!string.IsNullOrEmpty(OrderBy))
                {
                    if (OrderBy.ToLower() == "timeasc")
                    {
                        list = list.OrderBy(o => o.AddTime).ToList();
                    }

                    if (OrderBy.ToLower() == "timedesc")
                    {
                        list = list.OrderByDescending(o => o.AddTime).ToList();
                    }

                }

                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = list;
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "Erro:" + e.ToString();
                _Result.Data = "";

            }
            return _Result;
        }

        /// <summary>
        /// 获取指定类型的节目
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private async Task<QianMuResult> GetEffectiveProgramListByType(string ProgType, string OrderBy, ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                var list = await dbContext.Programs.Where(i => i.ProgType == ProgType && i.LaunchTime <= DateTime.Now && i.ExpiryDate >= DateTime.Now).Join(dbContext.ScreenInfo, pr => pr.ProgScreenInfo, si => si.Code, (pr, si) => new
                {

                    ID = pr.ID,
                    pr.Code,
                    PreviewSrc = pr.PreviewSrc == "" ? "" : Method.OSSServer + pr.PreviewSrc,
                    ProgramName = pr.ProgramName,
                    ProgType = pr.ProgType,
                    ProgScreenInfo = si.SName,
                    ScreenCode = si.Code,
                    LaunchTime = pr.LaunchTime.ToString("yyyy-MM-dd "),
                    ExpiryDate = pr.ExpiryDate.ToString("yyyy-MM-dd "),
                    SwitchMode = pr.SwitchMode,
                    SwitchTime = pr.SwitchTime,
                    ScreenMatch = pr.ScreenMatch,
                    AddTime = pr.AddTime
                }).ToListAsync();

                if (!string.IsNullOrEmpty(OrderBy))
                {
                    if (OrderBy.ToLower() == "timeasc")
                    {
                        list = list.OrderBy(o => o.AddTime).ToList();
                    }

                    if (OrderBy.ToLower() == "timedesc")
                    {
                        list = list.OrderByDescending(o => o.AddTime).ToList();
                    }

                }

                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = list;
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "Erro:" + e.ToString();
                _Result.Data = "";

            }
            return _Result;
        }



        /// <summary>
        /// 获取节目组中的节目
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private async Task<QianMuResult> GetEffectiveProgramListByScreen(string ScreenCode, string OrderBy, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            //判断输入条件
            if (string.IsNullOrEmpty(ScreenCode))
            {
                _Result.Code = "510";
                _Result.Msg = "未能检测到屏幕类型";
                _Result.Data = "";
                return _Result;
            }

            try
            {

                var list = await dbContext.Programs.Where(i => i.ProgScreenInfo == ScreenCode && i.LaunchTime <= DateTime.Now && i.ExpiryDate >= DateTime.Now).Join(dbContext.ScreenInfo, p => p.ProgScreenInfo, si => si.Code, (p, si) => new
                {
                    p.ID,
                    p.Code,
                    PreviewSrc = p.PreviewSrc == "" ? "" : Method.OSSServer + p.PreviewSrc,
                    ProgramName = p.ProgramName,
                    ProgType = p.ProgType,
                    ProgScreenInfo = si.SName,
                    ScreenCode = si.Code,
                    LaunchTime = p.LaunchTime.ToString("yyyy-MM-dd "),
                    ExpiryDate = p.ExpiryDate.ToString("yyyy-MM-dd "),
                    SwitchMode = p.SwitchMode,
                    SwitchTime = p.SwitchTime,
                    ScreenMatch = p.ScreenMatch,
                    AddTime = p.AddTime
                }).AsNoTracking().ToListAsync();


                if (!string.IsNullOrEmpty(OrderBy))
                {
                    if (OrderBy.ToLower() == "timeasc")
                    {
                        list = list.OrderBy(o => o.AddTime).ToList();
                    }

                    if (OrderBy.ToLower() == "timedesc")
                    {
                        list = list.OrderByDescending(o => o.AddTime).ToList();
                    }

                }

                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = list;
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "Erro:" + e.ToString();
                _Result.Data = "";

            }
            return _Result;
        }
        #endregion
        /// <summary>
        /// 获取节目组中的节目
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetProgramListByCode(Input_GetProgramListByCode model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetProgramListByCode)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            ////检测用户登录情况
            UserOnLine uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (uol == null || string.IsNullOrEmpty(uol.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }



            //判断输入条件
            if (string.IsNullOrEmpty(model.Code))
            {
                _Result.Code = "510";
                _Result.Msg = "未能检测到编码";
                _Result.Data = "";
                return Json(_Result);
            }

            try
            {
                DateTime dtime = DateTime.Now.AddDays(-5).Date;


                var list = await (from pg in dbContext.ProgramToGroup
                                  join pr in dbContext.Programs on pg.ProgramCode equals pr.Code
                                  join sc in dbContext.ScreenInfo on pr.ProgScreenInfo equals sc.Code into ProS
                                  from ps in ProS.DefaultIfEmpty()
                                  where pg.GroupCode == model.Code
                                  select new
                                  {

                                      ID = pr.ID,
                                      pr.Code,
                                      PreviewSrc = Method.OSSServer + pr.PreviewSrc,
                                      ProgramName = pr.ProgramName,
                                      ProgType = pr.ProgType,
                                      ProgScreenInfo = ps.SName,
                                      ScreenCode = ps.Code,
                                      LaunchTime = pr.LaunchTime.ToString("yyyy-MM-dd HH:mm:mm"),
                                      ExpiryDate = pr.ExpiryDate.ToString("yyyy-MM-dd HH:mm:mm"),
                                      SwitchMode = pr.SwitchMode,
                                      SwitchTime = pr.SwitchTime,
                                      ScreenMatch = pr.ScreenMatch,
                                      Order = pg.Order
                                  }).OrderBy(i => i.Order).ToListAsync();


                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = list;

                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "节目模块", LogMsg = username + "获取节目组："+ id+"中的节目", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
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
    }
}