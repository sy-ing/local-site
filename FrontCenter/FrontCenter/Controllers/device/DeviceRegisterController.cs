using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FrontCenter.AppCode;
using FrontCenter.Models;
using FrontCenter.Models.Data;
using FrontCenter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontCenter.Controllers.device
{
    public class DeviceRegisterController : Controller
    {

        /// <summary>
        /// 获取楼栋列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetMallBuildingList(Input_Mall_Code model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_Mall_Code)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



            var buildings = await dbContext.MallBuilding.Where(i => i.MallCode == model.MallCode).Join(dbContext.Building.Where(i => !i.IsDel),
                mb => mb.BuildingCode, b => b.Code, (mb, b) => b).ToListAsync();

            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = buildings;

            return Json(_Result);
        }


        /// <summary>
        /// 获取楼层列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetMallFloorList(Input_Building_Code model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();



            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_Building_Code)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());





            if (string.IsNullOrEmpty(model.BuildingCode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个楼栋编码";
                _Result.Data = "";

                return Json(_Result);
            }

            var floors = await dbContext.Floor.Where(i => i.BuildingCode == model.BuildingCode).ToListAsync();
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
                        floor.Code,
                        Map = ""

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
                        floor.Code,
                        Map = Method.MallSite + map.FilePath

                    });

                }
            }


            if (floors != null)
            {
                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = list;

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
        /// 获取字典值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDictListByName(Input_GetDictListByName model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetDictListByName)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            if (string.IsNullOrEmpty(model.Name))
            {
                _Result.Code = "510";
                _Result.Msg = "请按提示输入相关信息";
                _Result.Data = "";
                return Json(_Result);
            }

            var list = await dbContext.DataDict.Where(i => i.DictNameEn == model.Name.Trim()).OrderBy(o => o.ShowOrder).Select(s => new { s.ID, s.Code, s.DictValue }).ToListAsync();

            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = list;


            return Json(_Result);

        }


        /// <summary>
        /// 添加设备
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddDevice(Input_Device model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
     


            //调用云端接口创建设备
            var url = Method.MallSite + "API/IOT/AddDevice";
            _Result = Method.PostMothsToObj(url, inputStr);
            if (_Result.Code == "200")
            {
                Pull pull = new Pull();
               await pull.PullDevData();
            }
        





            return Json(_Result);
        }
    }
}