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

namespace FrontCenter.Controllers.Information
{
    public class ShopFormatController : Controller
    {



        /// <summary>
        /// 添加业态
        /// </summary>
        /// <param name="ConstructionInfo"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ShopFormatAdd(Input_ShopFormatAdd modelsf, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            modelsf = (Input_ShopFormatAdd)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, modelsf.GetType());

            var model = modelsf.Parameter;

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

            if (string.IsNullOrEmpty(model.Name))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入业态名称";
                _Result.Data = "";
                return Json(_Result);
            }


            var mall = await dbContext.Mall.Where(i => i.Code == model.MallCode).FirstOrDefaultAsync();
            if (mall == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的商场编码：" + model.MallCode;
                _Result.Data = "";
                return Json(_Result);

            }


            var sf = await dbContext.ShopFormat.Where(i => (i.Name == model.Name && i.IsDel == false && string.IsNullOrEmpty(i.ParentCode) && i.MallCode == model.MallCode)).FirstOrDefaultAsync();

            if (sf != null)
            {
                _Result.Code = "510";
                _Result.Msg = "业态名称已存在";
                _Result.Data = "";
                return Json(_Result);
            }
            foreach (var item in model.Child)
            {
                if (model.Child.Where(i => i.Name == item.Name).Count() > 1)
                {
                    _Result.Code = "510";
                    _Result.Msg = "Erro:子业态重复";
                    _Result.Data = "";
                    return Json(_Result);
                }
            }
            if (model.IconFile == null)
            {
                model.IconFile = string.Empty;
            }
            ShopFormat psf = new ShopFormat { IsDel = false, Color = model.Color, Name = model.Name, NameEn = model.NameEn, ParentCode = string.Empty, IconFile = model.IconFile, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), UpdateTime = DateTime.Now, MallCode = model.MallCode };
            dbContext.ShopFormat.Add(psf);
            var changecount = await dbContext.SaveChangesAsync();

            var ip = Method.GetUserIp(this.HttpContext);
            dbContext.SysLog.Add(new SysLog { AccountName = model.UserName, ModuleName = "业态管理", LogMsg = model.UserName + "添加了业态：" + model.Name + "及其子业态", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "创建", IP = ip });
            dbContext.SaveChanges();

            foreach (var child in model.Child)
            {
                if (child.IconFile == null)
                {
                    child.IconFile = string.Empty;
                }
                dbContext.ShopFormat.Add(new ShopFormat { IsDel = false, Color = model.Color, Name = child.Name, NameEn = child.NameEn, ParentCode = psf.Code, IconFile = child.IconFile, AddTime = DateTime.Now, UpdateTime = DateTime.Now, Code = Guid.NewGuid().ToString(), MallCode = model.MallCode });
            }

            changecount += await dbContext.SaveChangesAsync();
            if (changecount > 0)
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
        /// 删除业态
        /// </summary>
        /// <param name="ConstructionInfo"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ShopFormatDel(Input_ShopFormatDel model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_ShopFormatDel)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

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


            //判断字符串是否合法
            if (model.Codes.Count <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "未能检测到codes";
                _Result.Data = "";
                return Json(_Result);
            }



            List<ShopFormat> _sfList = new List<ShopFormat>();
            var snames = string.Empty;
            foreach (var item in model.Codes)
            {



                var sf = await dbContext.ShopFormat.Where(i => (i.Code == item || i.ParentCode == item) && !i.IsDel).ToListAsync();

                foreach (var s in sf)
                {
                    var shopnum = dbContext.Shops.Where(i => (i.ShopFormat == s.Code || i.SecFormat == s.Code) && !i.IsDel).Count();
                    if (shopnum > 0)
                    {
                        _Result.Code = "510";
                        _Result.Msg = "Erro:" + s.Name + "业态正被使用中不可删除";
                        _Result.Data = "";
                        return Json(_Result);
                    }
                    snames += s.Name + ",";
                }

                _sfList.AddRange(sf);

            }

            foreach (var item in _sfList)
            {
                item.IsDel = true;
                item.UpdateTime = DateTime.Now;
                //移除图标文件
                await FileHelper.DelFile(item.IconFile, dbContext);
            }

            dbContext.ShopFormat.UpdateRange(_sfList);

            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "删除成功";
                _Result.Data = "";

                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = model.UserName, ModuleName = "业态管理", LogMsg = model.UserName + "删除业态：" + snames.TrimEnd(','), AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "删除", IP = ip });
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
        /// 获取业态列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetShopFormatList(string mallCode, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            if (string.IsNullOrEmpty(mallCode))
            {
                ////检测用户登录情况
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

            var sfs = await dbContext.ShopFormat.Where(i => i.ParentCode == string.Empty && i.MallCode == mallCode && !i.IsDel).OrderBy(o => o.AddTime).AsNoTracking().ToListAsync();
            ArrayList list = new ArrayList();
            foreach (var sf in sfs)
            {
                var count = await dbContext.ShopFormat.Where(i => i.ParentCode == sf.Code && !i.IsDel).CountAsync();
                var iconFile = await dbContext.AssetFiles.Where(i => i.Code == sf.IconFile).FirstOrDefaultAsync();
                string IconFilePath = "";
                if (iconFile != null)
                {
                    //IconFilePath = Method.ServerAddr + "/MallSite/" + iconFile.FilePath;
                    IconFilePath = Method.OSSServer + iconFile.FilePath;
                }

                list.Add(new { sf.ID, sf.Name, sf.NameEn, ChildCount = count, sf.AddTime, sf.Color, IconFilePath, sf.Code });
            }
            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = list;


            //var ip = Method.GetUserIp(this.HttpContext);
            //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "商铺模块", LogMsg = username + "获取业态列表", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
            //dbContext.SaveChanges();

            return Json(_Result);

        }




        /// <summary>
        /// 编辑业态
        /// </summary>
        /// <param name="ConstructionInfo"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ShopFormatEdit(Input_ShopFormatEdit modele, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            modele = (Input_ShopFormatEdit)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, modele.GetType());

            var model = modele.Parameter;

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
            if (string.IsNullOrEmpty(model.Code) || string.IsNullOrEmpty(model.Name))
            {
                _Result.Code = "510";
                _Result.Msg = "输入信息不完整";
                _Result.Data = "";
                return Json(_Result);
            }




            var sf = await dbContext.ShopFormat.Where(i => i.Code == model.Code && !i.IsDel).FirstOrDefaultAsync();

            if (sf == null)
            {
                _Result.Code = "510";
                _Result.Msg = "业态不存在";
                _Result.Data = "";
                return Json(_Result);
            }

            sf.Name = model.Name;
            sf.NameEn = model.NameEn;
            sf.Color = model.Color;
            // sf.AddTime = DateTime.Now;
            sf.UpdateTime = DateTime.Now;

            var oldicon = String.Empty;

            if (model.IconFile != null)
            {
                if (sf.IconFile != model.IconFile)
                {
                    oldicon = sf.IconFile;
                }
                sf.IconFile = model.IconFile;
            }
            sf.UpdateTime = DateTime.Now;

            //判断子业态是否存在
            var childsfm = await dbContext.ShopFormat.Where(i => i.ParentCode == sf.Code && !i.IsDel).ToListAsync();
            foreach (var item in model.Child)
            {
                if (model.Child.Where(i => i.Name == item.Name).Count() > 1)
                {
                    _Result.Code = "510";
                    _Result.Msg = "Erro:子业态重复";
                    _Result.Data = "";
                    return Json(_Result);
                }
                var childsf = childsfm.Where(i => i.Name == item.Name).FirstOrDefault();
                //不存在
                if (childsf == null)
                {
                    if (item.IconFile == null)
                    {
                        item.IconFile = string.Empty;
                    }
                    //添加
                    dbContext.ShopFormat.Add(new ShopFormat { IsDel = false, MallCode = sf.MallCode, AddTime = DateTime.Now, Color = model.Color, Name = item.Name, NameEn = item.NameEn, ParentCode = model.Code, IconFile = item.IconFile, UpdateTime = DateTime.Now, Code = Guid.NewGuid().ToString() });
                }
                else
                {
                    //存在 更新
                    childsf.Color = model.Color;
                    childsf.NameEn = item.NameEn;
                    // childsf.AddTime = DateTime.Now;
                    childsf.UpdateTime = DateTime.Now;
                }
            }
            //判断子业态是否被删除
            foreach (var item in childsfm)
            {
                var childsf = model.Child.Where(i => i.Name == item.Name).FirstOrDefault();
                //已被删除
                if (childsf == null)
                {
                    //判断 被移除的子业态是否被使用
                    var shopnum = dbContext.Shops.Where(i => i.ShopFormat == item.Code || i.SecFormat == item.Code).Count();
                    if (shopnum > 0)
                    {
                        _Result.Code = "510";
                        _Result.Msg = "Erro:" + item.Name + "业态正被使用中不可删除";
                        _Result.Data = "";
                        return Json(_Result);
                    }

                    item.IsDel = true;
                    item.UpdateTime = DateTime.Now;
                    dbContext.ShopFormat.Update(item);
                }

            }

            dbContext.ShopFormat.Update(sf);

            if (await dbContext.SaveChangesAsync() > 0)
            {
                //更换了图标  删除旧图标文件
                if (!string.IsNullOrEmpty(oldicon))
                {
                    await FileHelper.DelFile(oldicon, dbContext);
                }
                _Result.Code = "200";
                _Result.Msg = "修改成功";
                _Result.Data = "";


                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = model.UserName, ModuleName = "业态管理", LogMsg = model.UserName + "编辑业态", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });
                dbContext.SaveChanges();
            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "修改失败";
                _Result.Data = "";
            }

            return Json(_Result);
        }

        /// <summary>
        /// 获取业态信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetShopFormatInfo(string code, [FromServices] ContextString dbContext)
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
                _Result.Msg = "未知的ID";
                _Result.Data = "";

                return Json(_Result);
            }

            var sf = await dbContext.ShopFormat.Where(i => i.Code == code && !i.IsDel).Select(s => new {
                s.ID,
                s.Code,
                s.Name,
                s.NameEn,
                s.Color,
                AddTime = s.AddTime.ToString("yyyy-MM-dd"),
                s.IconFile
            }).FirstOrDefaultAsync();
            if (sf != null)
            {
                var child = await dbContext.ShopFormat.Where(i => i.ParentCode == sf.Code && !i.IsDel).Select(s => new
                {
                    s.ID,
                    s.Code,
                    s.Name,
                    s.NameEn
                }).ToListAsync();


                var iconFile = await dbContext.AssetFiles.Where(i => i.Code == sf.IconFile).FirstOrDefaultAsync();
                string IconFilePath = "";
                if (iconFile != null)
                {
                    //IconFilePath = Method.ServerAddr + "/MallSite/" + iconFile.FilePath;
                    IconFilePath = Method.OSSServer + iconFile.FilePath;
                }

                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { sf.ID, sf.Code, sf.Name, sf.NameEn, sf.Color, child, sf.IconFile, IconFilePath };

                //var ip = Method.GetUserIp(this.HttpContext);
                //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "商铺模块", LogMsg = username + "获取ID为："+ id + "的业态信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
                //dbContext.SaveChanges();

            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "未能找到指定业态";
                _Result.Data = "";
            }


            return Json(_Result);
        }



        /// <summary>
        /// 获取业态信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryShopFormatList(string mallCode, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            if (string.IsNullOrEmpty(mallCode))
            {
                ////检测用户登录情况
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

            var sflist = await dbContext.ShopFormat.Where(i => i.MallCode == mallCode && !i.IsDel).Select(s => new
            {
                s.ID,
                s.Code,
                s.Name,
                s.NameEn,
                s.Color,
                s.IconFile,
                AddTime = s.AddTime.ToString("yyyy-MM-dd")
            }).ToListAsync();
            ArrayList arrayList = new ArrayList();
            foreach (var sf in sflist)
            {
                var child = await dbContext.ShopFormat.Where(i => i.ParentCode == sf.Code && !i.IsDel).Select(s => new
                {
                    s.ID,
                    s.Code,
                    s.Name,
                    s.NameEn,
                    s.Color,
                    AddTime = s.AddTime.ToString("yyyy-MM-dd")
                }).ToListAsync();

                var iconFile = await dbContext.AssetFiles.Where(i => i.Code == sf.IconFile).FirstOrDefaultAsync();
                string IconFilePath = "";
                if (iconFile != null)
                {
                    IconFilePath = iconFile.FilePath;
                }

                arrayList.Add(new { sf.ID, sf.Code, sf.Name, sf.Color, sf.NameEn, IconFilePath, sf.IconFile, child });


            }



            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = arrayList;



            return Json(_Result);
        }




    }
}