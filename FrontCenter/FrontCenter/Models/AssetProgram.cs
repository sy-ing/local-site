using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class AssetProgram
    {
        /// <summary>
        /// 节目ID
        /// </summary>
        [Required]
        [Display(Name = "ID")]
        public int ID { get; set; }


        /// <summary>
        /// 节目名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ProgramName")]
        public string ProgramName { get; set; }

        /// <summary>
        /// 节目文件
        /// </summary>
        [Display(Name = "FileCode")]
        public string FileCode { get; set; }

        /// <summary>
        /// 节目支持的屏幕
        /// </summary>
        [Display(Name = "ProgScreenInfo")]
        public int ProgScreenInfo { get; set; }


        /// <summary>
        /// 切换模式
        /// </summary>

        [Display(Name = "SwitchMode")]
        public int SwitchMode { get; set; }


        /// <summary>
        /// 切换时间
        /// </summary>
        [Display(Name = "SwitchTime")]
        public int SwitchTime { get; set; }


        /// <summary>
        /// 屏幕适应
        /// </summary>
        [Display(Name = "SwitchMode")]
        public int ScreenMatch { get; set; }


        /// <summary>
        /// 上线时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "LaunchTime")]
        public DateTime LaunchTime { get; set; }

        /// <summary>
        /// 预览图片
        /// </summary>
        [Display(Name = "PreviewFileID")]
        public long PreviewFileID { get; set; }

        /// <summary>
        /// 下线时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "ExpiryDate")]
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "AddTime")]
        public DateTime AddTime { get; set; }

    }
}
