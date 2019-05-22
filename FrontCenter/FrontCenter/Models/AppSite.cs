using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class AppSite:Base
    {

       


        /// <summary>
        /// 应用编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "AppCode")]
        public string AppCode { get; set; }

        /// <summary>
        /// 应用后台链接
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "Href")]
        public string Href { get; set; }

       
    }
}
