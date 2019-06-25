using FrontCenter.Models;
using FrontCenter.Models.Data;
using FrontCenter.ViewModels;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public  static void ServerSubIOT()
        {
            DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
            ContextString dbContext = new ContextString(options);
            //QianMuResult _Result = new QianMuResult();
            var serveriot = dbContext.ServerIOT.FirstOrDefault();

            if (serveriot != null)
            {
                ServerMqttClient mqttClient = new ServerMqttClient(Method.BaiduIOT, 1883, serveriot.ServerMac, serveriot.Name, serveriot.Key);
                
                 mqttClient.InitAsync();

                var _thisjobId = BackgroundJob.Schedule(() => ServerPublishIOT(mqttClient, serveriot), TimeSpan.FromMinutes(5));



                //await mqttClient.PublishAsync(serveriot.ServerMac, "online");
                //    mqttClient.Sub();
                // return false;

            }
            else
            {
               // return true;
            }
     


        }

        public static async  void ServerPublishIOT(ServerMqttClient mqttClient, ServerIOT serveriot)
        {
           await   mqttClient.PublishAsync(serveriot.ServerMac, JsonConvert.SerializeObject(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

            var _thisjobId = BackgroundJob.Schedule(() => ServerPublishIOT(mqttClient, serveriot), TimeSpan.FromMinutes(5));
        }

   
    }
}
