using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class SubtitleToDeviceGroup : Base
    {
        /// <summary>
        /// 已删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }

        /// <summary>
        /// 字幕编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "SubtitleCode")]
        public string SubtitleCode { get; set; }


        /// <summary>
        /// 设备组编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "GroupCode")]
        public string GroupCode { get; set; }
    }
}
