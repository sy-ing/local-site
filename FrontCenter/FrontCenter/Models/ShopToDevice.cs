using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 设备店铺关系表
    /// </summary>
    public class ShopToDevice : Base
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "DeviceCode")]
        public string DeviceCode { get; set; }

        /// <summary>
        /// 店铺编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ShopCode")]
        public string ShopCode { get; set; }
    }
}
