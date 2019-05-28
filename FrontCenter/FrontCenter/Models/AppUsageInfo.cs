using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 应用使用信息
    /// </summary>
    public class AppUsageInfo:Base
    {
      
        /// <summary>
        /// 设备编号
        /// </summary>
        [Display(Name = "DevID")]
        public int DevID { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        [Display(Name = "AppID")]
        public int AppID { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        [Display(Name = "LaunchTime")]
        public string LaunchTime { get; set; }


        /// <summary>
        /// 下线时间
        /// </summary>
        [Display(Name = "ExpiryDate")]
        public string ExpiryDate { get; set; }




        /// <summary>
        /// 是否被删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }

    }
}
