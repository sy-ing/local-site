using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class Building : Base
    {
        /// <summary>
        /// 楼栋名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 楼栋名称(英文)
        /// </summary>
        [StringLength(255)]
        [Display(Name = "NameEn")]
        public string NameEn { get; set; }

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
