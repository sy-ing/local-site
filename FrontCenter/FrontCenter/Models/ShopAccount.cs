using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class ShopAccount : Base
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 所属店铺
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ShopCode")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 微信登录开放编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "UnionID")]
        public string UnionID { get; set; }
    }
}
