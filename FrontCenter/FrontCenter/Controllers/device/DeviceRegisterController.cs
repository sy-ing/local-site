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
            model = (Input_Device)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


            //检测用户输入格式
            if (!ModelState.IsValid)
            {
                _Result.Code = "510";
                _Result.Msg = "请求信息不正确";
                _Result.Data = "";
                return Json(_Result);
            }

            IPAddress ipaddress;
            if (!IPAddress.TryParse(model.IP, out ipaddress))
            {
                _Result.Code = "510";
                _Result.Msg = "IP不合法";
                _Result.Data = "";
                return Json(_Result);
            }


            var build = await dbContext.Building.Where(i => i.Code == model.Building).FirstOrDefaultAsync();
            var floor = await dbContext.Floor.Where(i => i.Code == model.Floor).FirstOrDefaultAsync();

            if (build == null || floor == null)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入有效的楼层和楼栋编码";
                _Result.Data = "";
                return Json(_Result);
            }

            if (string.IsNullOrEmpty(model.SystemType))
            {
                model.SystemType = "Windows";
            }

            if (string.IsNullOrEmpty(model.DeviceType))
            {
                var devicetype = await dbContext.DataDict.Where(i => i.DictNameEn == "DeviceType" && i.DictValue == "标准型").FirstOrDefaultAsync();
                model.DeviceType = devicetype.Code;
            }
            else
            {
                var dtcount = await dbContext.DataDict.Where(i => i.DictNameEn == "DeviceType" && i.Code == model.DeviceType).CountAsync();
                if (dtcount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "请输入有效的设备类型编码";
                    _Result.Data = "";
                    return Json(_Result);
                }
            }

            // DeviceBase _Device = new DeviceBase();
            //判断设备是否已存在
            QMLog log = new QMLog();


            int deviceCount = dbContext.Device.Where(i => i.Code == model.Code).Count();

            if (!string.IsNullOrEmpty(model.Code) && deviceCount <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入有效的设备编码";
                _Result.Data = "";
                return Json(_Result);
            }


            //已存在 更新设备信息
            if (deviceCount > 0)
            {


                var _IPCount = dbContext.Device.Where(i => i.MallCode == model.MallCode && i.IP == model.IP && i.Code != model.Code && !string.IsNullOrEmpty(model.Code)).Count();

                if (_IPCount > 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "IP:" + model.IP + "已经被其他设备使用,容器信息更新失败";
                    _Result.Data = "";
                    return Json(_Result);

                }

                var _NameCount = dbContext.Device.Where(i => (i.MallCode == model.MallCode && i.DevNum == model.DevNum && i.Code != model.Code && !string.IsNullOrEmpty(model.Code)) || (i.MallCode == model.MallCode && i.DevNum == model.DevNum && i.IP != model.IP && string.IsNullOrEmpty(model.Code))).Count();

                if (_NameCount > 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "名称:" + model.DevNum + "已经被其他设备使用,容器信息更新失败";
                    _Result.Data = "";
                    return Json(_Result);

                }


                var _Device = dbContext.Device.Where(i => (i.Code == model.Code && !string.IsNullOrEmpty(model.Code)) || (i.IP == model.IP && string.IsNullOrEmpty(model.Code))).FirstOrDefault();

                string _ScreenType = dbContext.ScreenInfo.Where(i => i.Code == _Device.ScreenInfo && i.MallCode == model.MallCode).SingleOrDefault().SName;
                if (model.ScreenInfo != _ScreenType)
                {
                    _Result.Code = "1";
                    _Result.Msg = "设备分辨率应为：" + _ScreenType;
                    _Result.Data = "";
                    return Json(_Result);
                }


                var _device = dbContext.Device.Where(i => (i.Code == model.Code && !string.IsNullOrEmpty(model.Code)) || (i.IP == model.IP && string.IsNullOrEmpty(model.Code))).SingleOrDefault();
                _device.MallCode = model.MallCode;
                _device.IP = model.IP;
                _device.IsDelete = false;
                _device.Building = model.Building;
                _device.Floor = model.Floor;
                _device.DeviceOnline = true;
                _device.DevNum = model.DevNum;
                _device.FrontOnline = false;
                _device.MAC = model.MAC;
                _device.Mark = string.Empty;
                _device.SystemType = model.SystemType;
                _device.DeviceType = model.DeviceType;
                dbContext.Device.Update(_device);
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    var deviot = await dbContext.DeviceIOT.Where(i => i.DeviceCode == _Device.Code).FirstOrDefaultAsync();
                    _Result.Code = "200";
                    _Result.Msg = "更新成功";
                    _Result.Data = new { deviot.Key, deviot.Name, deviot.DeviceCode, _device.ShutdownTime };

                }
                else
                {
                    _Result.Code = "2";
                    _Result.Msg = "更新失败";
                    _Result.Data = "";
                }

                return Json(_Result);

            }


            //不存在  添加设备

            var IPCount = dbContext.Device.Where(i => i.MallCode == model.MallCode && i.IP == model.IP).Count();

            if (IPCount > 0)
            {
                _Result.Code = "510";
                _Result.Msg = "IP:" + model.IP + "已经被其他设备使用,容器注册失败";
                _Result.Data = "";
                return Json(_Result);

            }

            var NameCount = dbContext.Device.Where(i => i.MallCode == model.MallCode && i.DevNum == model.DevNum).Count();
            if (NameCount > 0)
            {
                _Result.Code = "510";
                _Result.Msg = "名称:" + model.DevNum + "已经被其他设备使用,容器信息更新失败";
                _Result.Data = "";
                return Json(_Result);

            }


            //转换屏幕类型
            int count = dbContext.ScreenInfo.Where(i => i.SName == model.ScreenInfo && i.MallCode == model.MallCode).Count();

            string _SCode = string.Empty;
            if (count > 0)
            {
                _SCode = dbContext.ScreenInfo.Where(i => i.SName == model.ScreenInfo && i.MallCode == model.MallCode).SingleOrDefault().Code;
            }
            else
            {
                ScreenInfo _SI = new ScreenInfo { SName = model.ScreenInfo, AddTime = DateTime.Now, UpdateTime = DateTime.Now, Code = Guid.NewGuid().ToString(), MallCode = model.MallCode };
                dbContext.ScreenInfo.Add(_SI);
                await dbContext.SaveChangesAsync();
                _SCode = _SI.Code;
            }




            if (string.IsNullOrEmpty(_SCode))
            {
                _Result.Code = "3";
                _Result.Msg = "添加屏幕类型失败";
                _Result.Data = "";
                return Json(_Result);
            }

            //创建并添加设备

            Device device = new Device();

            string code = Guid.NewGuid().ToString();


            device.MallCode = model.MallCode;
            device.IP = model.IP;
            device.MAC = model.MAC;
            device.IsDelete = false;
            device.AddTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            device.Building = model.Building;
            device.Floor = model.Floor;
            device.ScreenInfo = _SCode;
            device.DeviceOnline = true;
            device.Version = model.Version;
            device.DevNum = model.DevNum;
            device.FrontOnline = false;
            device.Code = code;
            device.Mark = string.Empty;
            device.UpdateTime = DateTime.Now;
            device.IsShow = false;
            device.IsSyn = false;
            device.SystemType = model.SystemType;
            device.DeviceType = model.DeviceType;
            device.Operable = true;



            dbContext.Device.Add(device);
            var host = Method.Configuration["IOT:SDKSite:Host"].ToString() + "/lotdmv3clientapi/addDevice";
            Post_AddDevice post_AddDevice = new Post_AddDevice
            {
                description = "",
                deviceName = device.Code.Replace("-", ""),
                schemaId = Method.Configuration["IOT:SchemaId"].ToString()
            };
            var param = JsonHelper.SerializeJSON(post_AddDevice);
            var _r = Method.PostMoths(host, param);

            IOTReturn _IOTReturn = new IOTReturn();
            try
            {
                _IOTReturn = (IOTReturn)Newtonsoft.Json.JsonConvert.DeserializeObject(_r, _IOTReturn.GetType());
            }
            catch (Exception e)
            {
                _Result.Code = "400";
                _Result.Msg = "创建物影子失败:" + e.ToString();
                _Result.Data = "";
                return Json(_Result);
            }
            if (_IOTReturn == null)
            {
                _Result.Code = "400";
                _Result.Msg = "创建物影子失败";
                _Result.Data = "";
                return Json(_Result);
            }
            if (string.IsNullOrEmpty(_IOTReturn.key) || string.IsNullOrEmpty(_IOTReturn.username))
            {
                _Result.Code = "400";
                _Result.Msg = "创建物影子失败";
                _Result.Data = "";
                return Json(_Result);
            }

            DeviceIOT deviceIOT = new DeviceIOT
            {
                AddTime = DateTime.Now,
                Code = Guid.NewGuid().ToString(),
                DeviceCode = device.Code,
                UpdateTime = DateTime.Now,
                Key = _IOTReturn.key,
                Name = _IOTReturn.username

            };
            dbContext.DeviceIOT.Add(deviceIOT);

            if (await dbContext.SaveChangesAsync() >= 0)
            {
                _Result.Code = "200";
                _Result.Msg = "添加设备成功";
                _Result.Data = new { deviceIOT.Key, deviceIOT.Name, deviceIOT.DeviceCode, device.ShutdownTime };
            }
            else
            {
                _Result.Code = "4";
                _Result.Msg = "添加设备失败";
                _Result.Data = "";
            }



            return Json(_Result);
        }
    }
}