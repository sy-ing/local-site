using FrontCenter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    /// <summary>
    /// 超级管理员信息
    /// </summary>
    public class Input_Mgr
    {
        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 电子哟想
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 密码（密文）
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 客户编码
        /// </summary>
        public string CusID { get; set; }

    }

    public class Input_CdnBase
    {
        /// <summary>
        /// 时间校验
        /// </summary>
        [Display(Name = "CheckTime")]
        public DateTime CheckTime { get; set; }

        /// <summary>
        /// 认证字符串
        /// </summary>
        [Display(Name = "Token")]
        public string Token { get; set; }
    }

    public class Input_MallCode : Input_CdnBase
    {
        public string CusID { get; set; }

        public string MallName { get; set; }



    }

    public class Input_DevSyn : Input_CdnBase
    {
        public string CusID { get; set; }

        //   public string CallBack { get; set; }
        /// <summary>
        /// 设备
        /// </summary>
        public List<Device> Device { get; set; }

        /// <summary>
        /// 设备坐标
        /// </summary>
        public List<DeviceCoordinate> DeviceCoordinate { get; set; }


        /// <summary>
        /// 设备组
        /// </summary>
        public List<DeviceGroup> Group { get; set; }

        /// <summary>
        /// 设备组到设备关系
        /// </summary>
        public List<DeviceToGroup> DevToGroup { get; set; }

        /// <summary>
        /// 屏幕
        /// </summary>
        public List<Input_Screen> Screen { get; set; }
    }

    public class Input_GetFunInfo : Input_CdnBase
    {
        public string CusID { get; set; }

        public string FunName { get; set; }
    }

    public class Input_SetADStatistics : Input_CdnBase
    {
        public string CusID { get; set; }

        public int ADNum { get; set; }

        public int PublishADNum { get; set; }

        public DateTime StatisticsDate { get; set; }
    }


    public class Input_SetMallVersion : Input_CdnBase
    {
        public string CusID { get; set; }
        public string SysVersion { get; set; }
    }

    public class Input_MallUserSyn : Input_CdnBase
    {
        public string CusID { get; set; }

        /// <summary>
        /// 本地用户
        /// </summary>
        public List<Input_MallUser> MallUser { get; set; }
    }


    public class Input_MallInfo
    {

        public string MallName { get; set; }

        public Input_Mgr MallUser { get; set; }

        public List<Input_MallBuilding> Blist { get; set; }

        public List<Input_MallArea> Arealist { get; set; }
    }


    public class Input_DevInfo
    {
        /// <summary>
        /// 设备
        /// </summary>
        public List<Device> Device { get; set; }

        /// <summary>
        /// 设备组
        /// </summary>
        public List<DeviceGroup> Group { get; set; }

        /// <summary>
        /// 设备组到设备关系
        /// </summary>
        public List<DeviceToGroup> DevToGroup { get; set; }

        /// <summary>
        /// 屏幕
        /// </summary>
        public List<Input_Screen> Screen { get; set; }
    }

    public class Input_MallUserInfo
    {
        /// <summary>
        /// 本地用户
        /// </summary>
        public List<Input_MallUser> MallUser { get; set; }

    }




    public class Input_MallUser : Base
    {
        /// <summary>
        /// 账户名称
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "PassWord")]
        public string PassWord { get; set; }




        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "NickName")]
        public string NickName { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "AvatarSrc")]
        public string AvatarSrc { get; set; }


        /// <summary>
        /// 性别
        /// </summary>
        [StringLength(10)]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Birthday")]
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [Required]
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }


        /// <summary>
        /// 是否可用
        /// </summary>
        [Required]
        [Display(Name = "Activity")]
        public bool Activity { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        [StringLength(100)]
        [Display(Name = "LastLoginIP")]
        public string LastLoginIP { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "LastLoginTime")]
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 登录标记
        /// </summary>
        [StringLength(100)]
        [Display(Name = "LoginSession")]
        public String LoginSession { get; set; }
    }

    public class Input_Screen : Base
    {
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(50)]
        [Display(Name = "SName")]
        public string SName { get; set; }
    }


    public class Input_MallBuilding
    {
        /// <summary>
        /// 楼栋名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "BuildingName")]
        public string BuildingName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "Order")]
        public int Order { get; set; }

        /// <summary>
        /// 楼层信息
        /// </summary>
        public List<Input_MallFloor> Floors { get; set; }
    }

    public class Input_MallFloor
    {

        /// <summary>
        /// 楼层名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "FloorName")]
        public string FloorName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "Order")]
        public int Order { get; set; }
    }
    /// <summary>
    /// 区域信息
    /// </summary>
    public class Input_MallArea
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "AreaName")]
        public string AreaName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "Order")]
        public int Order { get; set; }
    }

    /// <summary>
    /// Cdn身份认证
    /// </summary>
    public class Input_CdnIdentity
    {
        /// <summary>
        /// 客户唯一编码
        /// </summary>
        [Display(Name = "CusID")]
        public string CusID { get; set; }

        /// <summary>
        /// 时间校验
        /// </summary>
        [Display(Name = "CheckTime")]
        public DateTime CheckTime { get; set; }

        /// <summary>
        /// 认证字符串
        /// </summary>
        [Display(Name = "Token")]
        public string Token { get; set; }

    }

    public class Input_CDNGetFileInfo
    {
        /// <summary>
        /// 文件编码
        /// </summary>
        [Display(Name = "FileCode")]
        public string FileCode { get; set; }
    }


    public class Input_GetDeviceBill : Input_CdnIdentity
    {
        public string EffecDate { get; set; }
    }

    public class Input_GetMaterialInfo : Input_CdnIdentity
    {
        public List<string> ScheduleCode { get; set; }

        public List<string> DeviceShopRelateCode { get; set; }
    }

    public class Input_SendProg : Input_CdnIdentity
    {
        /// <summary>
        /// 节目编码
        /// </summary>
        [Display(Name = "ProgCode")]
        public string ProgCode { get; set; }
    }

    public class Input_SendSN : Input_CdnIdentity
    {
        /// <summary>
        /// 节目编码
        /// </summary>
        [Display(Name = "SNCode")]
        public string SNCode { get; set; }
    }
    public class Input_GetMallMapInfo
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }
}
