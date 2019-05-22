using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 专属节目素材表
    /// </summary>
    public class ProperMaterial : Base
    {
        /// <summary>
        /// 设备店铺关系编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "DeviceShopRelateCode")]
        public string DeviceShopRelateCode { get; set; }

        /// <summary>
        /// 素材编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "MaterialCode")]
        public string MaterialCode { get; set; }
    }
}
