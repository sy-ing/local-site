using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class StoreNews : Base
    {

        /// <summary>
        /// 店铺编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ShopCode")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 店铺消息
        /// </summary>
        [Display(Name = "News")]
        [StringLength(2000)]
        public string News { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        [Display(Name = "ValidityPeriod")]
        public DateTime ValidityPeriod { get; set; }

        /// <summary> 
        /// 审核状态 1-待审核  2-审核通过 3审核拒绝  4-下架
        /// </summary>
        [Display(Name = "AduitStatus")]
        public int AduitStatus { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        [Display(Name = "Reason")]
        [StringLength(2000)]
        public string Reason { get; set; }

        /// <summary>
        /// 操作者编码
        /// </summary>
        [Display(Name = "MgrCode")]
        [StringLength(50)]
        public string MgrCode { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [Display(Name = "PlacingNum")]
        [StringLength(50)]
        public string PlacingNum { get; set; }
    }
}
