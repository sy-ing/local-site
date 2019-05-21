using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class MallUserViewModel
    {
    }


    public class Input_MallUserGetList
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }
}
