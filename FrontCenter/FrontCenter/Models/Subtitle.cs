using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class Subtitle : Base
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 已删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }


        /// <summary>
        /// 字幕名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 字幕内容
        /// </summary>
        [Display(Name = "Text")]
        public string Text { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "BeginTime")]
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "EndTime")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 位置 top 顶部 foot 底部
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Location")]
        public string Location { get; set; }

        /// <summary>
        /// 模式-  定期的Regular  即时的 Immediate
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Type")]
        public string Type { get; set; }

        /// <summary>
        /// 时长
        /// </summary>
        [Display(Name = "Duration")]
        public int Duration { get; set; }

    }
}
