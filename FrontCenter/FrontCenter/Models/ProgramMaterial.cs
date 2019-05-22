using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 节目订单表
    /// </summary>
    public class ProgramMaterial : Base
    {
        /// <summary>
        /// 节目订单编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ProgramOrderCode")]
        public string ProgramOrderCode { get; set; }

        /// <summary>
        /// 文件编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "FileCode")]
        public string FileCode { get; set; }

        /// <summary>
        /// 预览文件编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "PreviewFileCode")]
        public string PreviewFileCode { get; set; }

        /// <summary>
        /// 节目类型
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ProgType")]
        public string ProgType { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [Display(Name = "IsDelete")]
        public int IsDelete { get; set; }

        /// <summary>
        /// 是否可疑
        /// </summary>
        [Display(Name = "IsSuspicious")]
        public int IsSuspicious { get; set; }
    }
}
