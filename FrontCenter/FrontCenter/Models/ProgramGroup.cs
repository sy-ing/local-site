using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 节目组信息
    /// </summary>
    public class ProgramGroup : Base
    {


        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 节目组名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "GroupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// 显示屏规格
        /// </summary>
        [Display(Name = "ScreenInfoCode")]
        [StringLength(50)]
        public string ScreenInfoCode { get; set; }




    }
}
