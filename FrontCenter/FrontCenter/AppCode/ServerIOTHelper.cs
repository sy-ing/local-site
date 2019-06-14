using FrontCenter.Models;
using FrontCenter.Models.Data;
using FrontCenter.ViewModels;
using Microsoft.EntityFrameworkCore;
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
                var url = Method.MallSite + "/API/IOT/AddFrontServer";
                var data = new
                {
                    ServerMac = servermac,
                    MallCode = Method.CusID

                };
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




                if ( dbContext.SaveChanges() >= 0)
                {
                    return true;
                }
                else
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


        public static bool ServerSubIOT()
        {
            DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
            ContextString dbContext = new ContextString(options);
            //QianMuResult _Result = new QianMuResult();
            var serveriot = dbContext.ServerIOT.FirstOrDefault();

            if (serveriot == null)
            {
                MqttClient mqttClient = new MqttClient(Method.BaiduIOT, 1883, serveriot.ServerMac, serveriot.Name, serveriot.Key);
                mqttClient.Sub();
                return false;

            }
            else
            {
                return true;
            }


        }
    }
}
