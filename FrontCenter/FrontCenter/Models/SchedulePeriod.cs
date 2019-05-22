using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 排期时间段表
    /// </summary>
    public class SchedulePeriod : Base
    {
        /// <summary>
        /// 排期订单编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ScheduleCode")]
        public string ScheduleCode { get; set; }

        /// <summary>
        /// 时间段编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "TimeSlotCode")]
        public string TimeSlotCode { get; set; }
    }
}
