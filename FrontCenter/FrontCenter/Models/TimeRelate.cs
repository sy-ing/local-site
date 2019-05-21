using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 时间段-时间轴关系表
    /// </summary>
    public class TimeRelate : Base
    {
        /// <summary>
        /// 时间轴编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "TimeAxisCode")]
        public string TimeAxisCode { get; set; }

        /// <summary>
        /// 时间段编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "TimeSlotCode")]
        public string TimeSlotCode { get; set; }

        /// <summary>
        /// 开放时长
        /// </summary>
        [Display(Name = "OpenTime")]
        public int OpenTime { get; set; }
    }
}
