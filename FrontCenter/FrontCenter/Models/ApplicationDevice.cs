using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class ApplicationDevice
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Display(Name = "ID")]
        public int ID { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [Display(Name = "AppID")]
        public int AppID { get; set; }

        /// <summary>
        /// 设备组ID
        /// </summary>
        [Display(Name = "DeviceGrounpID")]
        public int DeviceGrounpID { get; set; }


        /// <summary>
        /// 设备组ID
        /// </summary>
        [Display(Name = "DeviceID")]
        public int DeviceID { get; set; }


        /// <summary>
        /// 发布状态 0 发布中 1 发布成功 2 发布失败
        /// </summary>
        [Display(Name = "State")]
        public int State { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "AddTime")]
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 是否被删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }
    }
}
