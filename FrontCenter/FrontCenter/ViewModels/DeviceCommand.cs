using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    /// <summary>
    /// 设备命令
    /// </summary>
    public class DeviceCommand
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 最近一次心跳时间
        /// </summary>
        public DateTime DevBreathTime { get; set; }

        /// <summary>
        /// 最近一次心跳时间
        /// </summary>
        public DateTime AppBreathTime { get; set; }
    }
}
