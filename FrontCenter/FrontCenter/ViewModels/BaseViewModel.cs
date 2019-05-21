using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class BaseViewModel
    {
    }

    /// <summary>
    /// 分页
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// 是否分页 0 不分页 1分页
        /// </summary>
        [Required]
        [Display(Name = "Paging")]
        public int? Paging { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        [Display(Name = "PageIndex")]
        public int? PageIndex { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        [Display(Name = "PageSize")]
        public int? PageSize { get; set; }
    }
}
