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
    public class StoreController : Controller
    {


        /// <summary>
        /// 获取店铺用户列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetStoreUserList(Input_SUSearch model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_SUSearch)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
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
                    //   model.UserName = uol.UserName;
                }
            }
            var list = await dbContext.MallShop.Where(i => i.MallCode == model.MallCode).Join(dbContext.Shops.Where(i => !i.IsDel), ms => ms.ShopCode, sh => sh.Code, (ms, sh) => new
            {
                sh.HouseNum,
                sh.Name,
                sh.Code


            }).Join(dbContext.ShopAccount, sh => sh.Code, sa => sa.ShopCode, (sh, sa) => new
            {
                sa.Code,
                sh.HouseNum,
                sh.Name,
                sa.Phone,
                OrerTime = sa.AddTime,
                AddTime = sa.AddTime.ToString("yyyy-MM-dd")

            }).OrderByDescending(i => i.OrerTime).ToListAsync();

            if (!string.IsNullOrEmpty(model.SearchName))
            {
                list = list.Where(i => i.HouseNum.Contains(model.SearchName) || i.Name.Contains(model.SearchName) || i.Phone.Contains(model.SearchName)).ToList();
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
        /// 获取业态列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Unbinding(Input_SUUnbinding model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            ////检测用户登录情况
            //var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            //if (string.IsNullOrEmpty(uol.UserName))
            //{
            //    _Result.Code = "401";
            //    _Result.Msg = "请登陆后再进行操作";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_SUUnbinding)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            if (string.IsNullOrEmpty(model.Code))
            {

                _Result.Code = "510";
                _Result.Msg = "请输入绑定用户编码";
                _Result.Data = "";
                return Json(_Result);
            }

            var sa = await dbContext.ShopAccount.Where(i => i.Code == model.Code).FirstOrDefaultAsync();

            if (sa == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的编码";
                _Result.Data = "";
                return Json(_Result);
            }



            var shopnum = await dbContext.ShopNum.Where(i => i.ShopCode == sa.ShopCode).FirstOrDefaultAsync();
            shopnum.Num = Method.GetRandomStr(6, dbContext).ToString();
            shopnum.UpdateTime = DateTime.Now;

            dbContext.ShopNum.Update(shopnum);




            dbContext.ShopAccount.Remove(sa);
            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "解绑成功";
                _Result.Data = "";
            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "解绑失败";
                _Result.Data = "";
            }

            return Json(_Result);

        }


        /// <summary>
        /// 获取店铺用户列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetStoreList([FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            var list = await dbContext.Mall.ToListAsync();
            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = list;
            return Json(_Result);
        }

        /// <summary>
        /// 获取店铺用户列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetStoreUserListByManage(Input_SUSearch model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_SUSearch)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            /*
            var list = await dbContext.MallShop.Where(i => i.MallCode == model.MallCode || string.IsNullOrEmpty(model.MallCode)).Join(dbContext.Shops.Where(i => !i.IsDel), ms => ms.ShopCode, sh => sh.Code, (ms, sh) => new
            {
                sh.HouseNum,
                sh.Name,
                sh.Code,
                ms.MallCode
            }).Join(dbContext.ShopAccount, sh => sh.Code, sa => sa.ShopCode, (sh, sa) => new
            {
                sa.Code,
                sh.HouseNum,
                sh.Name,
                sa.Phone,
                OrerTime = sa.AddTime,
                AddTime = sa.AddTime.ToString("yyyy-MM-dd"),
                sh.MallCode
            }).Join(dbContext.Mall,ms=> ms.MallCode,m=>m.Code,(ms,m)=>new {
                ms.Code,
                ms.HouseNum,
                ms.Name,
                ms.Phone,
                ms.OrerTime,
                ms.AddTime,
                MallName=m.Name
            }).OrderByDescending(i => i.OrerTime).ToListAsync();
            */

            var list = await dbContext.Mall.Where(i => (string.IsNullOrEmpty(model.ProvinceID) || i.ProvinceID == model.ProvinceID)
           && (string.IsNullOrEmpty(model.CityID) || i.CityID == model.CityID)).Join(dbContext.MallShop.Where(i => i.MallCode == model.MallCode || string.IsNullOrEmpty(model.MallCode)), m => m.Code, ms => ms.MallCode, (m, ms) => ms).Join(dbContext.Shops.Where(i => !i.IsDel), ms => ms.ShopCode, sh => sh.Code, (ms, sh) => new
           {
               sh.HouseNum,
               sh.Name,
               sh.Code,
               ms.MallCode
           }).Join(dbContext.ShopAccount, sh => sh.Code, sa => sa.ShopCode, (sh, sa) => new
           {
               sa.Code,
               sh.HouseNum,
               sh.Name,
               sa.Phone,
               OrerTime = sa.AddTime,
               AddTime = sa.AddTime.ToString("yyyy-MM-dd"),
               sh.MallCode
           }).Join(dbContext.Mall, ms => ms.MallCode, m => m.Code, (ms, m) => new {
               ms.Code,
               ms.HouseNum,
               ms.Name,
               ms.Phone,
               ms.OrerTime,
               ms.AddTime,
               MallName = m.Name
           }).OrderByDescending(i => i.OrerTime).ToListAsync();
            if (!string.IsNullOrEmpty(model.SearchName))
            {
                list = list.Where(i => i.HouseNum.Contains(model.SearchName) || i.Name.Contains(model.SearchName) || i.Phone.Contains(model.SearchName)).ToList();
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
    }
}