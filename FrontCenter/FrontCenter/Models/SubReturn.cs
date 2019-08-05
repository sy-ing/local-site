using FrontCenter.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class SubReturn
    {
        /// <summary>
        /// 服务器当前连接
        /// </summary>
        public ServerMqttClient mqttClient { get; set; }

        /// <summary>
        /// 服务器IOT信息
        /// </summary>
        public ServerIOT serveriot { get; set; }
    }
}
