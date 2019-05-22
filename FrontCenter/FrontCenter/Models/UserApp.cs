using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class UserApp : Base
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name = "UserID")]
        public int UserID { get; set; }

        /// <summary>
        /// 应用编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "AppCode")]
        public string AppCode { get; set; }
    }
}
