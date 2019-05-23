using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class ShopFormatViewModel
    {
    }

    /// <summary>
    /// 添加用户输入
    /// </summary>
    public class Input_ShopFormatAdd
    {
        /// <summary>
        /// 客户对象
        /// </summary>
        [Display(Name = "Parameter")]
        public Input_ShopFormat Parameter { get; set; }
    }

    public class Input_ShopFormatDel
    {
        /// <summary>
        /// code集合
        /// </summary>
        [Display(Name = "Codes")]
        public List<string> Codes { get; set; }

        [Display(Name = "UserName")]
        public string UserName { get; set; }
    }


    /// <summary>
    /// 输入业态
    /// </summary>
    public class Input_ShopFormat
    {
        /// <summary>
        /// 业态名称
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string Name { get; set; }



        [Display(Name = "NameEn")]
        public string NameEn { get; set; }


        /// <summary>
        /// 业态颜色
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Color")]
        public string Color { get; set; }

        /// <summary>
        /// 图标文件
        /// </summary>
        [Display(Name = "IconFile")]
        public string IconFile { get; set; }

        /// <summary>
        /// 子业态
        /// </summary>
        public List<Input_ChildFM> Child { get; set; }

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

    public class Input_ChildFM
    {
        public string Name { get; set; }


        [Display(Name = "NameEn")]
        public string NameEn { get; set; }

        /// <summary>
        /// 图标文件
        /// </summary>
        [Display(Name = "IconFile")]
        public string IconFile { get; set; }
    }



    /// <summary>
    /// 编辑业态信息
    /// </summary>
    public class Input_ShopFormatEdit
    {

        /// <summary>
        /// 客户对象
        /// </summary>
        [Display(Name = "Parameter")]
        public Input_ShopFormatE Parameter { get; set; }
    }
    public class Input_ShopFormatE
    {
        public string Code { get; set; }

        /// <summary>
        /// 业态名称
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string Name { get; set; }



        [Display(Name = "NameEn")]
        public string NameEn { get; set; }


        /// <summary>
        /// 业态颜色
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Color")]
        public string Color { get; set; }

        /// <summary>
        /// 图标文件
        /// </summary>
        [Display(Name = "IconFile")]
        public string IconFile { get; set; }



        /// <summary>
        /// 子业态
        /// </summary>
        public List<Input_ChildFM> Child { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Display(Name = "UserName")]
        public string UserName { get; set; }
    }
    /// <summary>
    /// 输出业态
    /// </summary>
    public class Output_ShopFormat
    {
        /// <summary>
        /// 业态ID
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 业态名称
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 业态名称（英文）
        /// </summary>
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "NameEn")]
        public string NameEn { get; set; }

        /// <summary>
        /// 业态颜色
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Color")]
        public string Color { get; set; }

        /// <summary>
        /// 业态创建时间
        /// </summary>
        [StringLength(255)]
        [Display(Name = "AddTime")]
        public string AddTime { get; set; }

        /// <summary>
        /// 子业态
        /// </summary>
        public List<Output_ShopFormat> ChildFormat { get; set; }
    }

    /// <summary>
    /// 业态-标签编辑
    /// </summary>
    public class Input_EditTipsToFormat
    {

        /// <summary>
        /// 业态ID
        /// </summary>
        [Display(Name = "FormatCode")]
        public string FormatCode { get; set; }



        /// <summary>
        /// 权限编码列表
        /// </summary>
        [Display(Name = "TipsCode")]
        public List<string> TipsCode { get; set; }

    }
}
