using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class Shops : Base
    {



        /// <summary>
        /// 店铺名称
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 店铺名称（英文）
        /// </summary>
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "NameEn")]
        public string NameEn { get; set; }

        /// <summary>
        /// 楼栋编号
        /// </summary>
        [Display(Name = "BuildingCode")]
        [StringLength(50)]
        public string BuildingCode { get; set; }

        /// <summary>
        /// 楼层编号
        /// </summary>
        [Display(Name = "FloorCode")]
        [StringLength(50)]
        public string FloorCode { get; set; }


        /// <summary>
        /// 业态
        /// </summary>
        [Display(Name = "ShopType")]
        [StringLength(50)]
        public string ShopFormat { get; set; }

        /// <summary>
        /// 二级业态
        /// </summary>
        [Display(Name = "SecFormat")]
        [StringLength(50)]
        public string SecFormat { get; set; }


        /// <summary>
        /// 门牌号
        /// </summary>
        [StringLength(255)]
        [Display(Name = "HouseNum")]
        public string HouseNum { get; set; }




        /// <summary>
        /// 联系方式
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 店铺介绍
        /// </summary>
        [Display(Name = "Intro")]
        public string Intro { get; set; }

        /// <summary>
        /// 店铺介绍(英文)
        /// </summary>
        [Display(Name = "IntroEn")]
        public string IntroEn { get; set; }

        /// <summary>
        /// 店铺横坐标
        /// </summary>
        [StringLength(255)]
        [DefaultValue("")]
        [Display(Name = "Xaxis")]
        public string Xaxis { get; set; }


        /// <summary>
        /// 店铺纵坐标
        /// </summary>
        [StringLength(255)]
        [DefaultValue("")]
        [Display(Name = "Yaxis")]
        public string Yaxis { get; set; }

        /// <summary>
        /// 导航横坐标
        /// </summary>
        [StringLength(255)]
        [DefaultValue("")]
        [Display(Name = "NavXaxis")]
        public string NavXaxis { get; set; }


        /// <summary>
        /// 导航纵坐标
        /// </summary>
        [StringLength(255)]
        [DefaultValue("")]
        [Display(Name = "NavYaxis")]
        public string NavYaxis { get; set; }

        /// <summary>
        /// 区域坐标
        /// </summary>
        [StringLength(2000)]
        [DefaultValue("")]
        [Display(Name = "AreaCoordinates")]
        public string AreaCoordinates { get; set; }





        /// <summary>
        /// 营业时间
        /// </summary>
        [StringLength(255)]
        [Display(Name = "CloseTime")]
        public string CloseTime { get; set; }


        /// <summary>
        /// 是否显示
        /// </summary>
        [Display(Name = "IsShow")]
        public bool IsShow { get; set; }


        /// <summary>
        /// 店铺全拼
        /// </summary>

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Spelling")]
        public string Spelling { get; set; }

        /// <summary>
        /// 店铺首字母
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Initials")]
        public string Initials { get; set; }

        /// <summary>
        /// 店铺Logo文件
        /// </summary>
        [Display(Name = "Logo")]
        [StringLength(50)]
        public string Logo { get; set; }


        /// <summary>
        /// 区域编码
        /// </summary>
        [Display(Name = "AreaCode")]
        [StringLength(50)]
        public string AreaCode { get; set; }


        /// <summary>
        /// 被删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }



    }
}
