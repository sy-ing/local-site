using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 节目信息
    /// </summary>
    public class Programs : Base
    {

        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 节目名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ProgramName")]
        public string ProgramName { get; set; }

        /// <summary>
        /// 节目链接地址
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "ProgSrc")]
        public string ProgSrc { get; set; }



        /// <summary>
        /// 节目类型
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ProgType")]
        public string ProgType { get; set; }

        /// <summary>
        /// 节目支持的屏幕
        /// </summary>
        [Display(Name = "ProgScreenInfo")]
        [StringLength(50)]
        public string ProgScreenInfo { get; set; }



        /// <summary>
        /// 切换模式
        /// </summary>
        [StringLength(50)]
        [Display(Name = "SwitchMode")]
        public string SwitchMode { get; set; }

        /// <summary>
        /// 切换时间
        /// </summary>
        [Display(Name = "SwitchTime")]
        public int SwitchTime { get; set; }

        /// <summary>
        /// 屏幕适应
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ScreenMatch")]
        public string ScreenMatch { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "LaunchTime")]
        public DateTime LaunchTime { get; set; }

        /// <summary>
        /// 预览图片
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "PreviewSrc")]
        public string PreviewSrc { get; set; }

        /// <summary>
        /// 节目文件
        /// </summary>
        [Display(Name = "FileCode")]
        [StringLength(255)]
        public string FileCode { get; set; }

        /// <summary>
        /// 预览文件
        /// </summary>
        [Display(Name = "PreviewFileGuid")]
        [StringLength(50)]
        public string PreviewFileGuid { get; set; }

        /// <summary>
        /// 下线时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "ExpiryDate")]
        public DateTime ExpiryDate { get; set; }
    }
}
