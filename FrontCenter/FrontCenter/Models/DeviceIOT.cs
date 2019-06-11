using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class DeviceIOT : Base
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "DeviceCode")]
        public string DeviceCode { get; set; }

        /// <summary>
        /// IOT订阅名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }


        /// <summary>
        /// IOT订阅Key
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Key")]
        public string Key { get; set; }
    }
}
