using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class TimeAxisViewModel
    {
    }


    public class Input_TimeAxis
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 开始时间段
        /// </summary>
        [Display(Name = "BeginAxis")]
        public int? BeginAxis { get; set; }

        /// <summary>
        /// 结束时间段
        /// </summary>
        [Display(Name = "EndAxis")]
        public int? EndAxis { get; set; }

        /// <summary>
        /// 时间段编码
        /// </summary>
        [Display(Name = "TimeAxisCode")]
        public string TimeAxisCode { get; set; }
    }

    public class Input_TimeRelate
    {
        /// <summary>
        /// 时间段列表
        /// </summary>
        [Display(Name = "TimeRelateList")]
        public List<TimeRelateModel> TimeRelateList { get; set; }
    }

    public class TimeRelateModel
    {
        /// <summary>
        /// 关联编码
        /// </summary>
        [Display(Name = "TimeRelateCode")]
        public string TimeRelateCode { get; set; }

        /// <summary>
        /// 开放时间
        /// </summary>
        [Display(Name = "OpenTime")]
        public int OpenTime { get; set; }
    }
}
