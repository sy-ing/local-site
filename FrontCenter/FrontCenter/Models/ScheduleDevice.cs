using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 排期订单设备表
    /// </summary>
    public class ScheduleDevice : Base
    {
        /// <summary>
        /// 排期订单编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ScheduleCode")]
        public string ScheduleCode { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "DeviceCode")]
        public string DeviceCode { get; set; }
    }
}
