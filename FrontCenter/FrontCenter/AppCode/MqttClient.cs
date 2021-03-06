﻿
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    public class MqttClient
    {
        private QMLog log = new QMLog();
       // private Class_Log log = new Class_Log(); //日志记录文件
        //private static MqttClient Instance = null;
        private static IMqttClient mqttClient = null;
        private  string MqttServer = "malldevice.mqtt.iot.gz.baidubce.com";
        private  int MqttPort = 1883;
        private  string ClientId = "";
        private  string Username = "";
        private  string Password = "";
      //  App app = ((App)System.Windows.Application.Current);
        public delegate void WebSocketReceiveDelegate(string type, string message);
        public static event WebSocketReceiveDelegate WebSocketReceiveEvent;
        public MqttClient(string _MqttServer, int _MqttPort, string _ClientId, string _Username, string _Password)
        {
            this.MqttServer = _MqttServer;
            this.MqttPort = _MqttPort;
            this.ClientId = _ClientId;
            this.Username = _Username;
            this.Password = _Password;

            //log.WriteLogFile("33333333" + mqttClient.IsConnected.ToString(), "444444");
        }

        public async void InitAsync()
        {
            if (mqttClient == null)
            {
                mqttClient = new MqttFactory().CreateMqttClient();
                //if (!mqttClient.IsConnected) {
                await mqttClientConnectAsync();
                // }
                mqttClient.Connected += MqttClient_Connected;
                mqttClient.Disconnected += MqttClient_Disconnected;

                Sub();
            }
        }

        //public static MqttClient GetMqttClientInstance()
        //{
        //    if (Instance == null)
        //        Instance = new MqttClient();
        //    return Instance;
        //}

        private async void MqttClient_Disconnected(object sender, MqttClientDisconnectedEventArgs e)
        {
            mqttClientConnectAsync();
            //throw new NotImplementedException();
        }

        private void MqttClient_Connected(object sender, MqttClientConnectedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public async Task mqttClientConnectAsync()
        {
            var options = new MqttClientOptions
            {
                ClientId = ClientId,
                Credentials = new MqttClientCredentials
                {
                    Username = Username,
                    Password = Password,
                },
                ChannelOptions = new MqttClientTcpOptions
                {
                    Server = MqttServer,
                    Port = MqttPort,
                }
            };
            await mqttClient.ConnectAsync(options);

        }




        private void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            //   Action<string> BookAction = new Action<string>(BookR);
            Action UploadApp = new Action(() =>
            {
                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
                Dictionary<string, object> dicDesired = JsonConvert.DeserializeObject<Dictionary<string, object>>(dic["desired"].ToString());
                if (dicDesired != null && dicDesired.Count > 0)
                {
                    MessageReceived(dicDesired["MsgTemplate"].ToString());
                }
            });
            UploadApp.BeginInvoke(null, null);
        }

        public async Task PublishAsync(string ClientId, string reportedStr)
        {
            try
            {
                // var jsonstr = "{\"reported\": {\"AppName\": \"汉千目字\"}}";
                var jsonstr = "{\"reported\": " + reportedStr + "}";
                var appMsg = new MqttApplicationMessage
                {
                    Topic = "$baidu/iot/shadow/" + ClientId + "/update",
                    Payload = Encoding.UTF8.GetBytes(jsonstr),
                    QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
                    Retain = false
                };
                await mqttClient.PublishAsync(appMsg);
            }
            catch (Exception e)
            {
                log.WriteLogToFile(e.Message, "mqtt");
            }
        }

        public async void Sub()
        {
            try
            {
                mqttClient.ApplicationMessageReceived += MqttClient_ApplicationMessageReceived;
                var topic = "$baidu/iot/shadow/" + ClientId + "/update/accepted";
                var topic1 = "$baidu/iot/shadow/" + ClientId + "/update/rejected";
                await mqttClient.SubscribeAsync(new List<TopicFilter> {
                new TopicFilter(topic, MqttQualityOfServiceLevel.AtMostOnce),
                 new TopicFilter(topic1, MqttQualityOfServiceLevel.AtMostOnce)
                });
            }
            catch (Exception e)
            {
                log.WriteLogToFile(e.Message, "mgtt");
            }

        }

        async void MessageReceived(string msg)
        {
            try
            {
                log.WriteLogToFile(msg, "WebSocketLog");

                string type = "";
                Dictionary<string, Object> dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Object>>(msg.ToLower());
                if (dic["content"] != null && dic["senderid"] != null)
                {
                    Dictionary<string, Object> Commands = JsonConvert.DeserializeObject<Dictionary<string, Object>>(dic["content"].ToString());

                    if (!string.IsNullOrEmpty(Commands["type"].ToString()))
                    {
                        type = Commands["type"].ToString();
                    }

                    switch (type.Trim())
                    {
                        case "dataupdate":
                            log.WriteLogToFile("云端数据更新", "WebSocketLog");
                            if (!string.IsNullOrEmpty(Commands["modulename"].ToString()))
                            {
                                Pull pull = new Pull();
                                switch (Commands["modulename"].ToString().Trim())
                                {
                                    case "app":
                                        await pull.PullAppData();
                                        break;
                                    case "dev":
                                        await pull.PullDevData();
                                        break;
                                    case "file":
                                        await pull.PullFileData();
                                        break;
                                    case "init":
                                        await pull.PullInitData();
                                        break;
                                    case "prog":
                                        await pull.PullProgramData();
                                        break;
                                    case "review":
                                        await pull.PullReviewData();
                                        break;
                                    case "shopinfo":
                                        await pull.PullShopInfoData();
                                        break;
                                    case "system":
                                        await pull.PullSystemData();
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case "devicecommand":
                            log.WriteLogToFile("设备命令", "WebSocketLog");
                            if (!string.IsNullOrEmpty(Commands["devicecode"].ToString()) && string.IsNullOrEmpty(Commands["cmdstr"].ToString()) && string.IsNullOrEmpty(Commands["msgtype"].ToString()))
                            {
                                var devicecode = Commands["devicecode"].ToString();
                                var cmdstr = Commands["cmdstr"].ToString();
                                var msgtype = Commands["msgtype"].ToString();
                                MsgTemplate msgTemplate = new MsgTemplate();
                                msgTemplate.SenderID = Method.ServerAddr;
                                msgTemplate.MessageType = msgtype;
                                msgTemplate.Content = cmdstr;
                                msgTemplate.ReceiverID = devicecode;
                                await  Method.SendMsgAsync(msgTemplate);

                            }
                            break;
                        default:
                            break;
                    }
        
          
                }
                if (WebSocketReceiveEvent != null)
                {
                    WebSocketReceiveEvent(type, "");
                }
            }
            catch (Exception ex)
            {
                log.WriteLogToFile(ex.ToString(), "WebSocketLog");

            }
        }
        //void MessageReceived(string msg)
        //{
        //    try
        //    {
        //        log.WriteLogToFile(msg, "WebSocketLog");

        //        string type = "";
        //        Dictionary<string, Object> dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Object>>(msg.ToLower());
        //        if (dic["content"] != null && dic["senderid"] != null)
        //        {
        //            Dictionary<string, Object> Commands = JsonConvert.DeserializeObject<Dictionary<string, Object>>(dic["content"].ToString());


        //        }
        //        if (WebSocketReceiveEvent != null)
        //        {
        //            WebSocketReceiveEvent(type, "");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.WriteLogToFile(ex.ToString(), "WebSocketLog");

        //    }
        //}

    }
}
