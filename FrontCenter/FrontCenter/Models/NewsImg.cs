using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class NewsImg : Base
    {

        /// <summary>
        /// 消息编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "NewsCode")]
        public string NewsCode { get; set; }

        /// <summary>
        /// 图片GUID
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Img")]
        public string Img { get; set; }

        /// <summary>
        /// 是否可疑
        /// </summary>
        [Display(Name = "IsSuspicious")]
        public bool IsSuspicious { get; set; }
    }
}
