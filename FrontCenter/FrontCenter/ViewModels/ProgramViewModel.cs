using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{

    public class Input_FileUpload
    {
        /// <summary>
        /// 文件名
        /// </summary>
        [Required]
        [StringLength(500), MinLength(1)]
        [Display(Name = "FileName")]
        public string FileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [Required]
        [Display(Name = "FileSize")]
        public long FileSize { get; set; }

        /// <summary>
        /// 唯一标志
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "UUID")]
        public string UUID { get; set; }

        /// <summary>
        /// 是否分片
        /// </summary>
        [Display(Name = "Chunked")]
        public bool Chunked { get; set; }

        /// <summary>
        /// 文件|文件片
        /// </summary>
        [Display(Name = "Data")]
        public IFormFile Data { get; set; }

        /// <summary>
        /// 当前片序号
        /// </summary>
        [Display(Name = "CurrChunk")]
        public int CurrChunk { get; set; }

        /// <summary>
        /// 总片数
        /// </summary>
        [Display(Name = "TotalChunk")]
        public int TotalChunk { get; set; }
    }

    public class Input_ProAdd
    {


        /// <summary>
        /// 文件ID
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }


        /// <summary>
        /// 文件ID
        /// </summary>
        [Display(Name = "PreviewFileGUID")]
        public string PreviewFileGUID { get; set; }

        /// <summary>
        /// 节目名称
        /// </summary>
        [Required]
        [Display(Name = "ProgramName")]
        public string ProgramName { get; set; }


        /// <summary>
        /// 节目支持的屏幕
        /// </summary>
        [Display(Name = "ProgScreenInfo")]
        public string ProgScreenInfo { get; set; }


        /// <summary>
        /// 切换模式
        /// </summary>
        [StringLength(50)]
        [Display(Name = "SwitchMode")]
        public string SwitchMode { get; set; }

        /// <summary>
        /// 切换时间
        /// </summary>
        [Display(Name = "SwitchTime")]
        public int SwitchTime { get; set; }

        /// <summary>
        /// 屏幕适应
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ScreenMatch")]
        public string ScreenMatch { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        [Display(Name = "LaunchTime")]
        public string LaunchTime { get; set; }

        /// <summary>
        /// 下线时间
        /// </summary>

        [Display(Name = "ExpiryDate")]
        public string ExpiryDate { get; set; }
    }



    public class Input_PrgFile
    {

        /// <summary>
        /// 节目名称
        /// </summary>
        [Required]
        [Display(Name = "ProgramName")]
        public string ProgramName { get; set; }

        /// <summary>
        /// 文件ID
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }


        /// <summary>
        /// 文件ID
        /// </summary>
        [Display(Name = "PreviewFileGUID")]
        public string PreviewFileGUID { get; set; }
    }

    public class Input_Prog
    {
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
        /// <summary>
        /// 节目支持的屏幕
        /// </summary>
        [Display(Name = "ScreenInfo")]
        public string ScreenInfo { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        [Display(Name = "LaunchTime")]
        public string LaunchTime { get; set; }

        /// <summary>
        /// 下线时间
        /// </summary>

        [Display(Name = "ExpiryDate")]
        public string ExpiryDate { get; set; }

        /// <summary>
        /// 节目列表
        /// </summary>
        [Display(Name = "ProgFiles")]
        public List<Input_PrgFile> ProgFiles { get; set; }


        /// <summary>
        /// 切换模式
        /// </summary>
        [StringLength(50)]
        [Display(Name = "SwitchMode")]
        public string SwitchMode { get; set; }

        /// <summary>
        /// 切换时间
        /// </summary>
        [Display(Name = "SwitchTime")]
        public int SwitchTime { get; set; }

        /// <summary>
        /// 屏幕适应
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ScreenMatch")]
        public string ScreenMatch { get; set; }

    }

    public class Input_EditProg
    {

        /// <summary>
        /// 节目编码
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 节目支持的屏幕
        /// </summary>
        [Display(Name = "ScreenInfo")]
        public string ScreenInfo { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        [Display(Name = "LaunchTime")]
        public string LaunchTime { get; set; }

        /// <summary>
        /// 下线时间
        /// </summary>

        [Display(Name = "ExpiryDate")]
        public string ExpiryDate { get; set; }

        /// <summary>
        /// 节目名称
        /// </summary>
        [Required]
        [Display(Name = "ProgramName")]
        public string ProgramName { get; set; }

        /// <summary>
        /// 文件ID
        /// </summary>
        [Display(Name = "FileCode")]
        public string FileCode { get; set; }


        /// <summary>
        /// 文件ID
        /// </summary>
        [Display(Name = "PreviewFileGUID")]
        public string PreviewFileGUID { get; set; }


        /// <summary>
        /// 切换模式
        /// </summary>
        [StringLength(50)]
        [Display(Name = "SwitchMode")]
        public string SwitchMode { get; set; }

        /// <summary>
        /// 切换时间
        /// </summary>
        [Display(Name = "SwitchTime")]
        public int SwitchTime { get; set; }

        /// <summary>
        /// 屏幕适应
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ScreenMatch")]
        public string ScreenMatch { get; set; }

    }

    public class Input_GetProgList : Pagination
    {

        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        [Display(Name = "SearchKey")]
        public string SearchKey { get; set; }

        /// <summary>
        /// 节目类型
        /// </summary>
        [Display(Name = "ProgType")]
        public string ProgType { get; set; }

        /// <summary>
        /// 屏幕编码
        /// </summary>
        [Display(Name = "ScreenCode")]
        public string ScreenCode { get; set; }

        /// <summary>
        /// 切换模式
        /// </summary>
        [Display(Name = "SwitchMode")]
        public string SwitchMode { get; set; }

        /// <summary>
        /// 屏幕适应
        /// </summary>
        [Display(Name = "ScreenMatch")]
        public string ScreenMatch { get; set; }

    }

    public class Input_ProEdit : Input_ProAdd
    {
        /// <summary>
        /// 节目ID
        /// </summary>
        [Display(Name = "FileCode")]
        public string FileCode { get; set; }
    }

    public class Input_ProgramAdd
    {
        /// <summary>
        /// 节目ID
        /// </summary>
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 节目名称
        /// </summary>
        [Required]
        [Display(Name = "ProgramName")]
        public string ProgramName { get; set; }


        /// <summary>
        /// 节目支持的屏幕
        /// </summary>
        [Display(Name = "ProgScreenInfo")]
        public string ProgScreenInfo { get; set; }


        /// <summary>
        /// 切换模式
        /// </summary>
        [StringLength(50)]
        [Display(Name = "SwitchMode")]
        public string SwitchMode { get; set; }

        /// <summary>
        /// 切换时间
        /// </summary>
        [Display(Name = "SwitchTime")]
        public int SwitchTime { get; set; }

        /// <summary>
        /// 屏幕适应
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ScreenMatch")]
        public string ScreenMatch { get; set; }

        ///// <summary>
        ///// 上线时间
        ///// </summary>
        //[DataType(DataType.DateTime)]
        //[Display(Name = "LaunchTime")]
        //public DateTime LaunchTime { get; set; }

        ///// <summary>
        ///// 下线时间
        ///// </summary>
        //[DataType(DataType.DateTime)]
        //[Display(Name = "ExpiryDate")]
        //public DateTime ExpiryDate { get; set; }


        /// <summary>
        /// 上线时间
        /// </summary>

        [Display(Name = "LaunchTime")]
        public string LaunchTime { get; set; }

        /// <summary>
        /// 下线时间
        /// </summary>

        [Display(Name = "ExpiryDate")]
        public string ExpiryDate { get; set; }



    }


    public class Input_ProgramDel
    {
        public List<string> Codes { get; set; }
    }
    /// <summary>
    /// 创建节目
    /// </summary>
    public class Input_CreateProgram
    {
        /// <summary>
        /// 节目名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ProgramName")]
        public string ProgramName { get; set; }

        /// <summary>
        /// 节目文件
        /// </summary>
        [Display(Name = "ID")]
        public long ID { get; set; }

        /// <summary>
        /// 节目支持的屏幕
        /// </summary>
        [Display(Name = "ProgScreenInfo")]
        public int ProgScreenInfo { get; set; }


        /// <summary>
        /// 切换模式
        /// </summary>

        [Display(Name = "SwitchMode")]
        public int SwitchMode { get; set; }


        /// <summary>
        /// 切换时间
        /// </summary>
        [Display(Name = "SwitchTime")]
        public int SwitchTime { get; set; }


        /// <summary>
        /// 屏幕适应
        /// </summary>
        [Display(Name = "SwitchMode")]
        public int ScreenMatch { get; set; }


        /// <summary>
        /// 上线时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "LaunchTime")]
        public DateTime LaunchTime { get; set; }

        /// <summary>
        /// 预览图片
        /// </summary>
        [Display(Name = "PreviewFileID")]
        public long PreviewFileID { get; set; }

        /// <summary>
        /// 下线时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "ExpiryDate")]
        public DateTime ExpiryDate { get; set; }
    }

    public class Input_ProgramUpload
    {
        /// <summary>
        /// 节目名称
        /// </summary>
        [Required]
        [Display(Name = "ProgramName")]
        public string ProgramName { get; set; }

        /// <summary>
        /// 节目支持的屏幕
        /// </summary>
        [Display(Name = "ProgScreenInfo")]
        public string ProgScreenInfo { get; set; }


        /// <summary>
        /// 切换模式
        /// </summary>
        [StringLength(50)]
        [Display(Name = "SwitchMode")]
        public string SwitchMode { get; set; }

        /// <summary>
        /// 切换时间
        /// </summary>
        [Display(Name = "SwitchTime")]
        public int SwitchTime { get; set; }

        /// <summary>
        /// 屏幕适应
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ScreenMatch")]
        public string ScreenMatch { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "LaunchTime")]
        public DateTime LaunchTime { get; set; }

        /// <summary>
        /// 下线时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "ExpiryDate")]
        public DateTime ExpiryDate { get; set; }
    }


    public class Input_QueryProgramList
    {

        /// <summary>
        /// 屏幕类型ID
        /// </summary>
        public int? ScreenID { get; set; }

    }

    /*
    public class Input_GetProgramList
    {

        public string Type { get; set; }

        public string ScreenCode { get; set; }

        public string OrderBy { get; set; }

    }

    */
    public class Input_GetAllPList : Pagination
    {
        public string ScreenCode { get; set; }

        public string OrderBy { get; set; }
    }


    public class Input_EmergencyNews
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        /// <summary>
        /// 节目名称
        /// </summary>
        [Required]
        [Display(Name = "FileName")]
        public string FileName { get; set; }

        /// <summary>
        /// 文件ID
        /// </summary>
        [Display(Name = "FileCode")]
        public string FileCode { get; set; }

        /// <summary>
        /// 支持的屏幕
        /// </summary>
        [Display(Name = "ScreenInfo")]
        public string ScreenInfo { get; set; }


        /// <summary>
        /// 文件ID
        /// </summary>
        [Display(Name = "Duration")]
        public int Duration { get; set; }

    }

    public class Input_GetProgramListByCode
    {

        /// <summary>
        /// 节目组编码
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }
    }
}
