using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 排期订单素材表
    /// </summary>
    public class ScheduleMaterial : Base
    {
        /// <summary>
        /// 排期订单编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ScheduleCode")]
        public string ScheduleCode { get; set; }

        /// <summary>
        /// 素材编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "MaterialCode")]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "Order")]
        public int Order { get; set; }
    }
}
