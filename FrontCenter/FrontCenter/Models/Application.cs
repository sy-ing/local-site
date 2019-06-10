using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class Application
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        [Required]
        [Display(Name = "ID")]
        public int ID { get; set; }

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
        [Display(Name = "IconFileID")]
        public long IconFileID { get; set; }
        /*
        /// <summary>
        /// 应用分类
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Classification")]
        public string Classification { get; set; }
        */

        /// <summary>
        /// 应用分类
        /// </summary>
        [Display(Name = "Appclass")]
        public int AppClass { get; set; }

        /// <summary>
        /// 应用二级分类
        /// </summary>
        [Display(Name = "AppSecClass")]
        public int AppSecClass { get; set; }

        /// <summary>
        /// 屏幕属性
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ScreenInfoID")]
        public int ScreenInfoID { get; set; }




        /// <summary>
        /// 预览图片
        /// </summary>
        [StringLength(255)]
        [Display(Name = "PreviewFiles")]
        public string PreviewFiles { get; set; }


        /// <summary>
        /// 应用描述
        /// </summary>
        [StringLength(255)]
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
        /// 创建时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "AddTime")]
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 文件的ID
        /// </summary>
        [Display(Name = "FileCode")]
        public string FileCode { get; set; }


        /// <summary>
        /// 是否被删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }


    }
}
