using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class AssetFile
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Required]
        [Key]
        [Display(Name = "FileID")]
        public long FileID { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "FileName")]
        public string FileName { get; set; }

        /// <summary>
        /// 文件后缀名
        /// </summary>
        [StringLength(255)]
        [Display(Name = "FileExtName")]
        public string FileExtName { get; set; }


        /// <summary>
        /// 文件类型
        /// </summary>
        [StringLength(255)]
        [Display(Name = "FileType")]
        public string FileType { get; set; }

        /// <summary>
        /// 文件唯一标志
        /// </summary>
        [StringLength(255)]
        [Display(Name = "FileGUID")]
        public string FileGUID { get; set; }

        /// <summary>
        /// 文件哈希码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "FileHash")]
        public string FileHash { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [Display(Name = "FileSize")]
        public long FileSize { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "FilePath")]
        public string FilePath { get; set; }


        /// <summary>
        /// 更新时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "UpdateTime")]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 时长
        /// </summary>
        [Display(Name = "Duration")]
        public long Duration { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        [Display(Name = "Width")]
        public int Width { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        [Display(Name = "Height")]
        public int Height { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "AddTime")]
        public DateTime AddTime { get; set; }
    }
}
