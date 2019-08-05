using FrontCenter.Models;
using FrontCenter.Models.Data;
using FrontCenter.ViewModels;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    public class ServerIOTHelper
    {

        public static  bool CreateServerToIOT()
        {
            DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
            ContextString dbContext = new ContextString(options);
            QianMuResult _Result = new QianMuResult();
            var serveriot =  dbContext.ServerIOT.FirstOrDefault();

            if (serveriot == null)
            {
                var servermac = Method.GetServerMac().Replace(":", "");
                //调用云端接口创建设备
                var url = Method.MallSite + "API/IOT/AddFrontServer";
                var data = new
                {
                    ServerMac = servermac,
                    MallCode = Method.CusID

                };
                try
                {
                    _Result = Method.PostMothsToObj(url, JsonHelper.SerializeJSON(data));
                    if (_Result.Code == "200")
                    {
                        IOTReturn _IOTReturn = new IOTReturn();

                        _IOTReturn = (IOTReturn)Newtonsoft.Json.JsonConvert.DeserializeObject(_Result.Data.ToString(), _IOTReturn.GetType());

                        dbContext.ServerIOT.Add(new Models.ServerIOT
                        {
                            AddTime = DateTime.Now,
                            Code = Guid.NewGuid().ToString(),
                            Key = _IOTReturn.Key,
                            Name = _IOTReturn.UserName,
                            ServerMac = servermac,
                            UpdateTime = DateTime.Now
                        });
                    }




                    if (dbContext.SaveChanges() >= 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
           

            }
            else
            {
                return true;
            }

          
        }

        private class IOTReturn
        {
            public string Key { get; set; }
            public string UserName { get; set; }
        }


        public async static Task<bool> ServerSubIOT()
        {
            DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
            ContextString dbContext = new ContextString(options);

            var serveriot = dbContext.ServerIOT.FirstOrDefault();

            ServerMqttClient mqttClient = new ServerMqttClient(Method.BaiduIOT, 1883, serveriot.ServerMac, serveriot.Name, serveriot.Key);

            mqttClient.InitAsync();

            mqttClient.Sub();

            var isStop = false;
            if (serveriot != null)
            {
                Thread thread = new Thread(new ThreadStart(()=> {
                
                        while (!isStop)
                        {

                            Thread.Sleep(120000);


                        try
                        {

                            var jsonstr = "{\"reported\": " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "}";
                            //await mqttClient.PublishAsync(serveriot.ServerMac, jsonstr);
                             mqttClient.PublishAsync(serveriot.ServerMac, jsonstr);
                    


                        }
                        catch (Exception ex)
                        {
                            QMLog qMLog = new QMLog();
                            qMLog.WriteLogToFile("", ex.ToString());

                        }
                        finally
                        {

                            Thread.Sleep(10000);
                        }
                    }

               

            }));
                thread.IsBackground = true;
                thread.Start();
            }
      

       
           

            return true;
        }


 

        public async static  Task<bool> ServerPublishIOT(ServerMqttClient mqttClient, ServerIOT serveriot)
        {
            var _r = true;
            try
            {
                var jsonstr = "{\"reported\": " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "}";
                await mqttClient.PublishAsync(serveriot.ServerMac, jsonstr);
            }
            catch (Exception ex)
            {

                QMLog qMLog = new QMLog();
                qMLog.WriteLogToFile("", "发送心跳包失败" + ex.ToString());
                _r = false;
            }
          



            return _r;
        }


    }
}
