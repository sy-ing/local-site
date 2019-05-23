using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class AreaInfo : Base
    {

        /// <summary>
        /// 区域名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "AreaName")]
        public string AreaName { get; set; }



        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "Order")]
        public int Order { get; set; }

        /// <summary>
        /// 是否被删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }

    }
}
