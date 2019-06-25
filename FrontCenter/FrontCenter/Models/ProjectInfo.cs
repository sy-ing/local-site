using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class ProjectInfo:Base
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "CusID")]
        public string CusID { get; set; }


        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "RegKey")]
        public string RegKey { get; set; }

    }
}
