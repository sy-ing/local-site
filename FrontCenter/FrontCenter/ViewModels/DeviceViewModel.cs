using FrontCenter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class DeviceViewModel
    {
    }


    public class Input_GetDGList : Pagination
    {
        public string UnionID { get; set; }
    }


    public class Input_GetDCList
    {
        public string UnionID { get; set; }

        public string FloorCode { get; set; }
    }
    public class DevCommand
    {
        /// <summary>
        /// 命令类型
        /// </summary>
        [Display(Name = "Type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// 心跳返回
    /// </summary>
    public class Output_DeviceHeartbeat : DevCommand
    {

        /// <summary>
        /// 参数
        /// </summary>
        public string Parameter { get; set; }
    }
    /// <summary>
    /// 心跳包
    /// </summary>
    public class Input_Heartbeat
    {
        [StringLength(50)]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [StringLength(255)]
        [Display(Name = "AppName")]
        public string AppName { get; set; }

        [StringLength(255)]
        [Display(Name = "AppNameCH")]
        public string AppNameCH { get; set; }



        /// <summary>
        /// 应用版本
        /// </summary>
        [StringLength(255)]
        [Display(Name = "AppVersion")]
        public string AppVersion { get; set; }

        /// <summary>
        /// 容器版本
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ContainerVersion")]
        public string ContainerVersion { get; set; }
    }
    public class Input_SetSynStatus : Input_CdnBase
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


    public class Input_ScreenOper
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 状态 
        /// </summary>
        [Display(Name = "Type")]
        public bool Type { get; set; }
    }

    public class Input_SetDevStore
    {
        public string DevCode { get; set; }

        public List<string> ShopCode { get; set; }
    }

    public class Input_SetDevShop : Input_CdnIdentity
    {
        public string DevCode { get; set; }

        public List<ShopToDevice> ShopRelateList { get; set; }
    }

    public class Input_RemoveDevData : Input_CdnIdentity
    {
        public string DevCode { get; set; }
    }

    public class Input_GetDevShops
    {
        public string DevCode { get; set; }
    }

    public class Input_GetDevProg
    {
        public string DevCode { get; set; }
    }

    public class Input_GetCloudProg
    {
        public string DevCode { get; set; }

        /// <summary>
        /// 时间段
        /// </summary>
        [Display(Name = "TimeSolt")]
        public string TimeSolt { get; set; }

        [Display(Name = "EffcTime")]
        public DateTime EffcTime { get; set; }
    }


    public class Output_DG
    {
        /// <summary>
        /// 设备组编码
        /// </summary>
        [Key]
        public string Code { get; set; }

        /// <summary>
        /// 设备组名称
        /// </summary>
        public string GName { get; set; }
        /// <summary>
        /// 屏幕属性描述
        /// </summary>
        public string SName { get; set; }
        /// <summary>
        /// 包含设备数量
        /// </summary>
        public int DevCount { get; set; }
    }

    /// <summary>
    /// 添加设备
    /// </summary>
    public class Input_Device
    {

        [StringLength(50)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }


        /// <summary>
        /// IP地址
        /// </summary>
        [StringLength(50)]
        //   [RegularExpression(@"/^(([1-9]|([1-9]\d)|(1\d\d)|(2([0-4]\d|5[0-5])))\.)(([1-9]|([1-9]\d)|(1\d\d)|(2([0-4]\d|5[0-5])))\.){2}([1-9]|([1-9]\d)|(1\d\d)|(2([0-4]\d|5[0-5])))$/")]
        [Display(Name = "IP")]
        public string IP { get; set; }

        /// <summary>
        /// MAC地址
        /// </summary>
        [StringLength(50)]
        [Display(Name = "MAC")]
        public string MAC { get; set; }


        /// <summary>
        /// 屏幕信息（分辨率-横竖屏）
        /// </summary>

        [Display(Name = "ScreenInfo")]
        public string ScreenInfo { get; set; }

        /// <summary>
        /// 楼栋
        /// </summary>
        [Display(Name = "Building")]
        public string Building { get; set; }


        /// <summary>
        /// 楼层
        /// </summary>
        [Display(Name = "Floor")]
        public string Floor { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        [StringLength(255)]
        [Display(Name = "DevNum")]
        public string DevNum { get; set; }


        ///// <summary>
        ///// 设备标识
        ///// </summary>
        //[StringLength(255)]
        //[Display(Name = "Mark")]
        //public string Mark { get; set; }

        /// <summary>
        /// 系统类型
        /// </summary>
        [StringLength(255)]
        [Display(Name = "SystemType")]
        public string SystemType { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [StringLength(255)]
        [Display(Name = "DeviceType")]
        public string DeviceType { get; set; }



        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Version")]
        public string Version { get; set; }
    }

    public class Input_GetDictListByName
    {
        public string Name { get; set; }
    }
    public class IOTReturn
    {
        /// <summary>
        /// tcp地址
        /// </summary>
        [StringLength(50)]
        [Display(Name = "tcpEndpoint")]
        public string tcpEndpoint { get; set; }
        /// <summary>
        /// ssl地址
        /// </summary>
        [StringLength(50)]
        [Display(Name = "sslEndpoint")]
        public string sslEndpoint { get; set; }


        /// <summary>
        /// IOT订阅用户名
        /// </summary>
        [StringLength(255)]
        [Display(Name = "username")]
        public string username { get; set; }


        /// <summary>
        /// IOT订阅Key
        /// </summary>
        [StringLength(255)]
        [Display(Name = "key")]
        public string key { get; set; }
    }
}
