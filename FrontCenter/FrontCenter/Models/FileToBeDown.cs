using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class FileToBeDown : Base
    {

        /// <summary>
        /// 文件唯一标志
        /// </summary>
        [StringLength(255)]
        [Display(Name = "FileCode")]
        public string FileCode { get; set; }

        /// <summary>
        /// 尝试次数
        /// </summary>
        [Display(Name = "StartNum")]
        public int StartNum { get; set; }
    }
}
