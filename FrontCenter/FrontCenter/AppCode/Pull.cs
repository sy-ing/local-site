using FrontCenter.Models;
using FrontCenter.Models.Data;
using FrontCenter.ViewModels;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace FrontCenter.AppCode
{
    public class Pull
    {
        private object dbContext;

        /// <summary>
        /// 获取初始化信息 包含商场  权限  菜单 
        /// </summary>
        /// <returns></returns>
        public async Task<QianMuResult> PullInitData()
        {
         //   bool _r = false;

            QianMuResult _Result = new QianMuResult();

            DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
            ContextString dbContext = new ContextString(options);


            // 获取 商场
            QMLog log = new QMLog();
            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string salt = "QianMu";
            string key = salt + dtime;

            string _EncryptionKey = Method.StringToPBKDF2Hash(key);

            _EncryptionKey = System.Net.WebUtility.UrlEncode(_EncryptionKey);
            var url = Method.MallSite + "API/Cdn/SendSynData?CusID=" + Method.CusID + "&CheckTime=" + dtime + "&Token=" + _EncryptionKey+ "&Type=Init" + "&ServerMac=" + Method.ServerMac;
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };

                var response = await client.GetAsync("");
                var stream = await response.Content.ReadAsStreamAsync();
                StreamReader myStreamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                var _r = (QianMuResult)Newtonsoft.Json.JsonConvert.DeserializeObject(retString, _Result.GetType());
                //请求成功  获取文件
                if (_r.Code == "200")
                {
                    Input_PullInitData syndata = new Input_PullInitData();
                    Input_PullInitData _Syndata = (Input_PullInitData)Newtonsoft.Json.JsonConvert.DeserializeObject(_r.Data.ToString(), syndata.GetType());

                    bool nochange = true;

                    //去重

                    _Syndata.Malllist = _Syndata.Malllist.Distinct().ToList();
                    _Syndata.Menulist = _Syndata.Menulist.Distinct().ToList();
                    _Syndata.Permissionlist = _Syndata.Permissionlist.Distinct().ToList();

                
                    //商场
                    if (_Syndata.Malllist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var mall in _Syndata.Malllist)
                        {
                            var _mall = await dbContext.Mall.Where(i => i.Code == mall.Code).FirstOrDefaultAsync();

                            if (_mall == null)
                            {
                                _mall = new Mall();
                                SynDataHelper.MakeEqual(mall, _mall);
                                dbContext.Mall.Add(_mall);
                            }
                            else
                            {
                               SynDataHelper.MakeEqual(mall, _mall);
                               dbContext.Mall.Update(_mall);
                                
                            }


                        }
                    }


                    //菜单
                    if (_Syndata.Menulist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var menu in _Syndata.Menulist)
                        {
                            var _menu = await dbContext.Menu.Where(i => i.Code == menu.Code).FirstOrDefaultAsync();

                            if (_menu == null)
                            {
                                _menu = new Menu();
                                SynDataHelper.MakeEqual(menu, _menu);
                                dbContext.Menu.Add(_menu);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(menu, _menu);
                                dbContext.Menu.Update(_menu);

                            }


                        }
                    }


                    //权限
                    if (_Syndata.Permissionlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var permission in _Syndata.Permissionlist)
                        {
                            var _permission = await dbContext.Permission.Where(i => i.Code == permission.Code).FirstOrDefaultAsync();

                            if (_permission == null)
                            {
                                _permission = new Permission();
                                SynDataHelper.MakeEqual(permission, _permission);
                                dbContext.Permission.Add(_permission);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(permission, _permission);

                                dbContext.Permission.Update(_permission);

                            }


                        }
                    }

                    if (await dbContext.SaveChangesAsync() > 0 || nochange)
                    {

                       
                        _Result.Code = "200";
                        _Result.Msg = "成功";
                        _Result.Data = "";
                        log.WriteLogToFile("初始化数据同步成功", DateTime.Now.ToShortDateString());
                    }
                    else
                    {
                        _Result.Code = "2";
                        _Result.Msg = "同步失败";
                        _Result.Data = "";
                        log.WriteLogToFile("初始化数据同步失败", DateTime.Now.ToShortDateString());
                    }
                }
                else
                {
                    _Result = _r;
                    log.WriteLogToFile("初始化数据同步失败：" + _r.Msg, DateTime.Now.ToShortDateString());
                }
            }
            catch (Exception e)
            {

                _Result.Code = "500";
                _Result.Msg = e.ToString();
                _Result.Data = "";
                log.WriteLogToFile("初始化数据同步失败：" + e.ToString(), DateTime.Now.ToShortDateString());
            }

        
            return _Result;
        }

        /// <summary>
        /// 获取系统级信息   
        /// 用户
        /// 角色
        /// 用户-角色
        /// 角色-权限
        /// 日志
        /// 屏保
        /// 系统设置
        /// </summary>
        /// <returns></returns>
        public async Task<QianMuResult> PullSystemData()
        {
            QianMuResult _Result = new QianMuResult();

            DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
            ContextString dbContext = new ContextString(options);


            // 获取 商场
            QMLog log = new QMLog();
            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string salt = "QianMu";
            string key = salt + dtime;

            string _EncryptionKey = Method.StringToPBKDF2Hash(key);

            _EncryptionKey = System.Net.WebUtility.UrlEncode(_EncryptionKey);
            var url = Method.MallSite + "API/Cdn/SendSynData?CusID=" + Method.CusID + "&CheckTime=" + dtime + "&Token=" + _EncryptionKey + "&Type=System" + "&ServerMac=" + Method.ServerMac;
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };

                var response = await client.GetAsync("");
                var stream = await response.Content.ReadAsStreamAsync();
                StreamReader myStreamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                var _r = (QianMuResult)Newtonsoft.Json.JsonConvert.DeserializeObject(retString, _Result.GetType());
                //请求成功  获取文件
                if (_r.Code == "200")
                {
                    Input_PullSystemData syndata = new Input_PullSystemData();
                    Input_PullSystemData _Syndata = (Input_PullSystemData)Newtonsoft.Json.JsonConvert.DeserializeObject(_r.Data.ToString(), syndata.GetType());

                    bool nochange = true;

                    //去重

                    _Syndata.Accountlist = _Syndata.Accountlist.Distinct().ToList();
                    _Syndata.AuditProcesslist = _Syndata.AuditProcesslist.Distinct().ToList();
                    _Syndata.RolePermissionslist = _Syndata.RolePermissionslist.Distinct().ToList();
                    _Syndata.Roleslist = _Syndata.Roleslist.Distinct().ToList();
                    _Syndata.Screensaverlist = _Syndata.Screensaverlist.Distinct().ToList();
                    _Syndata.TimeAxislist = _Syndata.TimeAxislist.Distinct().ToList();
                    _Syndata.UserRoleslist = _Syndata.UserRoleslist.Distinct().ToList();
                    _Syndata.SysLoglist = _Syndata.SysLoglist.Distinct().ToList();

                    //账号
                    if (_Syndata.Accountlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.Accountlist)
                        {
                            var localData = await dbContext.Account.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new Account();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.Account.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.Account.Update(localData);

                            }


                        }
                    }

                    //审核
                    if (_Syndata.AuditProcesslist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.AuditProcesslist)
                        {
                            var localData = await dbContext.AuditProcess.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new AuditProcess();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.AuditProcess.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.AuditProcess.Update(localData);

                            }


                        }
                    }

                    //角色权限
                    if (_Syndata.RolePermissionslist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.RolePermissionslist)
                        {
                            var localData = await dbContext.RolePermissions.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new RolePermissions();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.RolePermissions.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.RolePermissions.Update(localData);

                            }


                        }
                    }


                    //角色
                    if (_Syndata.Roleslist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.Roleslist)
                        {
                            var localData = await dbContext.Roles.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new Roles();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.Roles.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.Roles.Update(localData);

                            }


                        }
                    }


                    //屏保
                    if (_Syndata.Screensaverlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.Screensaverlist)
                        {
                            var localData = await dbContext.Screensaver.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new Screensaver();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.Screensaver.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.Screensaver.Update(localData);

                            }


                        }
                    }



                    //时间轴
                    if (_Syndata.TimeAxislist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.TimeAxislist)
                        {
                            var localData = await dbContext.TimeAxis.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new TimeAxis();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.TimeAxis.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.TimeAxis.Update(localData);

                            }


                        }
                    }


                    //用户-角色
                    if (_Syndata.UserRoleslist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.UserRoleslist)
                        {
                            var localData = await dbContext.UserRoles.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new UserRoles();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.UserRoles.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.UserRoles.Update(localData);

                            }


                        }
                    }


                    //系统日志
                    if (_Syndata.SysLoglist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.SysLoglist)
                        {
                            var localData = await dbContext.SysLog.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new SysLog();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.SysLog.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.SysLog.Update(localData);

                            }


                        }
                    }

                    if (await dbContext.SaveChangesAsync() > 0 || nochange)
                    {


                        _Result.Code = "200";
                        _Result.Msg = "成功";
                        _Result.Data = "";
                        log.WriteLogToFile("系统级数据同步成功", DateTime.Now.ToShortDateString());
                    }
                    else
                    {
                        _Result.Code = "2";
                        _Result.Msg = "同步失败";
                        _Result.Data = "";
                        log.WriteLogToFile("系统级数据同步失败", DateTime.Now.ToShortDateString());
                    }
                }
                else
                {
                    _Result = _r;
                    log.WriteLogToFile("系统级数据同步失败：" + _r.Msg, DateTime.Now.ToShortDateString());
                }
            }
            catch (Exception e)
            {

                _Result.Code = "500";
                _Result.Msg = e.ToString();
                _Result.Data = "";
                log.WriteLogToFile("系统级数据同步失败：" + e.ToString(), DateTime.Now.ToShortDateString());
            }


            return _Result;
        }

        /// <summary>
        /// 获取设备相关信息   
        /// </summary>
        /// <returns></returns>
        public async Task<QianMuResult> PullDevData()
        {
            QianMuResult _Result = new QianMuResult();

            DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
            ContextString dbContext = new ContextString(options);


            // 获取 商场
            QMLog log = new QMLog();
            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string salt = "QianMu";
            string key = salt + dtime;

            string _EncryptionKey = Method.StringToPBKDF2Hash(key);

            _EncryptionKey = System.Net.WebUtility.UrlEncode(_EncryptionKey);
            var url = Method.MallSite + "API/Cdn/SendSynData?CusID=" + Method.CusID + "&CheckTime=" + dtime + "&Token=" + _EncryptionKey + "&Type=Dev" + "&ServerMac=" + Method.ServerMac;
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };

                var response = await client.GetAsync("");
                var stream = await response.Content.ReadAsStreamAsync();
                StreamReader myStreamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                var _r = (QianMuResult)Newtonsoft.Json.JsonConvert.DeserializeObject(retString, _Result.GetType());
                //请求成功  获取文件
                if (_r.Code == "200")
                {
                    Input_PullDevData syndata = new Input_PullDevData();
                    Input_PullDevData _Syndata = (Input_PullDevData)Newtonsoft.Json.JsonConvert.DeserializeObject(_r.Data.ToString(), syndata.GetType());

                    bool nochange = true;

                    //去重

                    _Syndata.DevAppOnlinelist = _Syndata.DevAppOnlinelist.Distinct().ToList();
                    _Syndata.DeviceCoordinatelist = _Syndata.DeviceCoordinatelist.Distinct().ToList();
                    _Syndata.DeviceGrouplist = _Syndata.DeviceGrouplist.Distinct().ToList();
                    _Syndata.Devicelist = _Syndata.Devicelist.Distinct().ToList();
                    _Syndata.DeviceToGrouplist = _Syndata.DeviceToGrouplist.Distinct().ToList();


                    //设备运行的应用
                    if (_Syndata.DevAppOnlinelist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.DevAppOnlinelist)
                        {
                            var localData = await dbContext.DevAppOnline.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new DevAppOnline();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.DevAppOnline.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.DevAppOnline.Update(localData);

                            }


                        }
                    }

                    //设备点位
                    if (_Syndata.DeviceCoordinatelist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.DeviceCoordinatelist)
                        {
                            var localData = await dbContext.DeviceCoordinate.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new DeviceCoordinate();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.DeviceCoordinate.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.DeviceCoordinate.Update(localData);

                            }


                        }
                    }

                    //设备组
                    if (_Syndata.DeviceGrouplist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.DeviceGrouplist)
                        {
                            var localData = await dbContext.DeviceGroup.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new DeviceGroup();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.DeviceGroup.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.DeviceGroup.Update(localData);

                            }


                        }
                    }


                    //设备
                    if (_Syndata.Devicelist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.Devicelist)
                        {
                            var localData = await dbContext.Device.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new Device();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.Device.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.Device.Update(localData);

                            }


                        }
                    }


                    //设备-组  关系
                    if (_Syndata.DeviceToGrouplist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.DeviceToGrouplist)
                        {
                            var localData = await dbContext.DeviceToGroup.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new DeviceToGroup();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.DeviceToGroup.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.DeviceToGroup.Update(localData);

                            }


                        }
                    }




                    if (await dbContext.SaveChangesAsync() > 0 || nochange)
                    {


                        _Result.Code = "200";
                        _Result.Msg = "成功";
                        _Result.Data = "";
                        log.WriteLogToFile("设备数据同步成功", DateTime.Now.ToShortDateString());
                    }
                    else
                    {
                        _Result.Code = "2";
                        _Result.Msg = "同步失败";
                        _Result.Data = "";
                        log.WriteLogToFile("设备数据同步失败", DateTime.Now.ToShortDateString());
                    }
                }
                else
                {
                    _Result = _r;
                    log.WriteLogToFile("设备数据同步失败：" + _r.Msg, DateTime.Now.ToShortDateString());
                }
            }
            catch (Exception e)
            {

                _Result.Code = "500";
                _Result.Msg = e.ToString();
                _Result.Data = "";
                log.WriteLogToFile("设备数据同步失败：" + e.ToString(), DateTime.Now.ToShortDateString());
            }


            return _Result;
        }


        /// <summary>
        /// 获取节目相关信息   
        /// 节目
        /// 节目组
        /// 节目组-设备组
        /// 直播
        /// 直播设备
        /// 字幕
        /// 字幕-设备组
        /// </summary>
        /// <returns></returns>
        public async Task<QianMuResult> PullProgramData()
        {
            QianMuResult _Result = new QianMuResult();

            DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
            ContextString dbContext = new ContextString(options);


            // 获取 商场
            QMLog log = new QMLog();
            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string salt = "QianMu";
            string key = salt + dtime;

            string _EncryptionKey = Method.StringToPBKDF2Hash(key);

            _EncryptionKey = System.Net.WebUtility.UrlEncode(_EncryptionKey);
            var url = Method.MallSite + "API/Cdn/SendSynData?CusID=" + Method.CusID + "&CheckTime=" + dtime + "&Token=" + _EncryptionKey + "&Type=Program" + "&ServerMac=" + Method.ServerMac;
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };

                var response = await client.GetAsync("");
                var stream = await response.Content.ReadAsStreamAsync();
                StreamReader myStreamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                var _r = (QianMuResult)Newtonsoft.Json.JsonConvert.DeserializeObject(retString, _Result.GetType());
                //请求成功  获取文件
                if (_r.Code == "200")
                {
                    Input_PullProgramData syndata = new Input_PullProgramData();
                    Input_PullProgramData _Syndata = (Input_PullProgramData)Newtonsoft.Json.JsonConvert.DeserializeObject(_r.Data.ToString(), syndata.GetType());

                    bool nochange = true;

                    //去重

                    _Syndata.Programslist = _Syndata.Programslist.Distinct().ToList();
                    _Syndata.ProgramGrouplist = _Syndata.ProgramGrouplist.Distinct().ToList();
                    _Syndata.ProgramToGrouplist = _Syndata.ProgramToGrouplist.Distinct().ToList();
                    _Syndata.ProgramDevicelist = _Syndata.ProgramDevicelist.Distinct().ToList();
                    _Syndata.Subtitlelist = _Syndata.Subtitlelist.Distinct().ToList();
                    _Syndata.SubtitleToDeviceGrouplist = _Syndata.SubtitleToDeviceGrouplist.Distinct().ToList();
                    _Syndata.Livelist = _Syndata.Livelist.Distinct().ToList();
                    _Syndata.LiveToDevlist = _Syndata.LiveToDevlist.Distinct().ToList();

                    //节目
                    if (_Syndata.Programslist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.Programslist)
                        {
                            var localData = await dbContext.Programs.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new Programs();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.Programs.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.Programs.Update(localData);

                            }


                        }
                    }

                    //节目组
                    if (_Syndata.ProgramGrouplist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ProgramGrouplist)
                        {
                            var localData = await dbContext.ProgramGroup.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ProgramGroup();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ProgramGroup.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ProgramGroup.Update(localData);

                            }


                        }
                    }

                    //节目-节目组
                    if (_Syndata.ProgramToGrouplist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ProgramToGrouplist)
                        {
                            var localData = await dbContext.ProgramToGroup.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ProgramToGroup();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ProgramToGroup.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ProgramToGroup.Update(localData);

                            }


                        }
                    }


                    //节目组-设备组
                    if (_Syndata.ProgramDevicelist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ProgramDevicelist)
                        {
                            var localData = await dbContext.ProgramDevice.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ProgramDevice();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ProgramDevice.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ProgramDevice.Update(localData);

                            }


                        }
                    }


                    //字幕
                    if (_Syndata.Subtitlelist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.Subtitlelist)
                        {
                            var localData = await dbContext.Subtitle.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new Subtitle();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.Subtitle.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.Subtitle.Update(localData);

                            }


                        }
                    }



                    //字幕-设备组
                    if (_Syndata.SubtitleToDeviceGrouplist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.SubtitleToDeviceGrouplist)
                        {
                            var localData = await dbContext.SubtitleToDeviceGroup.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new SubtitleToDeviceGroup();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.SubtitleToDeviceGroup.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.SubtitleToDeviceGroup.Update(localData);

                            }


                        }
                    }


                    //直播
                    if (_Syndata.Livelist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.Livelist)
                        {
                            var localData = await dbContext.Live.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new Live();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.Live.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.Live.Update(localData);

                            }


                        }
                    }


                    //直播-设备
                    if (_Syndata.LiveToDevlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.LiveToDevlist)
                        {
                            var localData = await dbContext.LiveToDev.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new LiveToDev();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.LiveToDev.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.LiveToDev.Update(localData);

                            }


                        }
                    }

                    if (await dbContext.SaveChangesAsync() > 0 || nochange)
                    {


                        _Result.Code = "200";
                        _Result.Msg = "成功";
                        _Result.Data = "";
                        log.WriteLogToFile("节目数据同步成功", DateTime.Now.ToShortDateString());
                    }
                    else
                    {
                        _Result.Code = "2";
                        _Result.Msg = "同步失败";
                        _Result.Data = "";
                        log.WriteLogToFile("节目数据同步失败", DateTime.Now.ToShortDateString());
                    }
                }
                else
                {
                    _Result = _r;
                    log.WriteLogToFile("节目数据同步失败：" + _r.Msg, DateTime.Now.ToShortDateString());
                }
            }
            catch (Exception e)
            {

                _Result.Code = "500";
                _Result.Msg = e.ToString();
                _Result.Data = "";
                log.WriteLogToFile("节目数据同步失败：" + e.ToString(), DateTime.Now.ToShortDateString());
            }


            return _Result;
        }


        /// <summary>
        /// 获取应用相关信息   
        /// 应用
        /// 应用分类
        /// 应用设备
        /// 应用时间段
        /// 应用后台地址
        /// 应用使用情况
        /// </summary>
        /// <returns></returns>
        public async Task<QianMuResult> PullAppData()
        {
            QianMuResult _Result = new QianMuResult();

            DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
            ContextString dbContext = new ContextString(options);


            // 获取 商场
            QMLog log = new QMLog();
            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string salt = "QianMu";
            string key = salt + dtime;

            string _EncryptionKey = Method.StringToPBKDF2Hash(key);

            _EncryptionKey = System.Net.WebUtility.UrlEncode(_EncryptionKey);
            var url = Method.MallSite + "API/Cdn/SendSynData?CusID=" + Method.CusID + "&CheckTime=" + dtime + "&Token=" + _EncryptionKey + "&Type=App" + "&ServerMac=" + Method.ServerMac;
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };

                var response = await client.GetAsync("");
                var stream = await response.Content.ReadAsStreamAsync();
                StreamReader myStreamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                var _r = (QianMuResult)Newtonsoft.Json.JsonConvert.DeserializeObject(retString, _Result.GetType());
                //请求成功  获取文件
                if (_r.Code == "200")
                {
                    Input_PullAppData syndata = new Input_PullAppData();
                    Input_PullAppData _Syndata = (Input_PullAppData)Newtonsoft.Json.JsonConvert.DeserializeObject(_r.Data.ToString(), syndata.GetType());

                    bool nochange = true;

                    //去重


                    _Syndata.AppClassNewlist = _Syndata.AppClassNewlist.Distinct().ToList();
                    _Syndata.AppDevlist = _Syndata.AppDevlist.Distinct().ToList();
                  
                    _Syndata.ApplicationNewlist = _Syndata.ApplicationNewlist.Distinct().ToList();
                    _Syndata.AppSitelist = _Syndata.AppSitelist.Distinct().ToList();
                    _Syndata.AppTimelist = _Syndata.AppTimelist.Distinct().ToList();
                    _Syndata.AppUsageInfolist = _Syndata.AppUsageInfolist.Distinct().ToList();
                    //应用分类
                    if (_Syndata.AppClassNewlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.AppClassNewlist)
                        {
                            var localData = await dbContext.AppClassNew.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new AppClassNew();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.AppClassNew.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.AppClassNew.Update(localData);

                            }


                        }
                    }

                    //应用设备
                    if (_Syndata.AppDevlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.AppDevlist)
                        {
                            var localData = await dbContext.AppDev.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new AppDev();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.AppDev.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.AppDev.Update(localData);

                            }


                        }
                    }




                    //应用
                    if (_Syndata.ApplicationNewlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ApplicationNewlist)
                        {
                            var localData = await dbContext.ApplicationNew.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ApplicationNew();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ApplicationNew.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ApplicationNew.Update(localData);

                            }


                        }
                    }


                    //应用后台地址
                    if (_Syndata.AppSitelist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.AppSitelist)
                        {
                            var localData = await dbContext.AppSite.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new AppSite();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.AppSite.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.AppSite.Update(localData);

                            }


                        }
                    }



                    //应用时间段
                    if (_Syndata.AppTimelist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.AppTimelist)
                        {
                            var localData = await dbContext.AppTime.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new AppTime();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.AppTime.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.AppTime.Update(localData);

                            }


                        }
                    }


                    //应用使用情况
                    if (_Syndata.AppUsageInfolist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.AppUsageInfolist)
                        {
                            var localData = await dbContext.AppUsageInfo.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new AppUsageInfo();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.AppUsageInfo.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.AppUsageInfo.Update(localData);

                            }


                        }
                    }




                    if (await dbContext.SaveChangesAsync() > 0 || nochange)
                    {


                        _Result.Code = "200";
                        _Result.Msg = "成功";
                        _Result.Data = "";
                        log.WriteLogToFile("应用数据同步成功", DateTime.Now.ToShortDateString());
                    }
                    else
                    {
                        _Result.Code = "2";
                        _Result.Msg = "同步失败";
                        _Result.Data = "";
                        log.WriteLogToFile("应用数据同步失败", DateTime.Now.ToShortDateString());
                    }
                }
                else
                {
                    _Result = _r;
                    log.WriteLogToFile("应用数据同步失败：" + _r.Msg, DateTime.Now.ToShortDateString());
                }
            }
            catch (Exception e)
            {

                _Result.Code = "500";
                _Result.Msg = e.ToString();
                _Result.Data = "";
                log.WriteLogToFile("应用数据同步失败：" + e.ToString(), DateTime.Now.ToShortDateString());
            }


            return _Result;
        }

        /// <summary>
        /// 获取审核相关信息   
        /// 小程序订单相关表
        /// </summary>
        /// <returns></returns>
        public async Task<QianMuResult> PullReviewData()
        {
            QianMuResult _Result = new QianMuResult();

            DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
            ContextString dbContext = new ContextString(options);


            // 获取 商场
            QMLog log = new QMLog();
            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string salt = "QianMu";
            string key = salt + dtime;

            string _EncryptionKey = Method.StringToPBKDF2Hash(key);

            _EncryptionKey = System.Net.WebUtility.UrlEncode(_EncryptionKey);
            var url = Method.MallSite + "API/Cdn/SendSynData?CusID=" + Method.CusID + "&CheckTime=" + dtime + "&Token=" + _EncryptionKey + "&Type=Review" + "&ServerMac=" + Method.ServerMac;
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };

                var response = await client.GetAsync("");
                var stream = await response.Content.ReadAsStreamAsync();
                StreamReader myStreamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                var _r = (QianMuResult)Newtonsoft.Json.JsonConvert.DeserializeObject(retString, _Result.GetType());
                //请求成功  获取文件
                if (_r.Code == "200")
                {
                    Input_PullReviewData syndata = new Input_PullReviewData();
                    Input_PullReviewData _Syndata = (Input_PullReviewData)Newtonsoft.Json.JsonConvert.DeserializeObject(_r.Data.ToString(), syndata.GetType());

                    bool nochange = true;

                    //去重


                    _Syndata.OrderAuditlist = _Syndata.OrderAuditlist.Distinct().ToList();
                    _Syndata.ProgramMateriallist = _Syndata.ProgramMateriallist.Distinct().ToList();
                    _Syndata.ProperMateriallist = _Syndata.ProperMateriallist.Distinct().ToList();
                    _Syndata.ScheduleDatelist = _Syndata.ScheduleDatelist.Distinct().ToList();
                    _Syndata.ScheduleDevicelist = _Syndata.ScheduleDevicelist.Distinct().ToList();
                    _Syndata.ScheduleMateriallist = _Syndata.ScheduleMateriallist.Distinct().ToList();
                    _Syndata.ScheduleOrderlist = _Syndata.ScheduleOrderlist.Distinct().ToList();
                    _Syndata.SchedulePeriodlist = _Syndata.SchedulePeriodlist.Distinct().ToList();
                    _Syndata.StoreNewslist = _Syndata.StoreNewslist.Distinct().ToList();

                    //节目订单审核
                    if (_Syndata.OrderAuditlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.OrderAuditlist)
                        {
                            var localData = await dbContext.OrderAudit.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new OrderAudit();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.OrderAudit.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.OrderAudit.Update(localData);

                            }


                        }
                    }

                    //节目素材
                    if (_Syndata.ProgramMateriallist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ProgramMateriallist)
                        {
                            var localData = await dbContext.ProgramMaterial.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ProgramMaterial();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ProgramMaterial.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ProgramMaterial.Update(localData);

                            }


                        }
                    }

                    //专属节目素材
                    if (_Syndata.ProperMateriallist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ProperMateriallist)
                        {
                            var localData = await dbContext.ProperMaterial.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ProperMaterial();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ProperMaterial.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ProperMaterial.Update(localData);

                            }


                        }
                    }


                    //订单时间
                    if (_Syndata.ScheduleDatelist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ScheduleDatelist)
                        {
                            var localData = await dbContext.ScheduleDate.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ScheduleDate();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ScheduleDate.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ScheduleDate.Update(localData);

                            }


                        }
                    }


                    //订单设备
                    if (_Syndata.ScheduleDevicelist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ScheduleDevicelist)
                        {
                            var localData = await dbContext.ScheduleDevice.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ScheduleDevice();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ScheduleDevice.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ScheduleDevice.Update(localData);

                            }


                        }
                    }



                    //订单素材
                    if (_Syndata.ScheduleMateriallist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ScheduleMateriallist)
                        {
                            var localData = await dbContext.ScheduleMaterial.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ScheduleMaterial();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ScheduleMaterial.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ScheduleMaterial.Update(localData);

                            }


                        }
                    }


                    //排期订单
                    if (_Syndata.ScheduleOrderlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ScheduleOrderlist)
                        {
                            var localData = await dbContext.ScheduleOrder.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ScheduleOrder();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ScheduleOrder.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ScheduleOrder.Update(localData);

                            }


                        }
                    }


                    //排期时间段
                    if (_Syndata.SchedulePeriodlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.SchedulePeriodlist)
                        {
                            var localData = await dbContext.SchedulePeriod.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new SchedulePeriod();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.SchedulePeriod.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.SchedulePeriod.Update(localData);

                            }


                        }
                    }



                    //店铺信息
                    if (_Syndata.StoreNewslist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.StoreNewslist)
                        {
                            var localData = await dbContext.StoreNews.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new StoreNews();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.StoreNews.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.StoreNews.Update(localData);

                            }


                        }
                    }


                    if (await dbContext.SaveChangesAsync() > 0 || nochange)
                    {


                        _Result.Code = "200";
                        _Result.Msg = "成功";
                        _Result.Data = "";
                        log.WriteLogToFile("订单数据同步成功", DateTime.Now.ToShortDateString());
                    }
                    else
                    {
                        _Result.Code = "2";
                        _Result.Msg = "同步失败";
                        _Result.Data = "";
                        log.WriteLogToFile("订单数据同步失败", DateTime.Now.ToShortDateString());
                    }
                }
                else
                {
                    _Result = _r;
                    log.WriteLogToFile("订单数据同步失败：" + _r.Msg, DateTime.Now.ToShortDateString());
                }
            }
            catch (Exception e)
            {

                _Result.Code = "500";
                _Result.Msg = e.ToString();
                _Result.Data = "";
                log.WriteLogToFile("订单数据同步失败：" + e.ToString(), DateTime.Now.ToShortDateString());
            }


            return _Result;
        }


        /// <summary>
        /// 获取商场及店铺相关信息   
        /// </summary>
        /// <returns></returns>
        public async Task<QianMuResult> PullShopInfoData()
        {
            QianMuResult _Result = new QianMuResult();

            DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
            ContextString dbContext = new ContextString(options);


            // 获取 商场
            QMLog log = new QMLog();
            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string salt = "QianMu";
            string key = salt + dtime;

            string _EncryptionKey = Method.StringToPBKDF2Hash(key);

            _EncryptionKey = System.Net.WebUtility.UrlEncode(_EncryptionKey);
            var url = Method.MallSite + "API/Cdn/SendSynData?CusID=" + Method.CusID + "&CheckTime=" + dtime + "&Token=" + _EncryptionKey + "&Type=ShopInfo" + "&ServerMac=" + Method.ServerMac;
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };

                var response = await client.GetAsync("");
                var stream = await response.Content.ReadAsStreamAsync();
                StreamReader myStreamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                var _r = (QianMuResult)Newtonsoft.Json.JsonConvert.DeserializeObject(retString, _Result.GetType());
                //请求成功  获取文件
                if (_r.Code == "200")
                {
                    Input_PullShopInfoData syndata = new Input_PullShopInfoData();
                    Input_PullShopInfoData _Syndata = (Input_PullShopInfoData)Newtonsoft.Json.JsonConvert.DeserializeObject(_r.Data.ToString(), syndata.GetType());

                    bool nochange = true;

                    //去重


                    _Syndata.AreaInfolist = _Syndata.AreaInfolist.Distinct().ToList();
                    _Syndata.Buildinglist = _Syndata.Buildinglist.Distinct().ToList();
                    _Syndata.Floorlist = _Syndata.Floorlist.Distinct().ToList();
                    _Syndata.MallBuildinglist = _Syndata.MallBuildinglist.Distinct().ToList();
                    _Syndata.ParkingLotlist = _Syndata.ParkingLotlist.Distinct().ToList();
                    _Syndata.ParkingSpacelist = _Syndata.ParkingSpacelist.Distinct().ToList();
                    _Syndata.ShopAccountlist = _Syndata.ShopAccountlist.Distinct().ToList();
                    _Syndata.ShopFormatlist = _Syndata.ShopFormatlist.Distinct().ToList();
                    _Syndata.ShopNumlist = _Syndata.ShopNumlist.Distinct().ToList();

                    _Syndata.Shopslist = _Syndata.Shopslist.Distinct().ToList();
                    _Syndata.ShopToDevicelist = _Syndata.ShopToDevicelist.Distinct().ToList();

                    //区域
                    if (_Syndata.AreaInfolist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.AreaInfolist)
                        {
                            var localData = await dbContext.AreaInfo.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new AreaInfo();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.AreaInfo.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.AreaInfo.Update(localData);

                            }


                        }
                    }

                    //楼栋
                    if (_Syndata.Buildinglist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.Buildinglist)
                        {
                            var localData = await dbContext.Building.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new Building();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.Building.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.Building.Update(localData);

                            }


                        }
                    }

                    //楼层
                    if (_Syndata.Floorlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.Floorlist)
                        {
                            var localData = await dbContext.Floor.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new Floor();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.Floor.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.Floor.Update(localData);

                            }


                        }
                    }


                    //商场楼栋关系
                    if (_Syndata.MallBuildinglist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.MallBuildinglist)
                        {
                            var localData = await dbContext.MallBuilding.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new MallBuilding();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.MallBuilding.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.MallBuilding.Update(localData);

                            }


                        }
                    }


                    //停车场
                    if (_Syndata.ParkingLotlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ParkingLotlist)
                        {
                            var localData = await dbContext.ParkingLot.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ParkingLot();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ParkingLot.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ParkingLot.Update(localData);

                            }


                        }
                    }



                    //停车位
                    if (_Syndata.ParkingSpacelist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ParkingSpacelist)
                        {
                            var localData = await dbContext.ParkingSpace.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ParkingSpace();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ParkingSpace.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ParkingSpace.Update(localData);

                            }


                        }
                    }


                    //商家账户
                    if (_Syndata.ShopAccountlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ShopAccountlist)
                        {
                            var localData = await dbContext.ShopAccount.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ShopAccount();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ShopAccount.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ShopAccount.Update(localData);

                            }


                        }
                    }


                    //业态
                    if (_Syndata.ShopFormatlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ShopFormatlist)
                        {
                            var localData = await dbContext.ShopFormat.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ShopFormat();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ShopFormat.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ShopFormat.Update(localData);

                            }


                        }
                    }



                    //店铺随机码
                    if (_Syndata.ShopNumlist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ShopNumlist)
                        {
                            var localData = await dbContext.ShopNum.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ShopNum();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ShopNum.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ShopNum.Update(localData);

                            }


                        }
                    }

                    //店铺
                    if (_Syndata.Shopslist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.Shopslist)
                        {
                            var localData = await dbContext.Shops.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new Shops();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.Shops.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.Shops.Update(localData);

                            }


                        }
                    }

                    //店铺专属设备
                    if (_Syndata.ShopToDevicelist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.ShopToDevicelist)
                        {
                            var localData = await dbContext.ShopToDevice.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                localData = new ShopToDevice();
                                SynDataHelper.MakeEqual(cloudData, localData);
                                dbContext.ShopToDevice.Add(localData);
                            }
                            else
                            {
                                SynDataHelper.MakeEqual(cloudData, localData);

                                dbContext.ShopToDevice.Update(localData);

                            }


                        }
                    }


                    if (await dbContext.SaveChangesAsync() > 0 || nochange)
                    {


                        _Result.Code = "200";
                        _Result.Msg = "成功";
                        _Result.Data = "";
                        log.WriteLogToFile("店铺数据同步成功", DateTime.Now.ToShortDateString());
                    }
                    else
                    {
                        _Result.Code = "2";
                        _Result.Msg = "同步失败";
                        _Result.Data = "";
                        log.WriteLogToFile("店铺数据同步失败", DateTime.Now.ToShortDateString());
                    }
                }
                else
                {
                    _Result = _r;
                    log.WriteLogToFile("店铺数据同步失败：" + _r.Msg, DateTime.Now.ToShortDateString());
                }
            }
            catch (Exception e)
            {

                _Result.Code = "500";
                _Result.Msg = e.ToString();
                _Result.Data = "";
                log.WriteLogToFile("店铺数据同步失败：" + e.ToString(), DateTime.Now.ToShortDateString());
            }


            return _Result;
        }



        /// <summary>
        /// 获取商场及店铺相关信息   
        /// </summary>
        /// <returns></returns>
        public async Task<QianMuResult> PullFileData()
        {
            QianMuResult _Result = new QianMuResult();

            DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
            ContextString dbContext = new ContextString(options);


            // 获取 商场
            QMLog log = new QMLog();
            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string salt = "QianMu";
            string key = salt + dtime;

            string _EncryptionKey = Method.StringToPBKDF2Hash(key);

            _EncryptionKey = System.Net.WebUtility.UrlEncode(_EncryptionKey);
            var url = Method.MallSite + "API/Cdn/SendSynData?CusID=" + Method.CusID + "&CheckTime=" + dtime + "&Token=" + _EncryptionKey + "&Type=File"+"&ServerMac="+Method.ServerMac + "&ServerMac=" + Method.ServerMac;
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };

                var response = await client.GetAsync("");
                var stream = await response.Content.ReadAsStreamAsync();
                StreamReader myStreamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                var _r = (QianMuResult)Newtonsoft.Json.JsonConvert.DeserializeObject(retString, _Result.GetType());
                //请求成功  获取文件
                if (_r.Code == "200")
                {
                    Input_PullFileData syndata = new Input_PullFileData();
                    Input_PullFileData _Syndata = (Input_PullFileData)Newtonsoft.Json.JsonConvert.DeserializeObject(_r.Data.ToString(), syndata.GetType());

                    bool nochange = true;

                    //去重


                    _Syndata.AssetFilelist = _Syndata.AssetFilelist.Distinct().ToList();
                   

                    //区域
                    if (_Syndata.AssetFilelist.Count() > 0)
                    {
                        nochange = false;
                        foreach (var cloudData in _Syndata.AssetFilelist)
                        {
                            var localData = await dbContext.AssetFiles.Where(i => i.Code == cloudData.Code).FirstOrDefaultAsync();

                            if (localData == null)
                            {
                                dbContext.AssetFiles.Add(new AssetFile
                                {
                                    AddTime = cloudData.AddTime,
                                    Duration = cloudData.Duration,
                                    FileExtName = cloudData.FileExtName,
                                    Code = cloudData.Code,
                                    FileHash = cloudData.FileHash,
                                    FileName = cloudData.FileName,
                                    FilePath = cloudData.FilePath,
                                    FileSize = cloudData.FileSize,
                                    FileType = cloudData.FileType,
                                    Height = cloudData.Height,
                                    UpdateTime = cloudData.UpdateTime,
                                    Width = cloudData.Width
                                });

                                dbContext.FileToBeDown.Add(new FileToBeDown
                                {
                                    AddTime = DateTime.Now,
                                    Code = Guid.NewGuid().ToString(),
                                    FileCode = cloudData.Code,
                                    StartNum = 0,
                                    UpdateTime = DateTime.Now
                                });
                            }
                            else
                            {
                                localData.AddTime = cloudData.AddTime;
                                localData.Duration = cloudData.Duration;
                                localData.FileExtName = cloudData.FileExtName;
                                localData.Code = cloudData.Code;
                                localData.FileHash = cloudData.FileHash;
                                localData.FileName = cloudData.FileName;
                                localData.FilePath = cloudData.FilePath;
                                localData.FileSize = cloudData.FileSize;
                                localData.FileType = cloudData.FileType;
                                localData.Height = cloudData.Height;
                                localData.UpdateTime = cloudData.UpdateTime;
                                localData.Width = cloudData.Width;

                                dbContext.AssetFiles.Update(localData);



                            }


                        }
                    }

                   if (await dbContext.SaveChangesAsync() > 0 || nochange)
                    {

                        var _thisjobId = BackgroundJob.Schedule(() => Method.StartDownTask(), TimeSpan.FromSeconds(5));
                        _Result.Code = "200";
                        _Result.Msg = "成功";
                        _Result.Data = "";
                        log.WriteLogToFile("文件数据同步成功", DateTime.Now.ToShortDateString());
                    }
                    else
                    {
                        _Result.Code = "2";
                        _Result.Msg = "同步失败";
                        _Result.Data = "";
                        log.WriteLogToFile("文件数据同步失败", DateTime.Now.ToShortDateString());
                    }
                }
                else
                {
                    _Result = _r;
                    log.WriteLogToFile("文件数据同步失败：" + _r.Msg, DateTime.Now.ToShortDateString());
                }
            }
            catch (Exception e)
            {

                _Result.Code = "500";
                _Result.Msg = e.ToString();
                _Result.Data = "";
                log.WriteLogToFile("文件数据同步失败：" + e.ToString(), DateTime.Now.ToShortDateString());
            }


            return _Result;
        }

    }
}
