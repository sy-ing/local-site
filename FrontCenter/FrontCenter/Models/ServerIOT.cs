using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class ServerIOT : Base
    {
        /// <summary>
        /// 设备地址
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ServerMac")]
        public string ServerMac { get; set; }

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
