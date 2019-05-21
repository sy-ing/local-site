using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class DataDict : Base
    {


        /// <summary>
        /// 类型名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "DictName")]
        public string DictName { get; set; }



        /// <summary>
        /// 类型名称英文
        /// </summary>
        [StringLength(255)]
        [Display(Name = "DictNameEn")]
        public string DictNameEn { get; set; }

        /// <summary>
        /// 字典值
        /// </summary>
        [StringLength(255)]
        [Display(Name = "DictValue")]
        public string DictValue { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "ShowOrder")]
        public int ShowOrder { get; set; }



    }
}
