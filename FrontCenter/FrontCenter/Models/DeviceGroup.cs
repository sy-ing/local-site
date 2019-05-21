using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 设备组信息
    /// </summary>
    public class DeviceGroup : Base
    {

        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }


        /// <summary>
        /// 设备组名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "GName")]
        public string GName { get; set; }


        /// <summary>
        /// 1-正常 2-同步
        /// </summary>
        [Display(Name = "Type")]
        public int Type { get; set; }

        /// <summary>
        /// 显示屏规格
        /// </summary>
        [Display(Name = "ScreenInfoCode")]
        [StringLength(50)]
        public string ScreenInfoCode { get; set; }

    }
}
