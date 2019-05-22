using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class LiveToDev : Base
    {
        /// <summary>
        /// 直播编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "LiveCode")]
        public string LiveCode { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "DevCode")]
        public string DevCode { get; set; }

        /// <summary>
        /// 已删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }
    }
}
