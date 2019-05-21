using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class AppDev : Base
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        [Display(Name = "AppCode")]
        public string AppCode { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "DevCode")]
        public string DevCode { get; set; }

        /// <summary>
        /// 底层应用
        /// </summary>
        [Display(Name = "Default")]
        public bool Default { get; set; }
    }
}
