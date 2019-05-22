using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 排期日期表
    /// </summary>
    public class ScheduleDate : Base
    {
        /// <summary>
        /// 排期订单编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ScheduleCode")]
        public string ScheduleCode { get; set; }

        /// <summary>
        /// 排期日期
        /// </summary>
        [Display(Name = "ScheduleDay")]
        public DateTime ScheduleDay { get; set; }
    }
}
