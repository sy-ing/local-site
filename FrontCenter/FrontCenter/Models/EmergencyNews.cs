using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class EmergencyNews : Base
    {

        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 文件编码
        /// </summary>
        [Display(Name = "FileCode")]
        [StringLength(50)]
        public string FileCode { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [Display(Name = "FileName")]
        [StringLength(255)]
        public string FileName { get; set; }

        /// <summary>
        /// 插播时长
        /// </summary>
        [Display(Name = "Duration")]
        public int Duration { get; set; }

        /// <summary>
        /// 1正常  2下架
        /// </summary>
        [Display(Name = "Status")]
        public int Status { get; set; }

        /// <summary>
        /// 支持的屏幕
        /// </summary>
        [Display(Name = "ScreenInfo")]
        [StringLength(50)]
        public string ScreenInfo { get; set; }
    }
}
