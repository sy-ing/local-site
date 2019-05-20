using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class FuncInfo : Base
    {

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }



        /// <summary>
        /// 权限编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "PermCode")]
        public string PermCode { get; set; }
    }
}
