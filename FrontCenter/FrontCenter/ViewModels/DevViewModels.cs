using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class DevViewModels
    {
    }

    public class Input_GetDevList : Pagination
    {

        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        [Display(Name = "SearchKey")]
        public string SearchKey { get; set; }


        /// <summary>
        /// 屏幕属性ID
        /// </summary>
        [Display(Name = "ScreenCode")]
        public string ScreenCode { get; set; }



        /// <summary>
        /// 楼层ID
        /// </summary>
        [Display(Name = "FloorCode")]
        public string FloorCode { get; set; }

        /// <summary>
        /// 设备状态 -1 不限 0 不在线 1在线
        /// </summary>
        [Display(Name = "DevStatus")]
        public int? DevStatus { get; set; }

        /// <summary>
        /// 前端状态 -1 不限 0 不在线 1在线
        /// </summary>
        [Display(Name = "FontStatus")]
        public int? FontStatus { get; set; }


    }

    public class Input_GetDevInfo
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }
    }

    /*
    public class Input_SetSynStatus
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 状态 0 非同步屏 1 同步屏
        /// </summary>
        [Display(Name = "Status")]
        public int Status { get; set; }


    }
    */
    public class Input_SetOperable
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 状态 0 非同步屏 1 同步屏
        /// </summary>
        [Display(Name = "Status")]
        public int Status { get; set; }


    }
    public class Input_GetAppDev : Pagination
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }
    public class Input_GetDevByGroup
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 设备组编码
        /// </summary>
        [Display(Name = "GroupCode")]
        public string GroupCode { get; set; }

        /// <summary>
        /// 分辨率编码
        /// </summary>
        [Display(Name = "ScreenCode")]
        public string ScreenCode { get; set; }


        /// <summary>
        /// 状态 0 非同步屏 1 同步屏
        /// </summary>
        [Display(Name = "IsSyn")]
        public bool IsSyn { get; set; }


    }


    //public class Input_SetDevSyn
    //{
    //    /// <summary>
    //    /// 设备编码
    //    /// </summary>
    //    [Display(Name = "DevCode")]
    //    public string DevCode { get; set; }


    //    /// <summary>
    //    /// 状态 0 非同步屏 1 同步屏
    //    /// </summary>
    //    [Display(Name = "IsSyn")]
    //    public int IsSyn { get; set; }
    //}

    public class Input_GetTSList
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "DevCode")]
        public string DevCode { get; set; }

        [Display(Name = "EffcTime")]
        public DateTime EffcTime { get; set; }
    }

    public class Input_GetPHList
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "DevCode")]
        public string DevCode { get; set; }

        /// <summary>
        /// 时间段
        /// </summary>
        [Display(Name = "TimeSolt")]
        public string TimeSolt { get; set; }

        [Display(Name = "EffcTime")]
        public DateTime EffcTime { get; set; }
    }

    public class Input_GetLocalList
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "DevCode")]
        public string DevCode { get; set; }


        [Display(Name = "EffcTime")]
        public DateTime EffcTime { get; set; }
    }


    public class Input_GetDevFlow
    {

        /// <summary>
        /// 楼层
        /// </summary>
        [Display(Name = "Floor")]
        public string Floor { get; set; }
    }


    public class Output_DevFlow
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Key]
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [Display(Name = "DevNum")]
        public string DevNum { get; set; }

        /// <summary>
        /// 订单数量
        /// </summary>
        [Display(Name = "ScheduleNum")]
        public int ScheduleNum { get; set; }


        /// <summary>
        /// 横坐标
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Xaxis")]
        public string Xaxis { get; set; }

        /// <summary>
        /// 纵坐标
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Yaxis")]
        public string Yaxis { get; set; }



        /// <summary>
        /// 设备角度
        /// </summary>
        [Display(Name = "Angle")]
        public string Angle { get; set; }

    }

    public class Output_TopDevFlow
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Key]
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [Display(Name = "DevNum")]
        public string DevNum { get; set; }

        /// <summary>
        /// 订单数量
        /// </summary>
        [Display(Name = "ScheduleNum")]
        public int ScheduleNum { get; set; }
    }
}
