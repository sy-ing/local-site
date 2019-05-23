using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public class ShopController : Controller
    {


        /// <summary>
        /// 添加店铺
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ShopAdd(Input_ShopAdd models, [FromServices] ContextString dbContext)
        {
            QMLog qMLog = new QMLog();
            QianMuResult _Result = new QianMuResult();
            try
            {
                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                models = (Input_ShopAdd)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, models.GetType());
                var model = models.Parameter;

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
                    _Result.Msg = "请输入店铺名称";
                    _Result.Data = "";
                    return Json(_Result);
                }


                //中文、英文、数字但不包括下划线等符号
                Regex reg = new Regex(@"^[\u4E00-\u9FA5A-Za-z0-9]+$");

                if (!reg.IsMatch(model.Name))
                {
                    _Result.Code = "510";
                    _Result.Msg = "角色名称不能有特殊字符";
                    _Result.Data = "";
                    return Json(_Result);
                }


                if (model.Logo == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "需要有效的店铺LOGO文件";
                    _Result.Data = "";
                    return Json(_Result);
                }
                if (string.IsNullOrEmpty(model.BuildingCode) || string.IsNullOrEmpty(model.FloorCode))
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入楼层楼栋信息";
                    _Result.Data = "";
                    return Json(_Result);
                }

                if (model.ShopFormat == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入业态信息";
                    _Result.Data = "";
                    return Json(_Result);
                }
                var logofile = await dbContext.AssetFiles.Where(i => i.FileGUID == model.Logo).FirstOrDefaultAsync();

                if (logofile == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "需要有效的店铺LOGO文件";
                    _Result.Data = "";
                    return Json(_Result);
                }
                FileTypeJudgment fimg = new FileTypeJudgment() { TypeTarget = new TypeImg() };
                if (!fimg.Judge(logofile.FileExtName))
                {
                    _Result.Code = "510";
                    _Result.Msg = "店铺LOGO文件不是图片";
                    _Result.Data = "";
                    return Json(_Result);
                }
                var building = await dbContext.Building.Where(i => i.Code == model.BuildingCode && !i.IsDel).FirstOrDefaultAsync();

                var floor = await dbContext.Floor.Where(i => i.Code == model.FloorCode && !i.IsDel).FirstOrDefaultAsync();

                if (building == null || floor == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的楼层楼栋编码";
                    _Result.Data = "";
                    return Json(_Result);
                }


                var sf = await dbContext.ShopFormat.Where(i => i.Code == model.ShopFormat && !i.IsDel).FirstOrDefaultAsync();
                // var sfse = await dbContext.ShopFormat.Where(i => i.ID == int.Parse(model.SecFormat)).FirstOrDefaultAsync();
                if (sf == null)
                {

                    _Result.Code = "510";
                    _Result.Msg = "无效的业态编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var mall = await dbContext.Mall.Where(i => i.Code == model.MallCode).FirstOrDefaultAsync();
                if (mall == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的商场编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                string spelling = NPinyin.Pinyin.GetPinyin(model.Name);
                string initials = NPinyin.Pinyin.GetInitials(model.Name);
                initials = Regex.Replace(initials, "[^a-z]", "", RegexOptions.IgnoreCase);
                if (string.IsNullOrEmpty(model.AreaCode))
                {
                    model.AreaCode = string.Empty;
                }
                var shop = new Shops
                {
                    AddTime = DateTime.Now,
                    BuildingCode = model.BuildingCode,
                    FloorCode = model.FloorCode,
                    HouseNum = model.HouseNum,
                    Intro = model.Intro,
                    IsShow = true,
                    Name = model.Name,
                    NameEn = model.NameEn,
                    Phone = model.Phone,
                    ShopFormat = model.ShopFormat,
                    SecFormat = model.SecFormat,
                    Logo = model.Logo,
                    Spelling = spelling,
                    Initials = initials,
                    AreaCode = model.AreaCode,
                    IntroEn = model.IntroEN,
                    Code = Guid.NewGuid().ToString(),
                    UpdateTime = DateTime.Now,
                    IsDel = false,
                    CloseTime = model.CloseTime,
                };
                var mallshop = new MallShop
                {
                    AddTime = DateTime.Now,
                    Code = Guid.NewGuid().ToString(),
                    MallCode = model.MallCode,
                    ShopCode = shop.Code,
                    UpdateTime = DateTime.Now
                };
                var aaa = Method.GetRandomStr(6, dbContext).ToString();
                var shopnum = new ShopNum
                {
                    AddTime = DateTime.Now,
                    Code = Guid.NewGuid().ToString(),
                    Num = aaa,
                    ShopCode = shop.Code,
                    UpdateTime = DateTime.Now
                };
                dbContext.Shops.Add(shop);
                dbContext.MallShop.Add(mallshop);
                dbContext.ShopNum.Add(shopnum);

                Brand brand = null;
                if (!string.IsNullOrEmpty(model.BrandCode))
                {
                    brand = await dbContext.Brand.Where(i => !i.IsDel && i.Code == model.BrandCode).FirstOrDefaultAsync();
                }


                if (brand == null)
                {
                    brand = await dbContext.Brand.Where(i => !i.IsDel && i.Name == shop.Name).FirstOrDefaultAsync();
                }
                if (brand == null)
                {


                    brand = new Brand
                    {
                        AddTime = DateTime.Now,
                        ChromotypeLogo = "",
                        Code = Guid.NewGuid().ToString(),
                        Degree = "50%",
                        Intro = shop.Intro,
                        IntroEn = shop.IntroEn,
                        IsDel = false,
                        MonochromeLogo = "",
                        Name = shop.Name,
                        NameEn = shop.NameEn,
                        UpdateTime = DateTime.Now,
                        Initials = initials,
                        Spelling = spelling
                    };
                    dbContext.Brand.Add(brand);
                }

                dbContext.BrandShop.Add(new BrandShop
                {
                    AddTime = DateTime.Now,
                    BrandCode = brand.Code,
                    Code = Guid.NewGuid().ToString(),
                    IsDel = false,
                    ShopCode = shop.Code,
                    UpdateTime = DateTime.Now
                });

                var bfcount = await dbContext.BrandFormat.Where(i => i.BrandCode == brand.Code && !i.IsDel).CountAsync();

                if (bfcount <= 0)
                {
                    dbContext.BrandFormat.Add(new BrandFormat
                    {
                        AddTime = DateTime.Now,
                        BrandCode = brand.Code,
                        Code = Guid.NewGuid().ToString(),
                        FormatCode = "",
                        IsDel = false,
                        ParentFormatCode = "",
                        UpdateTime = DateTime.Now

                    });
                }


                if (await dbContext.SaveChangesAsync() > 0)
                {
                    _Result.Code = "200";
                    _Result.Msg = "添加成功";
                    _Result.Data = new { ShopCode = shop.Code };

                    var ip = Method.GetUserIp(this.HttpContext);
                    dbContext.SysLog.Add(new SysLog { AccountName = model.UserName, ModuleName = "商铺模块", LogMsg = model.UserName + "添加店铺：" + shop.Name, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "创建", IP = ip, MallCode = model.MallCode, SystemModule = "Mall" });
                    dbContext.SaveChanges();

                }
                else
                {
                    _Result.Code = "2";
                    _Result.Msg = "添加失败";
                    _Result.Data = "";
                }
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "错误：" + e.ToString();
                _Result.Data = "";

                qMLog.WriteLogToFile("错误：", e.ToString());
            }


            return Json(_Result);
        }

        /// <summary>
        /// 删除店铺
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> ShopDel(string Code, string UserName, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            if (string.IsNullOrEmpty(UserName))
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
                    UserName = uol.UserName;
                }
            }
            //检测用户输入格式

            if (string.IsNullOrEmpty(Code))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入店铺ID";
                _Result.Data = "";
                return Json(_Result);
            }


            var shop = await dbContext.Shops.Where(i => i.Code == Code && !i.IsDel).FirstOrDefaultAsync();
            if (shop == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的店铺编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var mallshop = await dbContext.MallShop.Where(i => i.ShopCode == shop.Code).ToListAsync();
            shop.IsDel = true;
            shop.UpdateTime = DateTime.Now;
            dbContext.Shops.Update(shop);

            var brandshop = await dbContext.BrandShop.Where(i => i.ShopCode == shop.Code && !i.IsDel).ToListAsync();
            foreach (var bs in brandshop)
            {
                bs.IsDel = true;
                dbContext.BrandShop.Update(bs);
            }
            // dbContext.MallShop.RemoveRange(mallshop);

            if (await dbContext.SaveChangesAsync() > 0)
            {
                //删除店铺文件
                await FileHelper.DelFile(shop.Logo, dbContext);
                _Result.Code = "200";
                _Result.Msg = "删除成功";
                _Result.Data = "";

                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = UserName, ModuleName = "商铺模块", LogMsg = UserName + "删除店铺：" + shop.Name, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "删除", IP = ip });
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
        /// 修改店铺显示状态
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangeShopStatus(Input_ShopStatus model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_ShopStatus)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
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

            var shop = await dbContext.Shops.Where(i => i.Code == model.Code).FirstOrDefaultAsync();

            shop.IsShow = model.IsShow;
            shop.UpdateTime = DateTime.Now;
            dbContext.Update(shop);

            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "修改成功";
                _Result.Data = "";

                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = model.UserName, ModuleName = "商铺模块", LogMsg = model.UserName + "修改了店铺：" + shop.Name + "的显示状态为：" + model.IsShow, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });
                dbContext.SaveChanges();

            }
            return Json(_Result);
        }

        /// <summary>
        /// 获取商店信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetShopInfo(string code, [FromServices] ContextString dbContext)
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


            //检测用户输入格式

            try
            {




                if (string.IsNullOrEmpty(code))
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入店铺编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var shop = await dbContext.Shops.Where(i => i.Code == code && !i.IsDel).FirstOrDefaultAsync();
                if (shop == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的店铺无";
                    _Result.Data = "";
                    return Json(_Result);
                }
                var logo = await dbContext.AssetFiles.Where(i => i.FileGUID == shop.Logo).FirstOrDefaultAsync();
                string LogoPath = "";

                if (logo != null)
                {
                    //LogoPath = Method.ServerAddr + "/MallSite/" + logo.FilePath;
                    LogoPath = Method.OSSServer + logo.FilePath;
                }





                //获取店铺区域信息
                var area = await dbContext.AreaInfo.Where(i => i.Code == shop.AreaCode).FirstOrDefaultAsync();
                string AreaName = "";
                if (area != null)
                {
                    AreaName = area.AreaName;
                }

                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new
                {
                    shop.ID,
                    shop.Name,
                    shop.NameEn,
                    shop.HouseNum,
                    shop.Phone,
                    shop.Intro,
                    LogoPath,
                    shop.ShopFormat,
                    shop.SecFormat,
                    shop.BuildingCode,
                    shop.FloorCode,
                    AreaName,
                    shop.AreaCode,
                    shop.IntroEn,
                    shop.Code,
                    shop.CloseTime
                };
            }
            catch (Exception e)
            {
                QMLog qMLog = new QMLog();
                qMLog.WriteLogToFile("GetShopInfo", e.ToString());
                throw;
            }
            //var ip = Method.GetUserIp(this.HttpContext);
            //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "商铺模块", LogMsg = username + "获取店铺：" + shop.Name + "的信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
            //dbContext.SaveChanges();

            return Json(_Result);
        }

        /// <summary>
        /// 修改店铺信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ShopEdit(Input_ShopE modeledit, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            //转换参数
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            modeledit = (Input_ShopE)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, modeledit.GetType());
            var model = modeledit.Parameter;

            //中文、英文、数字但不包括下划线等符号
            Regex reg = new Regex(@"^[\u4E00-\u9FA5A-Za-z0-9]+$");

            if (!reg.IsMatch(model.Name))
            {
                _Result.Code = "510";
                _Result.Msg = "店铺名称不能有特殊字符";
                _Result.Data = "";
                return Json(_Result);
            }

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



            //验证店铺ID
            if (string.IsNullOrEmpty(model.Code))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入店铺编码";
                _Result.Data = "";
                return Json(_Result);
            }
            var shop = await dbContext.Shops.Where(i => i.Code == model.Code && !i.IsDel).FirstOrDefaultAsync();
            if (shop == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的店铺ID";
                _Result.Data = "";
                return Json(_Result);
            }



            if (string.IsNullOrEmpty(model.ShopFormat))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入业态信息";
                _Result.Data = "";
                return Json(_Result);
            }


            //验证区域参数
            if (string.IsNullOrEmpty(model.AreaCode))
            {
                model.AreaCode = string.Empty;
            }


            //创建名称拼写
            string spelling = NPinyin.Pinyin.GetPinyin(model.Name);
            string initials = NPinyin.Pinyin.GetInitials(model.Name);
            initials = Regex.Replace(initials, "[^a-z]", "", RegexOptions.IgnoreCase);

            if (shop.Name != model.Name)
            {
                //移除品牌店铺关系

                var bshop = await dbContext.BrandShop.Where(i => i.ShopCode == shop.Code && !i.IsDel).ToListAsync();
                foreach (var bs in bshop)
                {
                    bs.IsDel = true;
                    dbContext.BrandShop.Update(bs);
                }

                //修改品牌
                var brand = await dbContext.Brand.Where(i => !i.IsDel && i.Name == model.Name).FirstOrDefaultAsync();
                //品牌不存在 创建品牌
                if (brand == null)
                {


                    brand = new Brand
                    {
                        AddTime = DateTime.Now,
                        ChromotypeLogo = "",
                        Code = Guid.NewGuid().ToString(),
                        Degree = "50%",
                        Intro = model.Intro,
                        IntroEn = "",
                        IsDel = false,
                        MonochromeLogo = "",
                        Name = model.Name,
                        NameEn = model.NameEn,
                        UpdateTime = DateTime.Now,
                        Initials = initials,
                        Spelling = spelling
                    };
                    dbContext.Brand.Add(brand);


                    var bfcount = await dbContext.BrandFormat.Where(i => i.BrandCode == brand.Code && !i.IsDel).CountAsync();

                    if (bfcount <= 0)
                    {
                        dbContext.BrandFormat.Add(new BrandFormat
                        {
                            AddTime = DateTime.Now,
                            BrandCode = brand.Code,
                            Code = Guid.NewGuid().ToString(),
                            FormatCode = "",
                            IsDel = false,
                            ParentFormatCode = "",
                            UpdateTime = DateTime.Now

                        });
                    }
                }

                //添加店铺品牌关系
                dbContext.BrandShop.Add(new BrandShop
                {
                    AddTime = DateTime.Now,
                    BrandCode = brand.Code,
                    Code = Guid.NewGuid().ToString(),
                    IsDel = false,
                    ShopCode = shop.Code,
                    UpdateTime = DateTime.Now
                });

            }


            shop.BuildingCode = model.BuildingCode;

            shop.FloorCode = model.FloorCode;

            shop.HouseNum = model.HouseNum;
            shop.Intro = model.Intro;
            shop.AreaCode = model.AreaCode;
            shop.Name = model.Name;
            shop.NameEn = model.NameEn;
            shop.CloseTime = model.CloseTime;
            shop.Phone = model.Phone;
            shop.ShopFormat = model.ShopFormat;
            if (!string.IsNullOrEmpty(model.SecFormat))
            {
                shop.SecFormat = model.SecFormat;
            }
            shop.Spelling = spelling;
            shop.Initials = initials;
            shop.IntroEn = model.IntroEN;

            FileTypeJudgment fimg = new FileTypeJudgment() { TypeTarget = new TypeImg() };
            string oldlogo = string.Empty;
            //判断LOGO文件是否更改  更改后是否合法
            if (model.Logo != null)
            {


                var logofile = await dbContext.AssetFiles.Where(i => i.FileGUID == model.Logo).FirstOrDefaultAsync();

                if (logofile == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "需要有效的店铺LOGO文件";
                    _Result.Data = "";
                    return Json(_Result);
                }

                if (!fimg.Judge(logofile.FileExtName))
                {
                    _Result.Code = "510";
                    _Result.Msg = "店铺LOGO文件不是图片";
                    _Result.Data = "";
                    return Json(_Result);
                }
                if (shop.Logo != model.Logo)
                {
                    oldlogo = shop.Logo;
                }
                shop.Logo = model.Logo;
            }


            shop.UpdateTime = DateTime.Now;
            dbContext.Shops.Update(shop);




            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "修改成功";
                _Result.Data = "";

                if (!string.IsNullOrEmpty(oldlogo))
                {
                    await FileHelper.DelFile(oldlogo, dbContext);
                }
                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = model.UserName, ModuleName = "商铺模块", LogMsg = model.UserName + "修改店铺：" + shop.Name + "的信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });
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
        /// 编辑店铺导航资料
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ShopNavEdit(Input_ShopNavEdit model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_ShopNavEdit)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

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

            if (string.IsNullOrEmpty(model.ShopCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入店铺ID";
                _Result.Data = "";
                return Json(_Result);
            }

            var shop = await dbContext.Shops.Where(i => i.Code == model.ShopCode && !i.IsDel).FirstOrDefaultAsync();
            if (shop == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的店铺ID";
                _Result.Data = "";
                return Json(_Result);
            }

            shop.Xaxis = model.Xaxis;
            shop.Yaxis = model.Yaxis;
            shop.NavXaxis = model.NavXaxis;
            shop.NavYaxis = model.NavYaxis;
            shop.AreaCoordinates = model.AreaCoordinates;
            shop.UpdateTime = DateTime.Now;
            dbContext.Shops.Update(shop);



            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "修改成功";
                _Result.Data = "";

                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = model.UserName, ModuleName = "商铺模块", LogMsg = model.UserName + "编辑店铺导航资料", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip });
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
        /// 获取店铺导航信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetShopNaInfo(string code, [FromServices] ContextString dbContext)
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
                _Result.Msg = "请输入店铺ID";
                _Result.Data = "";
                return Json(_Result);
            }
            var shop = await dbContext.Shops.Where(i => i.Code == code && !i.IsDel).FirstOrDefaultAsync();

            if (shop == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的店铺编码";
                _Result.Data = "";
                return Json(_Result);
            }

            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = new { ShopID = shop.ID, shop.Code, shop.Xaxis, shop.Yaxis, shop.NavXaxis, shop.NavYaxis, shop.AreaCoordinates };

            //var ip = Method.GetUserIp(this.HttpContext);
            //dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "商铺模块", LogMsg = username + "获取店铺导航信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "查询", IP = ip });
            //dbContext.SaveChanges();
            return Json(_Result);
        }






        /// <summary>
        /// 查询店铺列表
        /// </summary>
        /// <param name="ShopID"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> QueryShopList(Input_ShopCond model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_ShopCond)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
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

            if (string.IsNullOrEmpty(model.Name))
            {
                model.Name = string.Empty;
            }

            if (model.FloorCode == "-1")
            {
                model.FloorCode = string.Empty;
            }

            if (model.ShopFormatCode == "-1")
            {
                model.ShopFormatCode = string.Empty;
            }

            if (model.BuildingCode == "-1")
            {
                model.BuildingCode = string.Empty;
            }

            /*
                        SqlParameter[] sqlParameter = new SqlParameter[]
                        {
                              new SqlParameter("@mallcode", uol.MallCode)
                        };
                        */

            try
            {
                var list = await dbContext.Output_ShopCond.FromSql(@"SELECT a.Code ,a.Name, a.BuildingCode,a.FloorCode,a.HouseNum,a.Phone, a.IsShow, a.SecFormat,a.Spelling,a.Logo,a.AreaCoordinates,
                                                                 a.Xaxis,a.Yaxis,a.NavXaxis,a.NavYaxis,a.Initials,a.AreaCode,a.CloseTime,a.Intro,
                                                                 a.ShopFormat as ShopFormatCode, 
                                                                 b.Name as ShopFormat ,
                                                                 b.Color as FormatColor,
                                                                 c.Name as FloorName,
                                                                 c.[Order] as FloorOrder,
                                                                 isnull(a.NameEn,'') as NameEn,
                                                                 isnull(f.FilePath,'') as LogoPath ,
                                                                 isnull(g.AreaName,'')as AreaName,
																 case	(select ISNULL(b.Code,'') as activity 
																         from FuncInfo a left join (SELECT * FROM MallFunc WHERE MallCode=@mallcode)  b on a.Code = b.FuncCode
						                                              	 where 	PermCode = 	( select Code  from FuncPerm where [Description] = 'StoreWeChatReg' )
																		 )
																 when ''  then '——'	
						                                       	 else
							                                              case	ISNULL(j.Code,'') 
																          when '' then  isnull(i.Num,'')
																          else '已绑定' end 
							                                     end					   
							                                     as ShopNum
                                                                 from[Shops] a 
	                                                                 left join ShopFormat b on a.ShopFormat = b.Code
	                                                                 left join [Floor] c on a.FloorCode = c.Code
	                                                                 left join AssetFile f on a.Logo = f.FileGUID
	                                                                 left join AreaInfo g on a.AreaCode = g.Code
																	 left join MallShop h on h.ShopCode = a.Code
																	 left join ShopNum i on i.ShopCode = a.Code
																	 left join ShopAccount j on j.ShopCode = a.Code
	                                                                 where h.MallCode = @mallcode and a.IsDel=0 order by a.addTime asc
                                                                 ", new SqlParameter("@mallcode", model.MallCode)).AsNoTracking().ToListAsync();


                //楼栋过滤
                if (!string.IsNullOrEmpty(model.BuildingCode))
                {
                    list = list.Where(i => i.BuildingCode == model.BuildingCode).ToList();
                }

                //名称过滤
                if (!string.IsNullOrEmpty(model.Name))
                {
                    list = list.Where(i => i.Name.ToLower().Contains(model.Name.ToLower())).ToList();
                }

                //业态过滤
                if (!string.IsNullOrEmpty(model.ShopFormatCode))
                {
                    list = list.Where(i => i.ShopFormatCode == model.ShopFormatCode).ToList();
                }

                //楼层过滤
                if (!string.IsNullOrEmpty(model.FloorCode))
                {
                    list = list.Where(i => i.FloorCode == model.FloorCode).ToList();
                }
                var shops = list.Select(s => new
                {
                    s.Code,
                    s.Name,
                    s.NameEn,
                    s.HouseNum,
                    s.BuildingCode,
                    s.FloorCode,
                    s.FloorOrder,
                    s.LogoPath,
                    s.AreaCoordinates,
                    s.Xaxis,
                    s.Yaxis,
                    s.NavXaxis,
                    s.NavYaxis,
                    s.Phone,
                    s.CloseTime,
                    s.Intro,
                    s.AreaName,
                    s.AreaCode,
                    s.FormatColor,
                    s.ShopFormat,
                    s.ShopFormatCode,
                    s.FloorName,
                    s.ShopNum,
                    s.IsShow
                }).ToList();




                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = shops;
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = e.ToString();
                _Result.Data = "";
            }

            /*
            if (!string.IsNullOrEmpty(model.FloorCode))
            {
                list = list.OrderByDescending(o => o.FloorCode).ToList();
                var thisfloor = list.Where(i => i.FloorCode == model.FloorCode).ToList();
                thisfloor = thisfloor.OrderBy(o => o.Order).ThenBy(o => o.Name).ToList();
                var otherfloor = list.Where(i => i.FloorCode != model.FloorCode).ToList();
                otherfloor = otherfloor.OrderBy(o => o.Order).ThenBy(o => o.Name).ToList();
                list.Clear();
                list.AddRange(thisfloor);
                list.AddRange(otherfloor);
            }
            else
            {
                list = list.OrderByDescending(o => o.FloorCode).ToList();
                list = list.OrderBy(o => o.Order).ThenBy(o => o.Name).ToList();
            }
            */




            return Json(_Result);
        }

        /// <summary>
        /// 查询店铺列表
        /// </summary>
        /// <param name="ShopID"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PageShopList(Input_ShopCondPage model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_ShopCondPage)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
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

            if (string.IsNullOrEmpty(model.Name))
            {
                model.Name = string.Empty;
            }

            if (model.FloorCode == "-1")
            {
                model.FloorCode = string.Empty;
            }

            if (model.ShopFormatCode == "-1")
            {
                model.ShopFormatCode = string.Empty;
            }

            if (model.BuildingCode == "-1")
            {
                model.BuildingCode = string.Empty;
            }

            try
            {
                var list = await dbContext.Output_ShopCond.FromSql(@"SELECT a.Code ,a.Name, a.BuildingCode,a.FloorCode,a.HouseNum,a.Phone, a.IsShow, a.SecFormat,a.Spelling,a.Logo,a.AreaCoordinates,
                                                                 a.Xaxis,a.Yaxis,a.NavXaxis,a.NavYaxis,a.Initials,a.AreaCode,a.CloseTime,'' as Intro,
                                                                 a.ShopFormat as ShopFormatCode, 
                                                                 b.Name as ShopFormat ,
                                                                 b.Color as FormatColor,
                                                                 c.Name as FloorName,
                                                                 c.[Order] as FloorOrder,
                                                                 isnull(a.NameEn,'') as NameEn,
                                                                 '' as LogoPath,
                                                                 --isnull(f.FilePath,'') as LogoPath ,
                                                                 isnull(g.AreaName,'')as AreaName,
																 case	(select ISNULL(b.Code,'') as activity 
																         from FuncInfo a left join (SELECT * FROM MallFunc WHERE MallCode=@mallcode)  b on a.Code = b.FuncCode
						                                              	 where 	PermCode = 	( select Code  from FuncPerm where [Description] = 'StoreWeChatReg' )
																		 )
																 when ''  then '——'	
						                                       	 else
							                                              case	ISNULL(j.Code,'') 
																          when '' then  isnull(i.Num,'')
																          else '已绑定' end 
							                                     end					   
							                                     as ShopNum
                                                                 from (select * from MallShop where MallCode = @mallcode)  h
																	 inner join [Shops] a  on h.ShopCode = a.Code 
	                                                                 inner join ShopFormat b on a.ShopFormat = b.Code
	                                                                 inner join [Floor] c on a.FloorCode = c.Code
	                                                                 --left join AssetFile f on a.Logo = f.FileGUID
	                                                                 left join AreaInfo g on a.AreaCode = g.Code
																	 --left join MallShop h on h.ShopCode = a.Code
																	 left join ShopNum i on i.ShopCode = a.Code
																	 left join ShopAccount j on j.ShopCode = a.Code
	                                                                 where a.IsDel=0 order by a.addTime asc
                                                                 ", new SqlParameter("@mallcode", model.MallCode)).AsNoTracking().ToListAsync();


                //楼栋过滤
                if (!string.IsNullOrEmpty(model.BuildingCode))
                {
                    list = list.Where(i => i.BuildingCode == model.BuildingCode).ToList();
                }

                //名称过滤
                if (!string.IsNullOrEmpty(model.Name))
                {
                    list = list.Where(i => i.Name.ToLower().Contains(model.Name.ToLower())).ToList();
                }

                //业态过滤
                if (!string.IsNullOrEmpty(model.ShopFormatCode))
                {
                    list = list.Where(i => i.ShopFormatCode == model.ShopFormatCode).ToList();
                }

                //楼层过滤
                if (!string.IsNullOrEmpty(model.FloorCode))
                {
                    list = list.Where(i => i.FloorCode == model.FloorCode).ToList();
                }
                var shops = list.Select(s => new
                {
                    s.Code,
                    s.Name,
                    s.NameEn,
                    s.HouseNum,
                    s.BuildingCode,
                    s.FloorCode,
                    s.FloorOrder,
                    s.LogoPath,
                    s.AreaCoordinates,
                    s.Xaxis,
                    s.Yaxis,
                    s.NavXaxis,
                    s.NavYaxis,
                    s.Phone,
                    s.CloseTime,
                    s.Intro,
                    s.AreaName,
                    s.AreaCode,
                    s.FormatColor,
                    s.ShopFormat,
                    s.ShopFormatCode,
                    s.FloorName,
                    s.ShopNum,
                    s.IsShow
                }).ToList();

                int allPage = 1;
                var allCount = shops.Count();
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
                    shops = shops.Skip(((int)model.PageIndex - 1) * (int)model.PageSize).Take((int)model.PageSize).ToList();
                }


                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { List = shops, AllPage = allPage, AllCount = allCount };
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = e.ToString();
                _Result.Data = "";
            }

            return Json(_Result);
        }




        /// <summary>
        /// 获取商店信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetShopAllInfo(string code, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            //检测用户输入格式

            if (string.IsNullOrEmpty(code))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入店铺编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var shop = await dbContext.Shops.Where(i => i.Code == code && i.IsShow && !i.IsDel).FirstOrDefaultAsync();
            if (shop == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的店铺无";
                _Result.Data = "";
                return Json(_Result);
            }

            //获取店铺LOGO 图片路径

            string LogoPath = "";
            var logo = await dbContext.AssetFiles.Where(i => i.FileGUID == shop.Logo).FirstOrDefaultAsync();

            if (logo != null)
            {
                //LogoPath = Method.ServerAddr + "/MallSite/" + logo.FilePath;
                LogoPath = Method.OSSServer + logo.FilePath;
            }

            //获取店铺楼层 楼栋信息
            var _Building = await dbContext.Building.Where(i => i.Code == shop.BuildingCode && !i.IsDel).Select(s => new
            {
                s.ID,
                s.Code,
                s.Name,
                s.NameEn,
                s.Order
            }).FirstOrDefaultAsync();

            string Map = "";
            var floor = await dbContext.Floor.Where(i => i.Code == shop.FloorCode && !i.IsDel).FirstOrDefaultAsync();
            var mapfile = await dbContext.AssetFiles.Where(i => i.FileGUID == floor.Map).FirstOrDefaultAsync();
            if (mapfile != null)
            {
                Map = mapfile.FilePath;
            }
            var _Floor = new { floor.ID, floor.Name, floor.NameEn, floor.Order, Map };

            //获取店铺业态信息

            var ShopFormat = await dbContext.ShopFormat.Where(i => i.Code == shop.ShopFormat && !i.IsDel).Select(s => new
            {
                s.ID,
                s.Code,
                s.Name,
                s.NameEn,
                s.Color

            }).FirstOrDefaultAsync();

            var SecFormat = await dbContext.ShopFormat.Where(i => i.Code == shop.SecFormat && !i.IsDel).Select(s => new
            {
                s.Code,
                s.ID,
                s.Name,
                s.NameEn,
                s.Color

            }).FirstOrDefaultAsync();




            //获取店铺区域信息
            var area = await dbContext.AreaInfo.Where(i => i.Code == shop.AreaCode && !i.IsDel).FirstOrDefaultAsync();
            string AreaName = "";
            if (area != null)
            {
                AreaName = area.AreaName;
            }


            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = new
            {
                shop.AddTime,
                shop.AreaCoordinates,
                Building = _Building,
                shop.CloseTime,
                Floor = _Floor,
                shop.HouseNum,
                shop.ID,
                shop.Intro,
                shop.IntroEn,
                LogoPath,
                shop.Name,
                shop.NameEn,
                shop.NavXaxis,
                shop.NavYaxis,
                shop.Phone,
                ShopFormat,
                SecFormat,
                shop.Spelling,
                shop.Xaxis,
                shop.Yaxis,
                AreaName,
                shop.AreaCode
            };

            return Json(_Result);
        }


        /// <summary>
        /// 获取商场营业时间
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetMallTime(Input_GetMallTime model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetMallTime)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

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



            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = new
            {
                OpenTime = mall.OpenTime.ToString("HH:mm"),
                CloseTime = mall.CloseTime.ToString("HH:mm")
            };

            return Json(_Result);
        }








    }
}