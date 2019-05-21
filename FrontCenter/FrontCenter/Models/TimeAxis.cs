using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{    /// <summary>
     /// 时间轴表
     /// </summary>
    public class TimeAxis : Base
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }
}
