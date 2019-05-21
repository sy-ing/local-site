using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class Device : Base
    {

        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }


        /// <summary>
        /// IP地址
        /// </summary>
        [StringLength(50)]
        // [RegularExpression(@"/^(([1-9]|([1-9]\d)|(1\d\d)|(2([0-4]\d|5[0-5])))\.)(([1-9]|([1-9]\d)|(1\d\d)|(2([0-4]\d|5[0-5])))\.){2}([1-9]|([1-9]\d)|(1\d\d)|(2([0-4]\d|5[0-5])))$/")]
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
        [StringLength(50)]
        public string ScreenInfo { get; set; }



        /// <summary>
        /// 楼栋
        /// </summary>
        [Display(Name = "Building")]
        [StringLength(50)]
        public string Building { get; set; }


        /// <summary>
        /// 楼层
        /// </summary>
        [Display(Name = "Floor")]
        [StringLength(50)]
        public string Floor { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        [StringLength(255)]
        [Display(Name = "DevNum")]
        public string DevNum { get; set; }



        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Position")]
        public string Position { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Version")]
        public string Version { get; set; }

        /// <summary>
        /// 关机时间
        /// </summary>
        //[DataType(DataType.Time)]
        //[DisplayFormat(DataFormatString = "{0:HH-mm}", ApplyFormatInEditMode = true)]
        [StringLength(50)]
        [Display(Name = "ShutdownTime")]
        public String ShutdownTime { get; set; }

        /// <summary>
        /// 截图（地址）
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "ScreenshotSrc")]
        public string ScreenshotSrc { get; set; }

        /// <summary>
        /// 设备在线
        /// </summary>
        [Display(Name = "DeviceOnline")]
        public bool DeviceOnline { get; set; }

        /// <summary>
        /// 前端在线
        /// </summary>
        [Display(Name = "FrontOnline")]
        public bool FrontOnline { get; set; }

        [StringLength(255)]
        [Display(Name = "Mark")]
        public string Mark { get; set; }



        /// <summary>
        /// 是否被删
        /// </summary>
        [Display(Name = "IsDelete")]
        public bool IsDelete { get; set; }


        /// <summary>
        /// 是否显示
        /// </summary>
        [Display(Name = "IsShow")]
        public bool IsShow { get; set; }

        /// <summary>
        /// 是否同步屏
        /// </summary>
        [Display(Name = "IsSyn")]
        public bool IsSyn { get; set; }

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
        /// 可操作（触摸屏）
        /// </summary>
        [Display(Name = "Operable")]
        public bool Operable { get; set; }

        /// <summary>
        /// 最后响应时间
        /// </summary>
        [Display(Name = "LastLinkTime")]
        public DateTime LastLinkTime { get; set; }


        [Timestamp]
        public byte[] RowVersion { get; set; }


    }
}
