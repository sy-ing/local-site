using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FrontCenter.AppCode;
using FrontCenter.Models;
using FrontCenter.Models.Data;
using FrontCenter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontCenter.Controllers.device
{
    public class DeviceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 设备心跳包
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeviceHeartbeat(Input_Heartbeat model, [FromServices] ContextString dbContext)
        {
            try
            {



                QianMuResult _Result = new QianMuResult();
                //检测用户输入格式
                if (String.IsNullOrEmpty(model.Code))
                {
                    _Result.Code = "510";
                    _Result.Msg = "编码不可为空";
                    _Result.Data = "";
                    return Json(_Result);
                }
         
                DateTime dtime = DateTime.Now;
                //数据库操作
                List<Output_DeviceHeartbeat> heart = new List<Output_DeviceHeartbeat>();
                //判断设备是否存在
                var d = await dbContext.Device.Where(i => i.Code == model.Code).FirstOrDefaultAsync();
                if (d != null)
                {
                    #region 向云端发生心跳
                    var deviceiot = await dbContext.DeviceIOT.Where(i => i.DeviceCode == d.Code).FirstOrDefaultAsync();

                    MqttClient mqttClient = new MqttClient(Method.BaiduIOT, 1883, deviceiot.DeviceCode.Replace("-",""), deviceiot.Name, deviceiot.Key);

                    var redata = new {
                        AppName = model.AppName,
                        AppNameCH = model.AppVersion,
                        AppVersion = model.AppVersion,
                        ContainerVersion = model.ContainerVersion
                    };
                    var restr = JsonHelper.SerializeJSON(redata);
                    await mqttClient.PublishAsync(deviceiot.DeviceCode.Replace("-", ""), restr);
                    #endregion

                    var dadcount = await dbContext.DevAppDetails.Where(i => i.DevCode == d.Code).AsNoTracking().CountAsync();
                    var daocount = await dbContext.DevAppOnline.Where(i => i.DeviceCode == d.Code).AsNoTracking().CountAsync();
     

                    string appname = model.AppName == null ? "" : model.AppName.ToLower();
                    string appnamecn = model.AppNameCH == null ? "" : model.AppNameCH.ToLower();
                    var app = await dbContext.Application.Where(i => i.NameEn.ToLower() == appname && !string.IsNullOrEmpty(i.NameEn)).AsNoTracking().FirstOrDefaultAsync();
                    var appid = app == null ? "" : app.AppID;

                    if (daocount <= 0)
                    {
                        DevAppOnline dao = new DevAppOnline();
                        dao.AddTime = DateTime.Now;
                        dao.AppName = string.IsNullOrEmpty(appnamecn) ? appname : appnamecn;
                        dao.AppVersion = model.AppVersion == null ? "" : model.AppVersion.Replace('\u0000', ' ').Trim();
                        dao.ContainerVersion = model.ContainerVersion == null ? "" : model.ContainerVersion.Replace('\u0000', ' ').Trim();
                        dao.Code = Guid.NewGuid().ToString();
                        dao.UpdateTime = DateTime.Now;
                        dao.DeviceCode = d.Code;
                       

                        dbContext.DevAppOnline.Add(dao);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var dao = await dbContext.DevAppOnline.Where(i => i.DeviceCode == d.Code).FirstOrDefaultAsync();
                        dao.UpdateTime = DateTime.Now;
                        dao.AppName = string.IsNullOrEmpty(appnamecn) ? appname : appnamecn;
                        dao.AppVersion = model.AppVersion == null ? "" : model.AppVersion.Replace('\u0000', ' ').Trim();

                        dao.ContainerVersion = model.ContainerVersion == null ? "" : model.ContainerVersion.Replace('\u0000', ' ').Trim();

                        dbContext.DevAppOnline.UpdateRange(dao);
                        await dbContext.SaveChangesAsync();
                    }


                    bool isComplete = !string.IsNullOrEmpty(appid) && !string.IsNullOrEmpty(model.AppVersion);//true 完整  false 不完整


                    if (dadcount <= 0)
                    {
                        //尚无记录  容器版本或程序信息是有的 添加一条记录
                        if (!string.IsNullOrEmpty(model.ContainerVersion) || isComplete)
                        {
                            DevAppDetails dad = new DevAppDetails
                            {
                                AddTime = DateTime.Now,
                                AppID = appid,
                                AppVersion = model.AppVersion == null ? "" : model.AppVersion,
                                Code = Guid.NewGuid().ToString(),
                                ContainerVersion = model.ContainerVersion,
                                DevCode = d.Code
                            };

                            dbContext.DevAppDetails.Add(dad);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    else
                    {

                        //已有记录

                        if (!string.IsNullOrEmpty(model.ContainerVersion))
                        {
                            var dads = await dbContext.DevAppDetails.Where(i => i.DevCode == d.Code).ToListAsync();
                            foreach (var item in dads)
                            {
                                item.ContainerVersion = model.ContainerVersion;
                            }
                            dbContext.DevAppDetails.UpdateRange(dads);
                            await dbContext.SaveChangesAsync();
                        }




                        if (isComplete)
                        {

                            var dad = await dbContext.DevAppDetails.Where(i => i.DevCode == d.Code && i.AppID == appid).FirstOrDefaultAsync();

                            if (dad == null)
                            {

                                DevAppDetails devappdetial = new DevAppDetails
                                {
                                    AddTime = DateTime.Now,
                                    AppID = appid,
                                    AppVersion = model.AppVersion,
                                    Code = Guid.NewGuid().ToString(),
                                    ContainerVersion = model.ContainerVersion,
                                    DevCode = d.Code

                                };

                                dbContext.DevAppDetails.Add(devappdetial);
                                await dbContext.SaveChangesAsync();
                            }
                            else
                            {

                                dad.AppVersion = model.AppVersion;
                                dbContext.DevAppDetails.Update(dad);
                                await dbContext.SaveChangesAsync();
                            }
                        }


                    }



                    //初始化心跳包
                    heart.Add(new Output_DeviceHeartbeat { Type = "downTime", Parameter = d.ShutdownTime });


                    //判断是否有对应设备存在
                    var l = Method.DeviceCommandList.Where(i => i.Code == d.Code).FirstOrDefault();
                    if (l == null)
                    {
                        var abtime = Convert.ToDateTime("2000-01-01");
                        if (!String.IsNullOrEmpty(model.AppName) || !String.IsNullOrEmpty(model.AppNameCH))
                        {
                            abtime = dtime;
                        }
                        //添加设备
                        Method.DeviceCommandList.Add(new DeviceCommand { Code = d.Code, DevBreathTime = dtime, AppBreathTime = abtime, Type = "" });

                    }
                    else
                    {
                        l.DevBreathTime = dtime;
                        if (!String.IsNullOrEmpty(model.AppName) || !String.IsNullOrEmpty(model.AppNameCH))
                        {
                            l.AppBreathTime = dtime;
                        }
                    }

                    var tslist = await dbContext.TimeSlot.ToListAsync();
                    var tscode = "";
                    foreach (var ts in tslist)
                    {
                        var btime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + ts.BeginTimeSlot + ":00");
                        var etime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + ts.EndTimeSlot + ":00");

                        if (DateTime.Now >= btime && DateTime.Now < etime)
                        {
                            tscode = ts.Code;
                            break;
                        }
                    }
                    var appdev = await dbContext.AppTime.Where(i => i.TimeSlot == tscode).Join(dbContext.AppDev.Where(i => i.DevCode == d.Code), at => at.AppCode, ad => ad.Code, (at, ad) => new
                    {
                        ad.AppCode

                    }).Join(dbContext.ApplicationNew, ap => ap.AppCode, an => an.Code, (ap, an) => new {
                        an.Name
                    }).FirstOrDefaultAsync();

                    if (appdev == null)
                    {
                        var defaultapp = await dbContext.AppDev.Where(i => i.DevCode == d.Code && i.Default).Join(dbContext.ApplicationNew, ad => ad.AppCode, an => an.Code, (ad, an) => new {
                            an.Name
                        }).FirstOrDefaultAsync();
                        if (defaultapp != null)
                        {
                            heart.Add(new Output_DeviceHeartbeat { Parameter = defaultapp.Name, Type = "AppName" });
                        }

                    }
                    else
                    {
                        heart.Add(new Output_DeviceHeartbeat { Parameter = appdev.Name, Type = "AppName" });
                    }

  



                    _Result.Code = "200";
                    _Result.Msg = "";
                    _Result.Data = heart;
                    return Json(_Result);
                }
                else
                {
                    _Result.Code = "1";
                    _Result.Msg = "无效的IP，找不到对应机器";
                    _Result.Data = "";
                    return Json(_Result);
                }


            }
            catch (Exception e)
            {
                QMLog qMLog = new QMLog();
                qMLog.WriteLogToFile("DeviceHeartbeat", e.ToString());
                throw;
            }

        }


        /// <summary>
        /// 上传设备截图文件
        /// </summary>
        /// <param name = "dbContext" ></ param >
        /// < returns ></ returns >
        [HttpPost]
        public async Task<IActionResult> UploadScreenshot([FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            QMLog qm = new QMLog();
            try
            {


                System.Text.ASCIIEncoding ASCII = new System.Text.ASCIIEncoding();

                int _LenDec = 6;//长度说明占位长度
                int _JsonLen = 0;//长度
                int _HeadLen = 0;//头部长度


                JsonModel jmodel = new JsonModel();

                //声明字符数据，将获取到的流信息读到字符数组中
                byte[] byteArray = new byte[HttpContext.Request.ContentLength.Value];
                using (Stream stream = HttpContext.Request.Body)
                {
                    int readCount = 0; // 已经成功读取的字节的个数
                    while (readCount < HttpContext.Request.ContentLength.Value)
                    {
                        readCount += stream.Read(byteArray, readCount, (int)HttpContext.Request.ContentLength.Value - readCount);
                    }
                }

                //读取Json长度
                string jsonLength = ASCII.GetString(byteArray, 0, _LenDec);

                _JsonLen = int.Parse(jsonLength.TrimStart('0'));
                _HeadLen = _LenDec + _JsonLen;

                //读取json信息
                string inputStr = ASCII.GetString(byteArray, _LenDec, _JsonLen);
                jmodel = (JsonModel)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, jmodel.GetType());
                string code = jmodel.Code;
                string filename = jmodel.FileName;

                //检测用户输入格式
                if (String.IsNullOrEmpty(code))
                {
                    _Result.Code = "510";
                    _Result.Msg = "编码不可为空";
                    _Result.Data = "";
                    return Json(_Result);
                }
                //IPAddress ipaddress;
                //if (!IPAddress.TryParse(ip, out ipaddress))
                //{
                //    _Result.Code = "510";
                //    _Result.Msg = "IP不合法";
                //    _Result.Data = "";
                //    return Json(_Result);
                //}

                int count = dbContext.Device.Where(i => i.Code == code).Count();
                if (count <= 0)
                {
                    _Result.Code = "1";
                    _Result.Msg = "未找到编码为：" + code + "的设备";
                    _Result.Data = "";
                    return Json(_Result);
                }
                AssetFile assetfile = new AssetFile();
                //文件名

                var GUID = Guid.NewGuid();
                long filesize = HttpContext.Request.ContentLength.Value - _HeadLen;
                //文件类型
                var ext = filename.Split('.').Last();

                FileTypeJudgment ftj = new FileTypeJudgment() { TypeTarget = new TypeImg() };
                //if (Method.ImgType.Contains(ext))
                //{
                //    //图片文件
                //    assetfile.FileType = "图片";

                //}
                if (ftj.Judge(ext))
                {
                    //图片文件
                    assetfile.FileType = "图片";

                }
                else
                {
                    _Result.Code = "510";
                    _Result.Msg = "不是合法的文件类型";
                    _Result.Data = "";
                    return Json(_Result);
                }

                //文件保存路径
                var _FolderPath = @"\Files" + @"\" + GUID;
                string path = Method._hostingEnvironment.WebRootPath + _FolderPath;

                if (Directory.Exists(path) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(path);
                }

                //文件全路径
                var filepath = Method._hostingEnvironment.WebRootPath + _FolderPath + @"\" + filename;
                using (FileStream fs = System.IO.File.Create(filepath))
                {
                    fs.Write(byteArray, _HeadLen, byteArray.Length - _HeadLen);
                    fs.Flush();
                    fs.Dispose();
                }

                #region 文件上传到OSS服务器并删除本地存档
                FileStream fileFacestream = new FileStream(filepath, FileMode.Open);
                byte[] btFace = new byte[fileFacestream.Length];             //调用read读取方法    
                fileFacestream.Read(btFace, 0, btFace.Length);
                fileFacestream.Close();
                string faceImg = Convert.ToBase64String(btFace);
                qm.WriteLogToFile("截图", "准备上传");
                var filedata = new { FileName = filename, FileStr = faceImg };
                var param = JsonHelper.SerializeJSON(filedata);
                var _r = Method.PostMoths(Method.FileServer + "/FileManage/UpLoadScreenshotFiles", param);
                qm.WriteLogToFile("截图", _r);
                QianMuResult qianMuResult = new QianMuResult();
                qianMuResult = (QianMuResult)Newtonsoft.Json.JsonConvert.DeserializeObject(_r, qianMuResult.GetType());
                if (qianMuResult.Code != "200")
                {
                    _Result.Code = "2";
                    _Result.Msg = "上传文件到OSS失败";
                    _Result.Data = "";
                    return Json(_Result);
                }
                // MemoryStream majorfs = new MemoryStream();
                // majorfs.Write(byteArray, _HeadLen, byteArray.Length - _HeadLen);
                //// file.CopyTo(majorfs);
                //var majorfs = FileHelper.FileToStream(filepath);
                //AliyunOSS aliyunOSS = new AliyunOSS();
                //if (!aliyunOSS.UploadFileToOSS("Files" + "/" + assetfile.Code + "/" + assetfile.FileName + "." + assetfile.FileExtName, majorfs))
                //{
                //    _Result.Code = "2";
                //    _Result.Msg = "上传文件到OSS失败";
                //    _Result.Data = "";
                //    return Json(_Result);
                //}



                #endregion
                //添加文件到数据库

                assetfile.AddTime = DateTime.Now;
                assetfile.FileExtName = ext;
                //assetfile.Code = GUID.ToString();
                assetfile.Code = qianMuResult.Data.ToString();
                assetfile.FileHash = "";
                assetfile.FileName = filename.Split('.').First();
                //assetfile.FilePath = @"\Files" + @"\" + GUID.ToString() + @"\" + filename;
                assetfile.FilePath = @"\Files" + @"\" + qianMuResult.Data.ToString() + @"\" + filename;
                assetfile.FileSize = filesize;

                dbContext.AssetFiles.Add(assetfile);

                //更新截图信息到设备
                var dev = await dbContext.Device.Where(i => i.Code == code).FirstOrDefaultAsync();
                dev.ScreenshotSrc = assetfile.FilePath;
                if (dbContext.SaveChanges() > 0)
                {
                    Output_FileInfo of = new Output_FileInfo();
                    of.FilePath = assetfile.FilePath;
                    // of.ID = assetfile.ID;
                    of.Code = assetfile.Code;
                    _Result.Code = "200";
                    _Result.Msg = "文件已被上传完成，并加入数据库";
                    _Result.Data = of;

                }
                else
                {
                    _Result.Code = "2";
                    _Result.Msg = "添加文件到数据库失败";
                    _Result.Data = "";

                }

            }
            catch (Exception e)
            {
                qm.WriteLogToFile("", e.ToString());
                _Result.Code = "500";
                _Result.Msg = "程序运行错误";
                _Result.Data = "";
            }


            return Json(_Result);

        }



    }
}