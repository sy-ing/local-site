using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class SubtitleViewModel
    {
    }

    public class Input_SetSubtitle
    {

        /// <summary>
        /// 字幕编码
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 字幕名称
        /// </summary>
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


    public class Input_SubtitlePublish
    {

        /// <summary>
        /// 字幕编码
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }



        /// <summary>
        /// 设备组编码
        /// </summary>
        [Display(Name = "GroupCode")]
        public List<string> GroupCode { get; set; }



    }


    public class Input_SubtitlePublishCancel
    {

        /// <summary>
        /// 关联编码
        /// </summary>
        [Display(Name = "Code")]
        public List<string> Code { get; set; }




    }

    public class Input_GetSubtitle
    {


        /// <summary>
        /// 字幕编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }

    public class Input_DelSubtitle
    {


        /// <summary>
        /// 字幕编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Code")]
        public List<string> Code { get; set; }
    }

    public class Input_GetSubtitleList : Pagination
    {
        /// <summary>
        /// 字幕名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }

    public class Input_GetSubtitleGroupList
    {


        /// <summary>
        /// 字幕编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }


    public class Input_GetSubtitleListByGroupCode
    {


        /// <summary>
        /// 设备组编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "GroupCode")]
        public string GroupCode { get; set; }
    }

    public class Input_GetDevSubtitle
    {
        /// <summary>
        /// 设备Code
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }
}
