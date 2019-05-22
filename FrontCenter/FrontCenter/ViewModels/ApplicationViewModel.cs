using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class ApplicationViewModel
    {
    }

    public class Input_AppDevListByCode
    {
        public string Code { get; set; }
    }

    /// <summary>
    /// 应用背景设置
    /// </summary>
    public class Input_ContainerSet
    {

        /// <summary>
        /// 文件编码
        /// </summary>
        [Display(Name = "FileCode")]
        public string FileCode { get; set; }

        /// <summary>
        /// 分辨率编码
        /// </summary>
        [Display(Name = "ScreenCode")]
        public string ScreenCode { get; set; }
    }

    public class Input_GetContainerBGByScreen
    {
        public string Code { get; set; }
    }



    public class Input_AppInfo
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "AppName")]
        public string AppName { get; set; }

        /// <summary>
        /// 应用简介
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Introduction")]
        public string Introduction { get; set; }

        /// <summary>
        /// 应用文件ID
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "AppFileID")]
        public int AppFileID { get; set; }

        /// <summary>
        /// 屏幕信息（分辨率-横竖屏）
        /// </summary>

        [Display(Name = "ScreenInfo")]
        public int ScreenInfo { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "LaunchTime")]
        public DateTime LaunchTime { get; set; }

        /// <summary>
        /// 预览图片
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "PreviewFile")]
        public int PreviewFile { get; set; }

        /// <summary>
        /// 下线时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "ExpiryDate")]
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Version")]
        public string Version { get; set; }
    }
    public class Input_DelAppNew
    {
        public string AppCode { get; set; }
    }
    public class Input_GetDeviceByAppCode
    {
        public string AppCode { get; set; }
    }
    public class Input_AppInfoEdit
    {

        /// <summary>
        /// 应用文件ID
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "FileCode")]
        public string FileCode { get; set; }


        /// <summary>
        /// Icon图片文件的ID
        /// </summary>
        [Display(Name = "IconFileCode")]
        public string IconFileCode { get; set; }

        /// <summary>
        /// android对应的命名空间
        /// </summary>
        [Display(Name = "Namespace")]
        public string Namespace { get; set; }

        /// <summary>
        /// 1：PC   2:android
        /// </summary>
        [Display(Name = "PlatformType")]
        public int PlatformType { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Version")]
        public string Version { get; set; }

        /// <summary>
        /// 服务器地址
        /// </summary>
        [Display(Name = "ServerUrl")]
        public string ServerUrl { get; set; }

        /// <summary>
        /// 启动项文件名
        /// </summary>
        [Display(Name = "Startup")]
        public string Startup { get; set; }

        /// <summary>
        /// 应用编码
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }
    }

    public class Input_AppInfoNew
    {


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
        /// 应用分类
        /// </summary>
        [Display(Name = "Appclass")]
        public string AppClass { get; set; }

        /// <summary>
        /// 应用二级分类
        /// </summary>
        [Display(Name = "AppSecClass")]
        public string AppSecClass { get; set; }

        /// <summary>
        /// 应用文件ID
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "FileCode")]
        public string FileCode { get; set; }

        /// <summary>
        /// 屏幕信息（分辨率-横竖屏）
        /// </summary>

        [Display(Name = "ScreenInfo")]
        public string ScreenInfo { get; set; }


        /// <summary>
        /// Icon图片文件的ID
        /// </summary>
        [Display(Name = "IconFileCode")]
        public string IconFileCode { get; set; }

        /// <summary>
        /// android对应的命名空间
        /// </summary>
        [Display(Name = "Namespace")]
        public string Namespace { get; set; }

        /// <summary>
        /// 1：PC   2:android
        /// </summary>
        [Display(Name = "PlatformType")]
        public int PlatformType { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Version")]
        public string Version { get; set; }

        /// <summary>
        /// 服务器地址
        /// </summary>
        [Display(Name = "ServerUrl")]
        public string ServerUrl { get; set; }

        /// <summary>
        /// 启动项文件名
        /// </summary>
        [Display(Name = "Startup")]
        public string Startup { get; set; }
    }


    public class Input_GetClassList
    {
        public string AppClassCode { get; set; }
    }

    /// <summary>
    /// 应用发布
    /// </summary>
    public class Input_PublishAppToDevice
    {
        /// <summary>
        /// 应用编码
        /// </summary>
        [Display(Name = "AppCode")]
        public string AppCode { get; set; }

        /// <summary>
        /// 设备Code列表
        /// </summary>
        [Display(Name = "DeviceCodes")]
        public List<string> DeviceCodes { get; set; }
    }

    public class Input_AppAdd
    {
        public string IP { get; set; }

        public List<string> AppName { get; set; }
    }




    public class Input_SetApp
    {
        public List<Input_SetAppTS> App { get; set; }
    }

    public class Input_SetAppTS
    {
        public string AppCode { get; set; }

        public List<string> TimeSlot { get; set; }

        //0 不是 1 是
        public int Default { get; set; }
    }


    public class Input_AppDevList
    {
        public string DevCode { get; set; }
    }

    public class Input_AppDevListByIP
    {
        public string IP { get; set; }
    }
    /// <summary>
    /// 程序钟输入
    /// </summary>
    public class Input_AppClock
    {
        [Display(Name = "AppID")]
        public int AppID { get; set; }

        [Display(Name = "DevID")]
        public int DevID { get; set; }

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


    /// <summary>
    /// 应用-用户编辑
    /// </summary>
    public class Input_EditUserApp
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name = "UserIDS")]
        public List<int> UserIDS { get; set; }



        /// <summary>
        /// 权限编码列表
        /// </summary>
        [Display(Name = "AppCode")]
        public List<string> AppCode { get; set; }

    }

    /// <summary>
    /// 应用-用户编辑
    /// </summary>
    public class Input_EditUserAppNew
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name = "UserCodeList")]
        public List<string> UserCodeList { get; set; }



        /// <summary>
        /// 权限编码列表
        /// </summary>
        [Display(Name = "AppCode")]
        public List<string> AppCode { get; set; }

    }

    public class Input_GetUserAppNew
    {
        public string UserCode { get; set; }
    }

    public class Input_GetAppUserListNew
    {

        public string AppCode { get; set; }
    }
    public class Output_AppUser
    {



        /// <summary>
        /// 用户ID
        /// </summary>
        [Key]
        [Display(Name = "UserID")]
        public int UserID { get; set; }


        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "NickName")]
        public string NickName { get; set; }


        /// <summary>
        /// 状态 是否有权限
        /// </summary>
        [Display(Name = "Status")]
        public int Status { get; set; }

    }

    public class Output_AppUserNew
    {



        /// <summary>
        /// 用户ID
        /// </summary>
        [Key]
        [Display(Name = "UserID")]
        public int UserID { get; set; }


        [Display(Name = "UserCode")]
        public string UserCode { get; set; }


        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "NickName")]
        public string NickName { get; set; }


        /// <summary>
        /// 状态 是否有权限
        /// </summary>
        [Display(Name = "Status")]
        public int Status { get; set; }

    }
}
