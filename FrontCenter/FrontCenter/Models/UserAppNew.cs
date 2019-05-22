using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class UserAppNew : Base
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        [Display(Name = "UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 应用编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "AppCode")]
        public string AppCode { get; set; }
    }
}
