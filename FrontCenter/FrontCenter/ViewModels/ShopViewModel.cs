using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class ShopViewModel
    {
    }

    /// <summary>
    /// 店铺添加
    /// </summary>
    public class Input_ShopAdd
    {

        /// <summary>
        /// 店铺对象
        /// </summary>
        [Display(Name = "Parameter")]
        public Input_AddShop Parameter { get; set; }

    }


    /// <summary>
    /// 店铺添加
    /// </summary>
    public class Input_AddShop
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
        /// 业态
        /// </summary>
        [Display(Name = "ShopFormat")]
        public string ShopFormat { get; set; }

        /// <summary>
        /// 二级业态
        /// </summary>
        public string SecFormat { get; set; }




        /// <summary>
        /// 楼栋编号
        /// </summary>
        [Display(Name = "BuildingCode")]
        public string BuildingCode { get; set; }

        /// <summary>
        /// 楼层编号
        /// </summary>
        [Display(Name = "FloorCode")]
        public string FloorCode { get; set; }

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
        /// 营业时间
        /// </summary>
        [StringLength(255)]
        [Display(Name = "CloseTime")]
        public string CloseTime { get; set; }


        /// <summary>
        /// 店铺Logo文件
        /// </summary>
        [Display(Name = "Logo")]
        public string Logo { get; set; }



        /// <summary>
        /// 店铺介绍
        /// </summary>
        [Display(Name = "Intro")]
        public string Intro { get; set; }

        /// <summary>
        /// 店铺介绍(英文)
        /// </summary>
        [Display(Name = "IntroEN")]
        public string IntroEN { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        [Display(Name = "AreaCode")]
        public string AreaCode { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 品牌编码
        /// </summary>
        [Display(Name = "BrandCode")]
        public string BrandCode { get; set; }
    }

    /// <summary>
    /// 店铺添加
    /// </summary>
    public class Input_Shop
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
        /// 业态
        /// </summary>
        [Display(Name = "ShopFormat")]
        public string ShopFormat { get; set; }

        /// <summary>
        /// 二级业态
        /// </summary>
        public string SecFormat { get; set; }




        /// <summary>
        /// 楼栋编号
        /// </summary>
        [Display(Name = "BuildingCode")]
        public string BuildingCode { get; set; }

        /// <summary>
        /// 楼层编号
        /// </summary>
        [Display(Name = "FloorCode")]
        public string FloorCode { get; set; }

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
        /// 营业时间
        /// </summary>
        [StringLength(255)]
        [Display(Name = "CloseTime")]
        public string CloseTime { get; set; }


        /// <summary>
        /// 店铺Logo文件
        /// </summary>
        [Display(Name = "Logo")]
        public string Logo { get; set; }



        /// <summary>
        /// 店铺介绍
        /// </summary>
        [Display(Name = "Intro")]
        public string Intro { get; set; }

        /// <summary>
        /// 店铺介绍(英文)
        /// </summary>
        [Display(Name = "IntroEN")]
        public string IntroEN { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        [Display(Name = "AreaCode")]
        public string AreaCode { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }

    public class Input_ShopE
    {
        /// <summary>
        /// 店铺对象
        /// </summary>
        [Display(Name = "Parameter")]
        public Input_ShopEdit Parameter { get; set; }
    }
    /// <summary>
    /// 店铺编辑
    /// </summary>
    public class Input_ShopEdit : Input_Shop
    {

        /// <summary>
        /// 数字ID
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }
    }









    /// <summary>
    /// 商店导航编辑
    /// </summary>
    public class Input_ShopNavEdit
    {
        /// <summary>
        /// 商店数字ID
        /// </summary>
        [Display(Name = "ShopCode")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 店铺横坐标
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Xaxis")]
        public string Xaxis { get; set; }


        /// <summary>
        /// 店铺纵坐标
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Yaxis")]
        public string Yaxis { get; set; }

        /// <summary>
        /// 导航横坐标
        /// </summary>
        [StringLength(255)]
        [Display(Name = "NavXaxis")]
        public string NavXaxis { get; set; }


        /// <summary>
        /// 导航纵坐标
        /// </summary>
        [StringLength(255)]
        [Display(Name = "NavYaxis")]
        public string NavYaxis { get; set; }

        /// <summary>
        /// 区域坐标
        /// </summary>
        //[StringLength(1000)]
        [Display(Name = "AreaCoordinates")]
        public string AreaCoordinates { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        //[StringLength(1000)]
        [Display(Name = "UserName")]
        public string UserName { get; set; }
    }



    public class Input_ShopQuery
    {
        /// <summary>
        /// 对象
        /// </summary>
        [Display(Name = "Parameter")]
        public Input_ShopListQuery Parameter { get; set; }
    }


    /// <summary>
    /// 店铺列表查询
    /// </summary>
    public class Input_ShopListQuery
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 楼层ID
        /// </summary>
        public string FloorCode { get; set; }

        /// <summary>
        /// 楼栋
        /// </summary>
        public string BuildingCode { get; set; }

        /// <summary>
        /// 业态
        /// </summary>
        public string ShopFormatCode { get; set; }

    }
    /// <summary>
    /// 店铺查询条件
    /// </summary>
    public class Input_ShopCond
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 楼层ID
        /// </summary>
        public string FloorCode { get; set; }

        /// <summary>
        /// 楼栋
        /// </summary>
        public string BuildingCode { get; set; }

        /// <summary>
        /// 业态
        /// </summary>
        public string ShopFormatCode { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }

    public class Input_ShopCondPage : Pagination
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 楼层ID
        /// </summary>
        public string FloorCode { get; set; }

        /// <summary>
        /// 楼栋
        /// </summary>
        public string BuildingCode { get; set; }

        /// <summary>
        /// 业态
        /// </summary>
        public string ShopFormatCode { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }

    /// <summary>
    /// 修改店铺显示状态
    /// </summary>
    public class Input_ShopStatus
    {
        /// <summary>
        /// 店铺编码
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        [Display(Name = "IsShow")]
        public bool IsShow { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "UserName")]
        public string UserName { get; set; }
    }



    public class Output_ShopCond
    {
        ///// <summary>
        ///// 数字ID
        ///// </summary>
        //[Display(Name = "ID")]
        //public int ID { get; set; }

        /// <summary>
        /// 字符编码
        /// </summary>
        [Key]
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string Name { get; set; }



        /// <summary>
        /// 业态
        /// </summary>
        [Display(Name = "ShopFormatCode")]
        public string ShopFormatCode { get; set; }

        /// <summary>
        /// 楼栋编号
        /// </summary>
        [Display(Name = "BuildingCode")]
        public string BuildingCode { get; set; }

        /// <summary>
        /// 楼层编号
        /// </summary>
        [Display(Name = "FloorCode")]
        public string FloorCode { get; set; }

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
        /// 排序
        /// </summary>
        [Display(Name = "FloorOrder")]
        public int FloorOrder { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        [Display(Name = "IsShow")]
        public bool IsShow { get; set; }


        /// <summary>
        /// 二级业态
        /// </summary>
        [Display(Name = "SecFormat")]
        public string SecFormat { get; set; }


        /// <summary>
        /// 店铺名称（英文）
        /// </summary>
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "NameEn")]
        public string NameEn { get; set; }

        /// <summary>
        /// 店铺全拼
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Spelling")]
        public string Spelling { get; set; }

        /// <summary>
        /// 店铺Logo文件
        /// </summary>
        [Display(Name = "Logo")]
        public string Logo { get; set; }


        /// <summary>
        /// 区域坐标
        /// </summary>
        //[StringLength(1000)]
        [DefaultValue("")]
        [Display(Name = "AreaCoordinates")]
        public string AreaCoordinates { get; set; }


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
        /// 营业时间
        /// </summary>
        [StringLength(255)]
        [Display(Name = "CloseTime")]
        public string CloseTime { get; set; }

        /// <summary>
        /// 店铺介绍
        /// </summary>
        [Display(Name = "Intro")]
        public string Intro { get; set; }

        /// <summary>
        /// 业态颜色
        /// </summary>
        [StringLength(255)]
        [Display(Name = "FormatColor")]
        public string FormatColor { get; set; }

        /// <summary>
        /// 业态名称
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "ShopFormat")]
        public string ShopFormat { get; set; }


        /// <summary>
        /// 楼层名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "FloorName")]
        public string FloorName { get; set; }




        /// <summary>
        /// 文件路径
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "LogoPath")]
        public string LogoPath { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "AreaName")]
        public string AreaName { get; set; }

        /// <summary>
        /// 缩写名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Initials")]
        public string Initials { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        [Display(Name = "AreaCode")]
        public string AreaCode { get; set; }

        /// <summary>
        /// 绑定码
        /// </summary>
        [Display(Name = "ShopNum")]
        public string ShopNum { get; set; }



    }

    public class Output_ShopListCond
    {
        ///// <summary>
        ///// 数字ID
        ///// </summary>
        //[Display(Name = "ID")]
        //public int ID { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Key]
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string Name { get; set; }



        /// <summary>
        /// 业态
        /// </summary>
        [Display(Name = "ShopFormatCode")]
        public string ShopFormatCode { get; set; }

        /// <summary>
        /// 楼栋编号
        /// </summary>
        [Display(Name = "BuildingCode")]
        public string BuildingCode { get; set; }

        /// <summary>
        /// 楼层编号
        /// </summary>
        [Display(Name = "FloorCode")]
        public string FloorCode { get; set; }

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
        /// 排序
        /// </summary>
        [Display(Name = "FloorOrder")]
        public int FloorOrder { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        [Display(Name = "IsShow")]
        public bool IsShow { get; set; }


        /// <summary>
        /// 二级业态
        /// </summary>
        [Display(Name = "SecFormat")]
        public string SecFormat { get; set; }


        /// <summary>
        /// 店铺名称（英文）
        /// </summary>
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "NameEn")]
        public string NameEn { get; set; }

        /// <summary>
        /// 店铺全拼
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Spelling")]
        public string Spelling { get; set; }

        /// <summary>
        /// 店铺Logo文件
        /// </summary>
        [Display(Name = "Logo")]
        public string Logo { get; set; }


        /// <summary>
        /// 区域坐标
        /// </summary>
        //[StringLength(1000)]
        [DefaultValue("")]
        [Display(Name = "AreaCoordinates")]
        public string AreaCoordinates { get; set; }


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
        /// 营业时间
        /// </summary>
        [StringLength(255)]
        [Display(Name = "CloseTime")]
        public string CloseTime { get; set; }

        /// <summary>
        /// 店铺介绍
        /// </summary>
        [Display(Name = "Intro")]
        public string Intro { get; set; }

        /// <summary>
        /// 业态颜色
        /// </summary>
        [StringLength(255)]
        [Display(Name = "FormatColor")]
        public string FormatColor { get; set; }

        /// <summary>
        /// 业态名称
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "ShopFormat")]
        public string ShopFormat { get; set; }


        /// <summary>
        /// 楼层名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "FloorName")]
        public string FloorName { get; set; }



        /// <summary>
        /// 楼栋名称
        /// </summary>
        [DefaultValue("")]
        [StringLength(255)]
        [Display(Name = "BuildingName")]
        public string BuildingName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [DefaultValue("")]
        [StringLength(2000)]
        [Display(Name = "LogoPath")]
        public string LogoPath { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        [DefaultValue("")]
        [StringLength(255)]
        [Display(Name = "AreaName")]
        public string AreaName { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        [Display(Name = "AreaCode")]
        public string AreaCode { get; set; }

        /// <summary>
        /// 店铺添加时间
        /// </summary>
        [Display(Name = "AddTime")]
        public DateTime AddTime { get; set; }
    }


    public class Output_SelectShop
    {
        /// <summary>
        /// 数字ID
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 店铺名称（英文）
        /// </summary>
        [Display(Name = "NameEn")]
        public string NameEn { get; set; }

        /// <summary>
        /// 门牌号
        /// </summary>
        [StringLength(255)]
        [Display(Name = "HouseNum")]
        public string HouseNum { get; set; }

        /// <summary>
        /// 楼栋编号
        /// </summary>
        [Display(Name = "BuildingCode")]
        public string BuildingCode { get; set; }


        /// <summary>
        /// 楼层编号
        /// </summary>
        [Display(Name = "FloorCode")]
        public string FloorCode { get; set; }



        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "FloorOrder")]
        public int FloorOrder { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "LogoPath")]
        public string LogoPath { get; set; }


        /// <summary>
        /// 区域坐标
        /// </summary>
        //[StringLength(1000)]
        [DefaultValue("")]
        [Display(Name = "AreaCoordinates")]
        public string AreaCoordinates { get; set; }

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

        [Display(Name = "Node")]
        public string Node { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }




        /// <summary>
        /// 营业结束时间
        /// </summary>
        [StringLength(255)]
        [Display(Name = "CloseTime")]
        public string CloseTime { get; set; }

        /// <summary>
        /// 店铺介绍
        /// </summary>
        [Display(Name = "Intro")]
        public string Intro { get; set; }

        /// <summary>
        /// 店铺介绍
        /// </summary>
        [Display(Name = "IntroEn")]
        public string IntroEn { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        [Display(Name = "AreaCode")]
        public string AreaCode { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "AreaName")]
        public string AreaName { get; set; }

        /// <summary>
        /// 业态名称
        /// </summary>
        [Display(Name = "ShopFormat")]
        public string ShopFormat { get; set; }

        /// <summary>
        /// 业态
        /// </summary>
        [Display(Name = "ShopFormatCode")]
        public string ShopFormatCode { get; set; }

        /// <summary>
        /// 二级业态
        /// </summary>
        [Display(Name = "SecFormat")]
        public string SecFormat { get; set; }


        /// <summary>
        /// 二级业态ID
        /// </summary>
        public string SecFormatCode { get; set; }

        /// <summary>
        /// 业态颜色
        /// </summary>
        [StringLength(255)]
        [Display(Name = "FormatColor")]
        public string FormatColor { get; set; }

    }

    public class Input_GetMallTime
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }

}
