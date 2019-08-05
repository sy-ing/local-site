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

namespace FrontCenter.Controllers.app
{
    public class AppController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GetListByCode(Input_AppDevListByCode model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_AppDevListByCode)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            try
            {
                if (string.IsNullOrEmpty(model.Code))
                {
                    _Result.Code = "510";
                    _Result.Msg = "未传入设备编码";
                    _Result.Data = "";
                    return Json(_Result);
                }

                var dev = await dbContext.Device.Where(i => i.Code == model.Code).FirstOrDefaultAsync();

                if (dev == null)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的设备编码:" + model.Code;
                    _Result.Data = "";
                    return Json(_Result);
                }
                // var files = await dbContext.AssetFiles.ToListAsync();
                var list = await dbContext.AppDev.Where(i => i.DevCode == dev.Code).Join(dbContext.ApplicationNew, ad => ad.AppCode, an => an.Code, (ad, an) => new {
                    an.Code,
                    an.FileCode,
                    an.IconFileCode,
                    an.Name,
                    an.Namespace,
                    an.ScreenInfoCode,
                    an.Startup,
                    an.Version,
                    ad.Default,
                    AppDevCode = ad.Code
                }).Join(dbContext.AssetFiles, an => an.IconFileCode, f => f.Code, (an, f) => new {
                    an.Code,
                    an.FileCode,
                    IconFilePath = Method.OSSServer + f.FilePath,
                    an.Name,
                    an.Namespace,
                    an.ScreenInfoCode,
                    an.Startup,
                    an.Version,
                    an.Default,
                    an.AppDevCode
                })
                //.Join(files, an => an.FileCode, f => f.Code, (an, f) => new {
                //    an.Code,
                //    FilePath = Method.OSSServer + f.FilePath,
                //    an.IconFilePath,
                //    an.Name,
                //    SpaceName = an.Namespace,
                //    an.ScreenInfoCode,
                //    an.Startup,
                //    an.Version,
                //    DefaultFile = an.Default
                //})
                .ToListAsync();
                ArrayList arrayList = new ArrayList();
                foreach (var an in list)
                {
                    var curApp = await dbContext.AppTime.Where(i => i.AppCode == an.AppDevCode).Join(dbContext.TimeSlot.Where(i => i.BeginTimeSlot == (DateTime.Now.ToString("HH") + ":00")), at => at.TimeSlot, ts => ts.Code, (at, ts) => at).FirstOrDefaultAsync();
                    var f = await dbContext.AssetFiles.Where(i => i.Code == an.FileCode).FirstOrDefaultAsync();
                    var defaultApp = an.Default;
                    if (curApp != null)
                    {
                        if (an.AppDevCode == curApp.AppCode)
                        {
                            defaultApp = true;
                        }
                        else
                        {
                            defaultApp = false;
                        }
                    }
                    arrayList.Add(new
                    {
                        an.Code,
                        FilePath = f == null ? "" : Method.OSSServer + f.FilePath,
                        an.IconFilePath,
                        an.Name,
                        SpaceName = an.Namespace,
                        an.ScreenInfoCode,
                        an.Startup,
                        an.Version,
                        DefaultFile = defaultApp
                    });
                }

                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = arrayList;
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "" + e.ToString();
                _Result.Data = "";

            }

            return Json(_Result);
        }
    }
}