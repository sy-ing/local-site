using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class AuditProcessViewModels
    {
    }


    public class Input_APAdd
    {
        /// <summary>
        /// 操作人
        /// </summary>
        [StringLength(255)]
        [Display(Name = "OperUser")]
        public string OperUser { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        ///// <summary>
        ///// 排序
        ///// </summary>
        //[Display(Name = "Order")]
        //public int Order { get; set; }

        /// <summary>
        /// 模块类型(1、排期订单审核  2素材审核)
        /// </summary>
        [Display(Name = "ModuleType")]
        public int? ModuleType { get; set; }
    }

    public class Input_APGetList
    {
        /// <summary>
        /// 模块类型(1、排期订单审核  2素材审核)
        /// </summary>
        [Display(Name = "ModuleType")]
        public int? ModuleType { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }



    public class Input_APDel
    {
        /// <summary>
        /// 记录编码
        /// </summary>
        public List<string> Code { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }

    public class Input_APOrderEdit
    {
        /// <summary>
        /// 记录编码
        /// </summary>
        public string Code { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "Order")]
        public int? Order { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }
}
