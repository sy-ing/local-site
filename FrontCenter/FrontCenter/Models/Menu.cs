using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class Menu : Base
    {


        /// <summary>
        /// 菜单的中文显示
        /// </summary>
        [StringLength(100)]
        [Display(Name = "TextCH")]
        public string TextCH { get; set; }

        /// <summary>
        /// 菜单的英文名称
        /// </summary>
        [StringLength(100)]
        [Display(Name = "TextEN")]
        public string TextEN { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        [Display(Name = "ParentCode")]
        [StringLength(50)]
        public string ParentCode { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "Order")]
        public int? Order { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        [Display(Name = "PermissionCode")]
        [StringLength(50)]
        public string PermissionCode { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        [Display(Name = "Enable")]
        public bool Enable { get; set; }

        /// <summary>
        /// 菜单的路径
        /// </summary>
        [StringLength(2500)]
        [Display(Name = "Href")]
        public string Href { get; set; }

        /// <summary>
        /// 图标路径
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Icon")]
        public string Icon { get; set; }


    }
}
