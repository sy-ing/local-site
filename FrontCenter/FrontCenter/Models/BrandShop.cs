using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class BrandShop : Base
    {

        /// <summary>
        /// 品牌编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "BrandCode")]
        public string BrandCode { get; set; }

        /// <summary>
        /// 店铺编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ShopCode")]
        public string ShopCode { get; set; }



        /// <summary>
        /// 被删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }
    }
}
