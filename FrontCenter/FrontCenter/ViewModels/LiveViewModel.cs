using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class LiveViewModel
    {
    }
    public class Input_LiveAdd
    {

        /// <summary>
        /// 直播名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 直播源路径
        /// </summary>
        [Display(Name = "Url")]
        public string Url { get; set; }

        /// <summary>
        /// 分辨率编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ScreenCode")]
        public string ScreenCode { get; set; }
    }

    public class Input_LiveDel
    {
        public List<string> Code { get; set; }
    }


    public class Input_LiveEdit
    {
        /// <summary>
        /// 直播名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 直播源路径
        /// </summary>
        [Display(Name = "Url")]
        public string Url { get; set; }


        public string Code { get; set; }
    }


    public class Input_GetLiveList : Pagination
    {
        /// <summary>
        /// 直播名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Order")]
        public string Order { get; set; }

        /// <summary>
        /// 分辨率
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ScreenCode")]
        public string ScreenCode { get; set; }
    }
    public class Input_GetLiveInfo
    {
        /// <summary>
        /// 直播编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }

    public class Input_GetDevLiveInfo
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }



    public class Input_LiveCtr
    {
        /// <summary>
        /// 直播编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Code")]
        public string Code { get; set; }


        /// <summary>
        /// 控制命令
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Command")]
        public string Command { get; set; }
    }
}
