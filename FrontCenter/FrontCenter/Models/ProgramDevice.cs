using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 节目组 设备组 关联表
    /// </summary>
    public class ProgramDevice : Base
    {


        /// <summary>
        /// 节目组编码
        /// </summary>
        [Display(Name = "ProgramGrounpCode")]
        [StringLength(50)]
        public string ProgramGrounpCode { get; set; }

        /// <summary>
        /// 设备组编码
        /// </summary>
        [Display(Name = "DeviceGrounpCode")]
        [StringLength(50)]
        public string DeviceGrounpCode { get; set; }




    }
}
