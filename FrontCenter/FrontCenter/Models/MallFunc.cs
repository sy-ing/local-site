using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class MallFunc : Base
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        [StringLength(255)]
        public string MallCode { get; set; }

        /// <summary>
        /// 功能编码
        /// </summary>
        [Display(Name = "FuncCode")]
        [StringLength(255)]
        public string FuncCode { get; set; }
    }
}
