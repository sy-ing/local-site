using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class LogViewModel
    {
    }


    /// <summary>
    /// 店铺查询条件
    /// </summary>
    public class Input_LogQuery
    {
        /// <summary>
        /// 关键字
        /// </summary>
        [Display(Name = "Keywords")]
        public string Keywords { get; set; }

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
    }

    public class Input_LogPageQuery : Pagination
    {
        /// <summary>
        /// 关键字
        /// </summary>
        [Display(Name = "Keywords")]
        public string Keywords { get; set; }

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
        /// 结束时间
        /// </summary>
        [Display(Name = "Type")]
        public int? Type { get; set; }
    }
}
