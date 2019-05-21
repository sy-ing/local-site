using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 设备-设备组 关联信息
    /// </summary>
    public class DeviceToGroup : Base
    {



        /// <summary>
        /// 设备ID
        /// </summary>
        [Display(Name = "DeviceCode")]
        [StringLength(50)]
        public string DeviceCode { get; set; }


        /// <summary>
        /// 设备组ID
        /// </summary>
        [Display(Name = "GroupCode")]
        [StringLength(50)]
        public string GroupCode { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        [StringLength(50)]
        public string MallCode { get; set; }


    }
}
