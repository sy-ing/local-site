using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class Live : Base
    {

        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }


        /// <summary>
        /// 直播名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 直播源路径
        /// </summary>
        [Display(Name = "Url")]
        public string Url { get; set; }

        /// <summary>
        /// 分辨率编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ScreenCode")]
        public string ScreenCode { get; set; }

        /// <summary>
        /// 已删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }

        /// <summary>
        /// 被使用
        /// </summary>
        [Display(Name = "BeingUsed")]
        public bool BeingUsed { get; set; }
    }
}
