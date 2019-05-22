using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class AppClassNew : Base
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ClassName")]
        public string ClassName { get; set; }

        /// <summary>
        /// 父类ID
        /// </summary>
        [Display(Name = "ParentCode")]
        public string ParentCode { get; set; }


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
