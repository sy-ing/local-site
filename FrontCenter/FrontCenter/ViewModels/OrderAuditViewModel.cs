using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class OrderAuditViewModel
    {
    }
    public class Input_OrderAuditAdd
    {
        /// <summary>
        /// 订单Code
        /// </summary>
        [Display(Name = "OrderCode")]
        public string OrderCode { get; set; }

        /// <summary>
        /// 订单处理Code
        /// </summary>
        [Display(Name = "OrderAuditCode")]
        public string OrderAuditCode { get; set; }

        /// <summary>
        /// 订单类型 1排期  2节目素材
        /// </summary>
        [Display(Name = "AuditType")]
        public int AuditType { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 审核状态 0未审核 1已通过 2已拒绝
        /// </summary>
        [Display(Name = "AuditStatus")]
        public int AuditStatus { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        [Display(Name = "AuditOpinion")]
        public string AuditOpinion { get; set; }
    }

    public class Input_OrderAuditGet
    {
        /// <summary>
        /// 订单Code
        /// </summary>
        [Display(Name = "OrderCode")]
        public string OrderCode { get; set; }

        /// <summary>
        /// 订单处理Code
        /// </summary>
        [Display(Name = "OrderAuditCode")]
        public string OrderAuditCode { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }

    public class Input_OrderAuditGetList : Pagination
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        [Display(Name = "UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 审核状态 0未审核 1审核中 2通过 3拒绝 4下架 5排期内 6未开始  7已结束
        /// </summary>
        [Display(Name = "AuditStatus")]
        public int AuditStatus { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "BeginTime")]
        public string BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "EndTime")]
        public string EndTime { get; set; }

        /// <summary>
        /// 搜索内容
        /// </summary>
        [Display(Name = "SearchName")]
        public string SearchName { get; set; }

    }
}
