using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class ProgramGroupViewModel
    {
    }

    /// <summary>
    /// 添加节目组
    /// </summary>
    public class Input_ProgramGroup
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }


        /// <summary>
        /// 节目组名称
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "GroupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// 显示屏规格
        /// </summary>
        [Required]
        [Display(Name = "ScreenInfoCode")]
        public string ScreenInfoCode { get; set; }

        /// <summary>
        /// 节目列表
        /// </summary>
        [Display(Name = "Programs")]
        public List<string> Programs { get; set; }

    }

    public class Input_QuickPublishProg : Input_Prog
    {
        /// <summary>
        /// 节目组名称
        /// </summary>
        [Display(Name = "GroupName")]
        public string GroupName { get; set; }
    }
    public class Input_GetDeviceGroupByProgGooup
    {
        public string ProgGroupCode { get; set; }
    }
    public class Input_GetProgByGroup
    {

        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }


        /// <summary>
        /// 节目组编码
        /// </summary>
        [Display(Name = "GroupCode")]
        public string GroupCode { get; set; }

        /// <summary>
        /// 分辨率编码
        /// </summary>
        [Display(Name = "ScreenCode")]
        public string ScreenCode { get; set; }
    }

    /// <summary>
    /// 节目组分页
    /// </summary>
    public class Input_ProgramGroupQuery : Pagination
    {
        /// <summary>
        /// 显示屏规格
        /// </summary>
        [Display(Name = "ScreenCode")]
        public string ScreenCode { get; set; }


    }
    /// <summary>
    /// 节目组分页
    /// </summary>
    public class Input_ProgramGroupQueryNew : Pagination
    {

        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 显示屏规格
        /// </summary>
        [Display(Name = "ScreenCode")]
        public string ScreenCode { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        [Display(Name = "SearchKey")]
        public string SearchKey { get; set; }

        /// <summary>
        /// 排序规则 desc降序 asc升序
        /// </summary>
        [Display(Name = "Order")]
        public string Order { get; set; }
    }
    public class Output_ProgramGroupQuery
    {

        /// <summary>
        /// 编号
        /// </summary>
        [Display(Name = "ID")]
        public int ID { get; set; }


        /// <summary>
        /// 编码
        /// </summary>
        [Key]
        [Display(Name = "Code")]
        public string Code { get; set; }


        /// <summary>
        /// 组名
        /// </summary>
        [Display(Name = "GroupName")]
        public string GroupName { get; set; }


        /// <summary>
        /// 分辨率
        /// </summary>
        [Display(Name = "ScreenInfo")]
        public string ScreenInfo { get; set; }

        /// <summary>
        /// 分辨率编码
        /// </summary>
        [Display(Name = "ScreenInfoCode")]
        public string ScreenInfoCode { get; set; }


        /// <summary>
        /// 节目数量
        /// </summary>
        [Display(Name = "ProgramCount")]
        public int ProgramCount { get; set; }

    }


    public class Input_PGDel
    {
        /// <summary>
        /// 编码数组
        /// </summary>
        [Display(Name = "Codes")]
        public List<string> Codes { get; set; }
    }

    /// <summary>
    /// 输出节目组列表
    /// </summary>
    public class Output_ProgramGroupList
    {
        /// <summary>
        /// 节目组ID
        /// </summary>
        [Required]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [StringLength(50)]
        [Display(Name = "GroupName")]
        public string GroupName { get; set; }


        /// <summary>
        /// 屏幕描述
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ScreenInfo")]
        public string ScreenInfo { get; set; }

        /// <summary>
        /// 节目数量
        /// </summary>
        [Display(Name = "ProgramCount")]
        public int ProgramCount { get; set; }
    }



    /// <summary>
    /// 节目组信息输入
    /// </summary>
    public class Input_ProgramGroupInfo
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 节目组名称
        /// </summary>
        [StringLength(50), MinLength(1)]
        [Display(Name = "GroupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// 节目列表
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "Programs")]
        public List<string> Programs { get; set; }
    }
}
