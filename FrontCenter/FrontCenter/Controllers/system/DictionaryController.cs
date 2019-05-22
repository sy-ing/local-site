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

namespace FrontCenter.Controllers.system
{
    public class DictionaryController : Controller
    {

        /// <summary>
        /// 获取设备相关选项
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDeviceOptionsNew(Input_GetDeviceOptionsNew model, [FromServices] ContextString dbContext)
        {
            Output_DeviceOptionsNew _OD = new Output_DeviceOptionsNew();
            QianMuResult _Result = new QianMuResult();

            try
            {
                Stream stream = HttpContext.Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (Input_GetDeviceOptionsNew)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
                string MallCode = model.MallCode;
                if (string.IsNullOrEmpty(model.MallCode))
                {
                    UserOnLine user = Method.GetLoginUserName(dbContext, this.HttpContext);
                    if (user == null || string.IsNullOrEmpty(user.UserName))
                    {
                        _Result.Code = "401";
                        _Result.Msg = "请登陆后再进行操作";
                        _Result.Data = "";
                        return Json(_Result);
                    }
                    else
                    {
                        MallCode = user.MallCode;
                    }

                }
                if (string.IsNullOrEmpty(MallCode))
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的商场编码";
                    _Result.Data = "";
                    return Json(_Result);
                }
                var mall = await dbContext.Mall.Where(i => i.Code == MallCode).FirstOrDefaultAsync();

                if (mall == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的商场编码";
                    _Result.Data = "";
                    return Json(_Result);
                }


                _OD.ScreenInfos = await dbContext.ScreenInfo.Where(i => i.MallCode == MallCode).ToListAsync();
                List<Building> list = await dbContext.MallBuilding.Where(i => i.MallCode == MallCode).Join(dbContext.Building.Where(i => !i.IsDel), mb => mb.BuildingCode, b => b.Code, (mb, b) => b).ToListAsync();
                List<Output_Building_Local> Buildings = new List<Output_Building_Local>();
                foreach (var l in list)
                {
                    var floors = await dbContext.Floor.Where(i => i.BuildingCode == l.Code && !i.IsDel).Select(s => new {
                        FloorName = s.Name,
                        Code = s.Code
                    }).ToListAsync();

                    Buildings.Add(new Output_Building_Local { BuildingName = l.Name, Floors = floors, Code = l.Code });
                }
                _OD.Buildings = Buildings;
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = _OD;
                return Json(_Result);
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "Erro：" + e.ToString();
                _Result.Data = "";
                return Json(_Result);
                throw;
            }
        }
    }
}