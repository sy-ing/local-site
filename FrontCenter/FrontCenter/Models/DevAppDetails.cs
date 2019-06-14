using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class DevAppDetails:Base
    {
      

        /// <summary>
        /// 设备ID
        /// </summary>
        [Display(Name = "DevCode")]
        public string DevCode { get; set; }



        /// <summary>
        /// 应用编码
        /// </summary>
        [StringLength(100)]
        [Display(Name = "AppID")]
        public string AppID { get; set; }


        /// <summary>
        /// 应用版本
        /// </summary>
        [StringLength(255)]
        [Display(Name = "AppVersion")]
        public string AppVersion { get; set; }

        /// <summary>
        /// 容器版本
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ContainerVersion")]
        public string ContainerVersion { get; set; }




    }
}
