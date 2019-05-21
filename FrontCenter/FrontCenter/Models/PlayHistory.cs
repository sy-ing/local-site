using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class PlayHistory : Base
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "DeviceCode")]
        public string DeviceCode { get; set; }

        /// <summary>
        /// 订单编码
        /// </summary>
        [Display(Name = "ScheduleCode")]
        public string ScheduleCode { get; set; }

        /// <summary>
        /// 订单开始日期
        /// </summary>
        [Display(Name = "ScheduleStart")]
        public DateTime ScheduleStart { get; set; }

        /// <summary>
        /// 订单结束日期
        /// </summary>
        [Display(Name = "ScheduleEnd")]
        public DateTime ScheduleEnd { get; set; }

        /// <summary>
        /// 文件编码 
        /// </summary>
        [Display(Name = "FileCode")]
        public string FileCode { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [Display(Name = "FilePath")]
        public string FilePath { get; set; }

        /// <summary>
        /// 图标编码
        /// </summary>
        [Display(Name = "ShopIcon")]
        public string ShopIcon { get; set; }

        /// <summary>
        /// 图标路径
        /// </summary>
        [Display(Name = "IconPath")]
        public string IconPath { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        [Display(Name = "ShopName")]
        public string ShopName { get; set; }

        /// <summary>
        /// 店铺门牌号
        /// </summary>
        [Display(Name = "ShopHouseNum")]
        public string ShopHouseNum { get; set; }

        /// <summary>
        /// 时间段编码
        /// </summary>
        [Display(Name = "TimeSolt")]
        public string TimeSolt { get; set; }

        /// <summary>
        /// 类型 1-排期订单 2-专属设备
        /// </summary>
        [Display(Name = "Type")]
        public int Type { get; set; }

        /// <summary>
        /// 作用日期
        /// </summary>
        [Display(Name = "EffcDate")]
        public DateTime EffcDate { get; set; }


        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "BeginTimeSlot")]
        public string BeginTimeSlot { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "EndTimeSlot")]
        public string EndTimeSlot { get; set; }

        /// <summary>
        /// 素材排序
        /// </summary>
        [Display(Name = "Order")]
        public int Order { get; set; }
    }
}
