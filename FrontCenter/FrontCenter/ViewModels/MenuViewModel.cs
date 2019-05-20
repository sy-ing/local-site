using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class Input_MenuAccount
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

    }

    public class MenuViewModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Required]
        [Display(Name = "ID")]
        public int ID { get; set; }

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
        [Display(Name = "ParentID")]
        public int? ParentID { get; set; }


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

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "Order")]
        public int? Order { get; set; }



    }
}
