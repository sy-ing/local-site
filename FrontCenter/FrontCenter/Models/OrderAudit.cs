using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 订单审核表
    /// </summary>
    public class OrderAudit : Base
    {
        /// <summary>
        /// 订单编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "OrderCode")]
        public string OrderCode { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [StringLength(255)]
        [Display(Name = "OperUser")]
        public string OperUser { get; set; }

        /// <summary>
        /// 审核流程排序
        /// </summary>
        [Display(Name = "AuditOrder")]
        public int AuditOrder { get; set; }

        /// <summary>
        /// 审核状态 0未审核 1已通过 2已拒绝
        /// </summary>
        [Display(Name = "AuditStatus")]
        public int AuditStatus { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        [Display(Name = "AuditOpinion")]
        [StringLength(2000)]
        public string AuditOpinion { get; set; }
    }
}
