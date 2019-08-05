using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using Newtonsoft.Json;
using static FrontCenter.ViewModels.DeviceManageViewModel;

namespace FrontCenter.Controllers.device
{
    public class DeviceManageController : Controller
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

                    MqttClient mqttClient = new MqttClient(Method.BaiduIOT, 1883, deviceiot.DeviceCode.Replace("-", ""), deviceiot.Name, deviceiot.Key);

                    var redata = new
                    {
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
              //  AssetFile assetfile = new AssetFile();
                //文件名

                var GUID = Guid.NewGuid();
                long filesize = HttpContext.Request.ContentLength.Value - _HeadLen;
                //文件类型
                var ext = filename.Split('.').Last();

                FileTypeJudgment ftj = new FileTypeJudgment() { TypeTarget = new TypeImg() };
                if (!ftj.Judge(ext))
                {
                    _Result.Code = "510";
                    _Result.Msg = "不是合法的文件类型";
                    _Result.Data = "";
                    return Json(_Result);
                }
                //if (Method.ImgType.Contains(ext))
                //{
                //    //图片文件
                //    assetfile.FileType = "图片";

                //}
                //if (ftj.Judge(ext))
                //{
                //    //图片文件
                //    assetfile.FileType = "图片";

                //}
                //else
                //{
                //    _Result.Code = "510";
                //    _Result.Msg = "不是合法的文件类型";
                //    _Result.Data = "";
                //    return Json(_Result);
                //}

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
                var prjinfo = dbContext.ProjectInfo.FirstOrDefault();
                FileStream fileFacestream = new FileStream(filepath, FileMode.Open);
                byte[] btFace = new byte[fileFacestream.Length];             //调用read读取方法    
                fileFacestream.Read(btFace, 0, btFace.Length);
                fileFacestream.Close();
                string faceImg = Convert.ToBase64String(btFace);
                qm.WriteLogToFile("截图", "准备上传");
                var filedata = new { FileName = filename, FileStr = faceImg, MallCode = prjinfo.CusID };
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


                string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string salt = "QianMu";
                string key = salt + dtime;

                string _EncryptionKey = Method.StringToPBKDF2Hash(key);

                var parameter = new
                {
                    CusID = prjinfo.CusID,
                    CheckTime = dtime,
                    Token = _EncryptionKey,
                    DevCode = code,
                    FilePath =  @"\Files" + @"\" + qianMuResult.Data.ToString() + @"\" + filename

            };

               var  _result = Method.PostMothsToObj(Method.MallSite + "API/CDN/SetDevImg",JsonHelper.SerializeJSON( parameter));
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

                //assetfile.AddTime = DateTime.Now;
                //assetfile.FileExtName = ext;
                ////assetfile.Code = GUID.ToString();
                //assetfile.Code = qianMuResult.Data.ToString();
                //assetfile.FileHash = "";
                //assetfile.FileName = filename.Split('.').First();
                ////assetfile.FilePath = @"\Files" + @"\" + GUID.ToString() + @"\" + filename;
                //assetfile.FilePath = @"\Files" + @"\" + qianMuResult.Data.ToString() + @"\" + filename;
                //assetfile.FileSize = filesize;

                //dbContext.AssetFiles.Add(assetfile);

                //更新截图信息到设备
                //var dev = await dbContext.Device.Where(i => i.Code == code).FirstOrDefaultAsync();
                //dev.ScreenshotSrc = assetfile.FilePath;
                if (_result.Code == "200")
                {
                    //Output_FileInfo of = new Output_FileInfo();
                   // of.FilePath = assetfile.FilePath;
                    // of.ID = assetfile.ID;
                   // of.Code = assetfile.Code;
                    _Result.Code = "200";
                    _Result.Msg = "文件已被上传完成，并加入数据库";
                    //_Result.Data = of;
                    _Result.Data = "";

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


        /*
/// <summary>
/// 设备反馈
/// </summary>
/// <param name="model"></param>
/// <param name="dbContext"></param>
/// <returns></returns>
[HttpPost]
public async Task<IActionResult> DeviceFeedback(Input_Feedback model, [FromServices] ContextString dbContext)
{
    QianMuResult _Result = new QianMuResult();


    Stream stream = HttpContext.Request.Body;
    byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
    stream.Read(buffer, 0, buffer.Length);
    string inputStr = Encoding.UTF8.GetString(buffer);
    model = (Input_Feedback)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


    //检测用户输入格式
    if (!ModelState.IsValid)
    {
        _Result.Code = "510";
        _Result.Msg = "请求信息不正确";
        _Result.Data = "";
        return Json(_Result);
    }

    //获取设备信息
    var dev = await dbContext.Device.Where(i => i.Code == model.Code).Where(o => o.IsDelete == false).AsNoTracking().FirstOrDefaultAsync();
    if (dev == null)
    {
        _Result.Code = "510";
        _Result.Msg = "无效的设备";
        _Result.Data = "";
        return Json(_Result);
    }
    if (model.Type.ToLower() == "downloadapp")
    {
        //校验状态类型是否合法
        if (model.State != 1 && model.State != 2)
        {
            _Result.Code = "1";
            _Result.Msg = "无效的状态编码";
            _Result.Data = "";
            return Json(_Result);
        }
        if (!string.IsNullOrEmpty(model.Parameter))
        {
            //获取应用信息
            var app = await dbContext.Application.Where(i => i.Name == model.Parameter.Trim()).AsNoTracking().SingleOrDefaultAsync();
            if (app == null)
            {
                _Result.Code = "2";
                _Result.Msg = "未知的应用名称";
                _Result.Data = "";
                return Json(_Result);
            }
            //获取发布信息
            //       var adev =await  dbContext.ApplicationDevice.Where(i =>( i.DeviceID == dev.ID && i.IsDel == false && i.AppID == app.ID)).AsNoTracking().FirstOrDefaultAsync();
            var adevs = await dbContext.ApplicationDevice.Where(i => (i.DeviceID == dev.ID && i.IsDel == false && i.AppID == app.ID)).AsNoTracking().ToListAsync();

            if (adevs.Count() <= 0)
            {
                _Result.Code = "3";
                _Result.Msg = "未知的发布关系";
                _Result.Data = "";
                return Json(_Result);
            }
            foreach (var adev in adevs)
            {
                adev.State = model.State;
            }

            // adev.State = model.State;
            //  dbContext.ApplicationDevice.Update(adev);
            dbContext.ApplicationDevice.UpdateRange(adevs);
            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "更新状态成功";
                _Result.Data = "";
            }
            else
            {
                _Result.Code = "1";
                _Result.Msg = "更新状态失败";
                _Result.Data = "";
            }

        }
        else
        {
            _Result.Code = "510";
            _Result.Msg = "请输入应用名称";
            _Result.Data = "";
        }

    }
    else
    {
        _Result.Code = "4";
        _Result.Msg = "未知的反馈类型";
        _Result.Data = "";
    }

    return Json(_Result);
}
*/
        /// <summary>
        /// 获取设备节目列表
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetProgramList(Input_GetProgramList_Local model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_GetProgramList_Local)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


            //检测用户输入格式
            if (String.IsNullOrEmpty(model.Code))
            {
                _Result.Code = "510";
                _Result.Msg = "编码不可为空";
                _Result.Data = "";
                return Json(_Result);
            }

            int count = dbContext.Device.Where(i => i.Code == model.Code).Count();
            if (count <= 0)
            {
                _Result.Code = "1";
                _Result.Msg = "未找到编码为：" + model.Code + "的设备";
                _Result.Data = "";
                return Json(_Result);
            }
            try
            {

                var dev = await dbContext.Device.Where(i => i.Code == model.Code).FirstOrDefaultAsync();
                var bill = await dbContext.DeviceBill.Where(i => i.DeviceCode == dev.Code && i.EffecDate == DateTime.Now.Date).FirstOrDefaultAsync();

                var properstr = "";

                if (bill != null)
                {
                    bill.Bill = bill.Bill.TrimStart('[').TrimEnd(']');
                    if (string.IsNullOrEmpty(bill.Bill))
                    {
                        properstr = bill.ProperBill;

                    }
                }







                var list = await dbContext.Outpput_ProgList.FromSql(@" 
                  select  top 100 percent e.ID ,e.progsrc as IMG ,e.switchtime as Time  ,e.switchmode as Effect ,e.ScreenMatch ,e.LaunchTime,e.ExpiryDate from  Device a
                  join DeviceToGroup b on a.Code = b.DeviceCode
                  join ProgramDevice c on c.DeviceGrounpCode = b.GroupCode
                  join ProgramToGroup d on d.GroupCode = c.ProgramGrounpCode
                  join Programs e on e.Code = d.ProgramCode
                 where
                  a.code = @Code and 
                    CONVERT(date, getdate(), 120)   between e.LaunchTime and e.ExpiryDate
                  order by   c.AddTime, c.ProgramGrounpCode,c.DeviceGrounpCode,d.[Order]", new SqlParameter("@Code", model.Code)).Select(s => new Outpput_ProgList
                {
                    Effect = s.Effect,
                    ExpiryDate = s.ExpiryDate,
                    ID = s.ID,
                    IMG = Method.OSSServer + s.IMG,
                    LaunchTime = s.LaunchTime,
                    ScreenMatch = s.ScreenMatch,
                    Time = s.Time


                }).AsNoTracking().ToListAsync();



                var listcloud = await dbContext.Output_ProgramList.FromSql(@" 
                select top 100 percent a.id , d.FilePath as progsrc ,10 as switchtime ,'随机' switchmode,'自适应' as ScreenMatch ,a.LaunchTime,a.ExpiryDate ,9999 as [Order],a.[AddTime]
                from ProgramFile a 
                left join  DeviceToGroup b on a.DevGroup = b.GroupCode 
                left join Device c on b.DeviceCode = c.Code
                left join AssetFile d on d.Code = a.ProgFile
                where c.code = @Code
                order by [Order],AddTime ", new SqlParameter("@Code", model.Code)).OrderBy(o => o.Order).Select(s => new Outpput_ProgList
                {
                    ID = s.ID,
                    IMG = Method.OSSServer + s.ProgSrc,
                    Time = s.SwitchTime,
                    Effect = s.SwitchMode,
                    ScreenMatch = s.ScreenMatch,
                    LaunchTime = Convert.ToDateTime(s.LaunchTime.ToString("yyyy-MM-dd HH:mm:mm")),
                    ExpiryDate = Convert.ToDateTime(s.ExpiryDate.ToString("yyyy-MM-dd HH:mm:mm")),
                }).AsNoTracking().ToListAsync();

                list.AddRange(listcloud);

                var liststr = JsonConvert.SerializeObject(list);
                var data = string.Empty;

                if (string.IsNullOrEmpty(properstr))
                {
                    data = liststr;
                }
                else
                {
                    if (string.IsNullOrEmpty(liststr))
                    {
                        data = properstr;
                    }
                    else
                    {
                        data = '[' + properstr.TrimStart('[').TrimEnd(']') + ',' + liststr.TrimStart('[').TrimEnd(']') + ']';
                    }
                }





                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = data;
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