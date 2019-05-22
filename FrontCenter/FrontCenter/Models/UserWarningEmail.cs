using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class UserWarningEmail:Base
    {


        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "UserCode")]
        public string UserCode { get; set; }


    }
}
