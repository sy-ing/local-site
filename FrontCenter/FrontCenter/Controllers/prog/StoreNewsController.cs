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
    public class StoreNewsController : Controller
    {
        /// <summary>
        /// 创建消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(Input_StoreNewsAdd model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //检测用户登录情况
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_StoreNewsAdd)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            DateTime dt = DateTime.Now;

            if (model.Imgs.Count() <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "至少需要一张图片";
                _Result.Data = "";
                return Json(_Result);
            }

            if (model.ValidityPeriod == null)
            {
                model.ValidityPeriod = Convert.ToDateTime("3000-01-01");
            }

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


            if (model.ValidityPeriod < DateTime.Now)
            {
                _Result.Code = "510";
                _Result.Msg = "结束时间早于当前时间";
                _Result.Data = "";
                return Json(_Result);
            }
            if (model.Imgs.Count() > 10)
            {
                _Result.Code = "510";
                _Result.Msg = "一条消息最多只能附加九张图片";
                _Result.Data = "";
                return Json(_Result);
            }

            var newscode = Guid.NewGuid().ToString();
            //var isAutoPass = 2;
            var isAutoPass = 1;
            foreach (var img in model.Imgs)
            {
                var file = await dbContext.AssetFiles.Where(i => i.Code == img).FirstOrDefaultAsync();

                if (file == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "编码：" + img + ",不是有效的图片编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                //var suspiciousPicValue = ValidatePic.GetValidatePicValue(Method.ServerAddr + "MallSite" + file.FilePath).Result;
                // var suspiciousPicValue = ValidatePic.GetValidatePicValue(Method.OSSServer  + file.FilePath).Result;

                var IsSuspicious = 0;



                if (!ValidatePic.GreenImg(Method.OSSServer + file.FilePath))
                {
                    IsSuspicious = 1;
                    isAutoPass = 1;
                }



                dbContext.NewsImg.Add(new NewsImg
                {
                    AddTime = dt,
                    Code = Guid.NewGuid().ToString(),
                    UpdateTime = dt,
                    Img = img,
                    NewsCode = newscode,
                    IsSuspicious = Convert.ToBoolean(IsSuspicious)
                });

            }

            var _PlacingNum = "SN" + Method.ConvertDateTimeInt(DateTime.Now).ToString() + Method.Number(2);
            StoreNews sn = new StoreNews
            {
                AddTime = dt,
                Code = newscode,
                News = model.News,
                ShopCode = sacount.ShopCode,
                ValidityPeriod = model.ValidityPeriod,
                UpdateTime = dt,
                AduitStatus = isAutoPass,
                MgrCode = "",
                Reason = "",
                PlacingNum = _PlacingNum
            };







            dbContext.StoreNews.Add(sn);
            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "发布成功";
                _Result.Data = "";
            }

            return Json(_Result);
        }

        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetList(Input_GetNewsList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //检测用户登录情况
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);

            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetNewsList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


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

            List<StoreNews> list = new List<StoreNews>();
            list = await dbContext.StoreNews.Where(i => i.ShopCode == sacount.ShopCode).OrderByDescending(o => o.AddTime).ToListAsync();
            if (model.Paging == 0)
            {

                ArrayList newslist = new ArrayList();

                foreach (var news in list)
                {
                    var Imgs = await dbContext.NewsImg.Where(i => i.NewsCode == news.Code).Join(dbContext.AssetFiles, ni => ni.Img, af => af.Code, (ni, af) => new {
                        af.FilePath,
                        af.Code
                    }).ToListAsync();

                    newslist.Add(new
                    {
                        AddTime = news.AddTime.ToString("yyyy-MM-dd HH:mm"),
                        news.AduitStatus,
                        news.Code,
                        news.ID,
                        news.News,
                        news.ShopCode,
                        UpdateTime = news.UpdateTime.ToString("yyyy-MM-dd HH:mm"),
                        news.PlacingNum,
                        news.Reason,
                        ValidityPeriod = news.ValidityPeriod.ToString("yyyy-MM-dd HH:mm"),
                        Imgs
                    });
                }




                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = newslist;
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
                ArrayList newslist = new ArrayList();

                foreach (var news in list)
                {
                    var Imgs = await dbContext.NewsImg.Where(i => i.NewsCode == news.Code).Join(dbContext.AssetFiles, ni => ni.Img, af => af.Code, (ni, af) => new {
                        af.FilePath,
                        af.Code
                    }).ToListAsync();

                    newslist.Add(new
                    {
                        AddTime = news.AddTime.ToString("yyyy-MM-dd HH:mm"),
                        news.AduitStatus,
                        news.Code,
                        news.ID,
                        news.News,
                        news.ShopCode,
                        news.PlacingNum,
                        news.Reason,
                        UpdateTime = news.UpdateTime == news.AddTime ? "" : news.UpdateTime.ToString("yyyy-MM-dd HH:mm"),
                        ValidityPeriod = news.ValidityPeriod.ToString("yyyy-MM-dd"),
                        Imgs
                    });


                }


                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { List = newslist, AllPage = allPage, AllCount = allCount };
            }







            return Json(_Result);
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetInfo(Input_GetNewsInfo model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //检测用户登录情况
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetNewsInfo)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


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


            var news = await dbContext.StoreNews.Where(i => i.ShopCode == sacount.ShopCode && i.Code == model.NewsCode).FirstOrDefaultAsync();


            var Imgs = await dbContext.NewsImg.Where(i => i.NewsCode == news.Code).Join(dbContext.AssetFiles, ni => ni.Img, af => af.Code, (ni, af) => new {
                af.FilePath,
                af.Code
            }).ToListAsync();

            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = new
            {
                AddTime = news.AddTime.ToString("yyyy-MM-dd HH:mm"),
                news.AduitStatus,
                news.Code,
                news.ID,
                news.News,
                news.ShopCode,
                news.PlacingNum,
                news.Reason,
                UpdateTime = news.UpdateTime == news.AddTime ? "" : news.UpdateTime.ToString("yyyy-MM-dd HH:mm"),
                ValidityPeriod = news.ValidityPeriod.ToString("yyyy-MM-dd"),
                Imgs
            };




            return Json(_Result);
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Del(Input_GetNewsInfo model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //检测用户登录情况
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetNewsInfo)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


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


            var news = await dbContext.StoreNews.Where(i => i.ShopCode == sacount.ShopCode && i.Code == model.NewsCode).FirstOrDefaultAsync();


            if (news == null)
            {
                _Result.Code = "200";
                _Result.Msg = "不存在的消息无需删除";
                _Result.Data = "";
                return Json(_Result);
            }

            dbContext.StoreNews.Remove(news);

            var Imgs = await dbContext.NewsImg.Where(i => i.NewsCode == news.Code).ToListAsync();

            dbContext.NewsImg.RemoveRange(Imgs);

            if (await dbContext.SaveChangesAsync() > 0)
            {
                //删除文件
                foreach (var img in Imgs)
                {
                    await FileHelper.DelFile(img.Img, dbContext);
                }
                _Result.Code = "200";
                _Result.Msg = "删除成功";
                _Result.Data = "";
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
        /// 获取消息列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetAuditList(Input_GetAuditNewsList model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //检测用户登录情况
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetAuditNewsList)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

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
            var _BeginDate = DateTime.MinValue;
            var _EndDate = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(model.BeginTime))
            {
                _BeginDate = Convert.ToDateTime(model.BeginTime);
            }

            if (!string.IsNullOrEmpty(model.EndTime))
            {
                _EndDate = Convert.ToDateTime(model.EndTime);
            }

            var curDate = DateTime.Now;
            var shops = await dbContext.MallShop.Where(i => i.MallCode == model.MallCode).Join(dbContext.Shops, ms => ms.ShopCode, s => s.Code, (ms, s) => new {
                ms.ShopCode,
                s.HouseNum,
                s.Name
            }).ToListAsync();
            var list = await dbContext.StoreNews.Join(shops, sn => sn.ShopCode, s => s.ShopCode, (sn, s) => new
            {
                sn.Code,
                sn.AduitStatus,
                sn.AddTime,
                sn.MgrCode,
                sn.PlacingNum,
                sn.ShopCode,
                sn.UpdateTime,
                sn.ValidityPeriod,
                s.Name,
                s.HouseNum,
                DateState = sn.ValidityPeriod.AddDays(1) > curDate ? 5 : 7
            }).Where(i => (i.Name.Contains(model.SearchName) || i.HouseNum.Contains(model.SearchName) || i.PlacingNum.Contains(model.SearchName)) && i.AddTime > _BeginDate && i.AddTime < _EndDate && ((i.AduitStatus == model.State && model.State > 0 && model.State < 5) || model.State == 0 || model.State > 4)).OrderBy(i => i.AduitStatus).ThenBy(i => i.DateState).ThenByDescending(i => i.AddTime).ToListAsync();
            if (model.State > 4)
            {
                list = list.Where(i => i.DateState == model.State && (i.AduitStatus == 3 || i.AduitStatus == 4)).OrderBy(i => i.AduitStatus).ThenBy(i => i.DateState).ThenByDescending(i => i.AddTime).ToList();
            }

            if (model.Paging == 0)
            {
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = list;
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
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { List = list, AllPage = allPage, AllCount = allCount };
            }

            return Json(_Result);
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetAuditInfo(string NewsCode, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            var news = await dbContext.StoreNews.Where(i => i.Code == NewsCode).Join(dbContext.Shops, sn => sn.ShopCode, s => s.Code, (sn, s) => new {
                sn.AddTime,
                sn.AduitStatus,
                sn.Code,
                sn.ID,
                sn.MgrCode,
                sn.News,
                sn.PlacingNum,
                sn.Reason,
                sn.ShopCode,
                sn.UpdateTime,
                sn.ValidityPeriod,
                s.Name,
                s.HouseNum
            }).FirstOrDefaultAsync();
            var Imgs = await dbContext.NewsImg.Where(i => i.NewsCode == news.Code).Join(dbContext.AssetFiles, ni => ni.Img, af => af.Code, (ni, af) => new {
                //FilePath = Method.ServerAddr +"/MallSite/"+ af.FilePath,
                FilePath = Method.OSSServer + af.FilePath,
                af.Code,
                ni.IsSuspicious,
                af.Duration,
                af.FileSize,
                af.Height,
                af.Width
            }).ToListAsync();

            _Result.Code = "200";
            _Result.Msg = "获取成功";
            var curDate = DateTime.Now;
            _Result.Data = new
            {
                news.AddTime,
                news.AduitStatus,
                news.Code,
                news.ID,
                news.News,
                news.ShopCode,
                news.UpdateTime,
                news.PlacingNum,
                news.Reason,
                news.HouseNum,
                news.Name,
                ValidityPeriod = news.ValidityPeriod.ToString("yyyy-MM-dd HH:mm"),
                Imgs,
                DateState = news.ValidityPeriod.AddDays(1) > curDate ? 5 : 7
            };
            return Json(_Result);
        }

        [HttpPost]
        public async Task<IActionResult> AuditState(Input_AuditState model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_AuditState)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            //检测用户登录情况
            //var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            //if (string.IsNullOrEmpty(uol.UserName))
            //{
            //    _Result.Code = "401";
            //    _Result.Msg = "请登陆后再进行操作";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}

            if (string.IsNullOrEmpty(model.Code))
            {
                _Result.Code = "401";
                _Result.Msg = "请输入店铺信息编码";
                _Result.Data = "";
                return Json(_Result);
            }
            var storeNews = await dbContext.StoreNews.Where(i => i.Code == model.Code).FirstOrDefaultAsync();
            if (storeNews == null)
            {
                _Result.Code = "401";
                _Result.Msg = "店铺信息编码错误";
                _Result.Data = "";
                return Json(_Result);
            }

            if (model.State == 0)
            {
                _Result.Code = "401";
                _Result.Msg = "请输入审核状态";
                _Result.Data = "";
                return Json(_Result);
            }

            if (model.State == 3)
            {
                if (string.IsNullOrEmpty(model.Reason))
                {
                    _Result.Code = "401";
                    _Result.Msg = "请输入拒绝理由";
                    _Result.Data = "";
                    return Json(_Result);
                }
            }

            storeNews.AduitStatus = model.State;
            storeNews.MgrCode = model.MgrCode;
            storeNews.UpdateTime = DateTime.Now;
            storeNews.Reason = string.IsNullOrEmpty(model.Reason) ? "" : model.Reason;
            dbContext.StoreNews.Update(storeNews);
            await dbContext.SaveChangesAsync();
            _Result.Code = "200";
            _Result.Msg = "审核成功";
            _Result.Data = "";
            return Json(_Result);
        }
    }
}