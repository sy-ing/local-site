using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class DevAppOnline : Base
    {


        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "DeviceCode")]
        public string DeviceCode { get; set; }



        /// <summary>
        /// 应用版本
        /// </summary>
        [StringLength(255)]
        [Display(Name = "AppVersion")]
        public string AppVersion { get; set; }


        /// <summary>
        /// 容器版本号
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ContainerVersion")]
        public string ContainerVersion { get; set; }


        /// <summary>
        /// 应用版本
        /// </summary>
        [StringLength(255)]
        [Display(Name = "AppName")]
        public string AppName { get; set; }


    }
}
