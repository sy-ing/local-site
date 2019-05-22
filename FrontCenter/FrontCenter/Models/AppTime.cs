using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class AppTime : Base
    {
        /// <summary>
        /// 应用编码
        /// </summary>
        [Display(Name = "AppCode")]
        public string AppCode { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "TimeSlot")]
        public string TimeSlot { get; set; }
    }
}
