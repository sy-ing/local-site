using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class BrandFormat : Base
    {
        /// <summary>
        /// 品牌编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "BrandCode")]
        public string BrandCode { get; set; }




        /// <summary>
        /// 业态编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "FormatCode")]
        public string FormatCode { get; set; }


        /// <summary>
        /// 父编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ParentFormatCode")]
        public string ParentFormatCode { get; set; }

        /// <summary>
        /// 被删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }
    }
}
