using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class Brand : Base
    {
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "NameEn")]
        public string NameEn { get; set; }



        /// <summary>
        /// 全彩Logo
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ChromotypeLogo")]
        public string ChromotypeLogo { get; set; }

        /// <summary>
        /// 反白Logo
        /// </summary>
        [StringLength(50)]
        [Display(Name = "MonochromeLogo")]
        public string MonochromeLogo { get; set; }


        /// <summary>
        /// 简介
        /// </summary>
        [Display(Name = "Intro")]
        public string Intro { get; set; }


        /// <summary>
        /// 英文简介
        /// </summary>
        [Display(Name = "IntroEn")]
        public string IntroEn { get; set; }

        /// <summary>
        /// 完成度
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Degree")]
        public string Degree { get; set; }

        /// <summary>
        /// 被删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }



        /// <summary>
        /// 品牌全拼
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Spelling")]
        public string Spelling { get; set; }

        /// <summary>
        /// 品牌首字母
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Initials")]
        public string Initials { get; set; }
    }
}
