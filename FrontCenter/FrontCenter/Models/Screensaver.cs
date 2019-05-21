using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class Screensaver : Base
    {
        [StringLength(50)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 时长（单位为秒）
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// 屏保提示图标设置
        /// </summary>
        public int? ScreenType { get; set; }
    }
}
