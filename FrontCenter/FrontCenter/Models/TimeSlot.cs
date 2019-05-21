using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 时间段表  基础数据  默认分24个时间段
    /// </summary>
    public class TimeSlot : Base
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "BeginTimeSlot")]
        [StringLength(50)]
        public string BeginTimeSlot { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "EndTimeSlot")]
        [StringLength(50)]
        public string EndTimeSlot { get; set; }
    }
}
