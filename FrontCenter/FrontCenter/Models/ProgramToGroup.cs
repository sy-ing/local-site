using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 节目-节目组 关系表
    /// </summary>
    public class ProgramToGroup : Base
    {


        /// <summary>
        /// 节目ID
        /// </summary>
        [Display(Name = "ProgramCode")]
        [StringLength(50)]
        public string ProgramCode { get; set; }

        /// <summary>
        /// 节目组ID
        /// </summary>
        [Display(Name = "GroupID")]
        [StringLength(50)]
        public string GroupCode { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "Order")]
        public int Order { get; set; }


        /// <summary>
        /// 切换风格
        /// </summary>
        [Display(Name = "SwitchStyle")]
        public int SwitchStyle { get; set; }


        /// <summary>
        /// 时长
        /// </summary>
        [Display(Name = "PlayLength")]
        public int PlayLength { get; set; }

    }
}
