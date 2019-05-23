using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 店铺业态
    /// </summary>
    public class ShopFormat : Base
    {
        /// <summary>
        /// 业态名称
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 业态名称（英文）
        /// </summary>
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "NameEn")]
        public string NameEn { get; set; }

        /// <summary>
        /// 业态颜色
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Color")]
        public string Color { get; set; }

        /// <summary>
        /// 图标文件
        /// </summary>
        [Display(Name = "IconFile")]
        [StringLength(50)]
        public string IconFile { get; set; }

        /// <summary>
        /// 父编码
        /// </summary>
        [Display(Name = "ParentCode")]
        [StringLength(50)]
        public string ParentCode { get; set; }

        /// <summary>
        /// 是否被删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }

        /// <summary>
        /// 所属商场
        /// </summary>
        [Display(Name = "MallCode")]
        [StringLength(50)]
        public string MallCode { get; set; }


    }
}
