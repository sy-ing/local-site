using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 屏幕信息（分辨率 横竖）
    /// </summary>
    public class ScreenInfo : Base
    {



        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(50)]
        [Display(Name = "SName")]
        public string SName { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

    }
}
