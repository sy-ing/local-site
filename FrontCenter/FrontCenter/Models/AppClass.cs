using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 应用分类
    /// </summary>
    public class AppClass
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Display(Name = "ID")]
        public int ID { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ClassName")]
        public string ClassName { get; set; }

        /// <summary>
        /// 父类ID
        /// </summary>
        [Display(Name = "Parent")]
        public int Parent { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "Order")]
        public int Order { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "AddTime")]
        public DateTime AddTime { get; set; }
    }
}
