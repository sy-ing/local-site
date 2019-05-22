using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 排期订单表
    /// </summary>
    public class ScheduleOrder : Base
    {
        /// <summary>
        /// 店铺编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ShopCode")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [Display(Name = "PlacingNum")]
        [StringLength(200)]
        public string PlacingNum { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "Info")]
        [StringLength(2000)]
        public string Info { get; set; }

        /// <summary>
        /// 审核状态  0-待审核 1-审核中  2-审核通过 3审核拒绝  4-下架
        /// </summary>
        [Display(Name = "Status")]
        public int Status { get; set; }

        /// <summary>
        /// 分辨率编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ScreenCode")]
        public string ScreenCode { get; set; }

        /// <summary>
        /// 小程序提交的FormID，用于发送通知消息
        /// </summary>
        [Display(Name = "FormID")]
        [StringLength(200)]
        public string FormID { get; set; }
    }
}
