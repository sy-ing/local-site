using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    /// <summary>
    /// 上传文件返回信息
    /// </summary>
    public class Output_FileInfo
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        [Display(Name = "FilePath")]
        public string FilePath { get; set; }

        /// <summary>
        /// 文件ID
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 缩略图路径
        /// </summary>
        [Display(Name = "PreviewPath")]
        public string PreviewPath { get; set; }

        /// <summary>
        /// 缩略图ID
        /// </summary>
        [Display(Name = "PreviewFileGUID")]
        public string PreviewFileGUID { get; set; }


    }
}
