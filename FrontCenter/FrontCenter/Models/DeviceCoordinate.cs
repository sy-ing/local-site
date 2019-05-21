using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class DeviceCoordinate : Base
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "DevCode")]
        [StringLength(50)]
        public string DevCode { get; set; }


        /// <summary>
        /// 横坐标
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Xaxis")]
        public string Xaxis { get; set; }

        /// <summary>
        /// 纵坐标
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Yaxis")]
        public string Yaxis { get; set; }



        /// <summary>
        /// 设备角度
        /// </summary>
        [Display(Name = "Angle")]
        [StringLength(50)]
        public string Angle { get; set; }


        /// <summary>
        /// 设备区域
        /// </summary>
        [Display(Name = "AreaCode")]
        [StringLength(50)]
        public string AreaCode { get; set; }
    }

}
