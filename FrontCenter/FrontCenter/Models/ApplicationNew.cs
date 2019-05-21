using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class ApplicationNew : Base
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 应用唯一编码
        /// </summary>
        [Display(Name = "AppID")]
        [StringLength(255)]
        public string AppID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }




        /// <summary>
        /// 应用英文名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "NameEn")]
        public string NameEn { get; set; }



        /// <summary>
        /// Icon图片文件的ID
        /// </summary>
        [Display(Name = "IconFileCode")]
        [StringLength(255)]
        public string IconFileCode { get; set; }

        /// <summary>
        /// android对应的命名空间
        /// </summary>
        [Display(Name = "Namespace")]
        [StringLength(255)]
        public string Namespace { get; set; }

        /// <summary>
        /// 1：PC   2:android
        /// </summary>
        [Display(Name = "PlatformType")]
        public int PlatformType { get; set; }


        /// <summary>
        /// 应用分类
        /// </summary>
        [Display(Name = "Appclass")]
        [StringLength(255)]
        public string AppClass { get; set; }

        /// <summary>
        /// 应用二级分类
        /// </summary>
        [Display(Name = "AppSecClass")]
        [StringLength(255)]
        public string AppSecClass { get; set; }

        /// <summary>
        /// 屏幕属性
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ScreenInfoCode")]
        public string ScreenInfoCode { get; set; }

        /// <summary>
        /// 预览图片
        /// </summary>
        [StringLength(255)]
        [Display(Name = "PreviewFiles")]
        public string PreviewFiles { get; set; }


        /// <summary>
        /// 应用描述
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// 开发者
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Developer")]
        public string Developer { get; set; }

        /// <summary>
        /// 设备支持
        /// </summary>
        [StringLength(255)]
        [Display(Name = "DevSupport")]
        public string DevSupport { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Version")]
        public string Version { get; set; }

        /// <summary>
        /// 文件的ID
        /// </summary>
        [Display(Name = "FileCode")]
        [StringLength(255)]
        public string FileCode { get; set; }


        /// <summary>
        /// 是否被删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }


        /// <summary>
        /// 启动项文件名
        /// </summary>
        [Display(Name = "Startup")]
        [StringLength(1000)]
        public string Startup { get; set; }


    }
}
