﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class MallArea : Base
    {
        /// <summary>
        /// 客户唯一编码
        /// </summary>
        [Display(Name = "MallCode")]
        [StringLength(50)]
        public string MallCode { get; set; }

        /// <summary>
        /// 楼栋编码
        /// </summary>
        [Display(Name = "AreaCode")]
        [StringLength(50)]
        public string AreaCode { get; set; }
    }
}
