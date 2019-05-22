using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class ContainerBG : Base
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 分辨率编码
        /// </summary>
        [Display(Name = "ScreenCode")]
        public string ScreenCode { get; set; }

        /// <summary>
        /// 文件编码
        /// </summary>
        [Display(Name = "FileCode")]
        public string FileCode { get; set; }
    }
}
