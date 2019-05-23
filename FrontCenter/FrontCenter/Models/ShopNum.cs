using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class ShopNum : Base
    {
        /// <summary>
        /// 店铺编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ShopCode")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 店铺随机码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Num")]
        public string Num { get; set; }
    }
}
