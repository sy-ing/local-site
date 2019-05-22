using FrontCenter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class DeviceGroupViewModel
    {
    }

    /// <summary>
    /// 添加设备组
    /// </summary>
    public class Input_DeviceGroup
    {
        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }


        /// <summary>
        /// 设备组名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "GName")]
        public string GName { get; set; }

        /// <summary>
        /// 显示屏规格
        /// </summary>
        [Display(Name = "ScreenInfoCode")]
        public string ScreenInfoCode { get; set; }

        /// <summary>
        /// 设备列表
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "Devices")]
        public List<string> Devices { get; set; }

    }

    /// <summary>
    /// 添加设备组
    /// </summary>
    public class Input_DeviceGroupNew : Input_DeviceGroup
    {
        [Display(Name = "IsSync")]
        public bool IsSync { get; set; }

    }

    public class Input_GetDevGroupInfo
    {
        /// <summary>
        /// 编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }

    public class Input_ProgramToDevice : Input_DeviceGroupNew
    {
        [Display(Name = "ProgramGroupCode")]
        public string ProgramGroupCode { get; set; }
    }

    public class Input_DelDeviceGroup
    {
        /// <summary>
        /// 编码列表
        /// </summary>
        [Display(Name = "Codes")]
        public List<string> Codes { get; set; }
    }

    public class Input_DeviceGroupQuery : Pagination
    {
        /// <summary>
        /// 显示屏规格
        /// </summary>
        [Display(Name = "ScreenInfoCode")]
        public string ScreenInfoCode { get; set; }
    }

    public class Input_GetDevGroupList : Pagination
    {
        /// <summary>
        /// 关键字
        /// </summary>
        [Display(Name = "SearchKey")]
        public string SearchKey { get; set; }

    }

    public class Input_GetDevGroupListNew : Pagination
    {

        [Display(Name = "MallCode")]
        public string MallCode { get; set; }


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

    public class Output_GetDevGroupList
    {
        /// <summary>
        /// 编码
        /// </summary>
        [Key]
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "GName")]
        public string GName { get; set; }

        /// <summary>
        /// 分辨率编码
        /// </summary>
        [Display(Name = "ScreenInfoCode")]
        public string ScreenInfoCode { get; set; }


        /// <summary>
        /// 设备类型
        /// </summary>
        [Display(Name = "Type")]
        public int Type { get; set; }

        /// <summary>
        /// 分辨率
        /// </summary>
        [Display(Name = "SName")]
        public string SName { get; set; }

        /// <summary>
        /// 类型描述
        /// </summary>
        [Display(Name = "TypeStr")]
        public string TypeStr { get; set; }

        /// <summary>
        /// 设备数量
        /// </summary>
        [Display(Name = "DevCount")]
        public int DevCount { get; set; }


        /// <summary>
        /// 节目组数量
        /// </summary>
        [Display(Name = "PGCount")]
        public int PGCount { get; set; }


    }

    public class Input_GetProgramGroupByDeviceGroup
    {
        [Display(Name = "Code")]
        public string Code { get; set; }
    }
    /// <summary>
    /// 设备组信息输入
    /// </summary>
    public class Input_GroupInfo
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 设备组名称
        /// </summary>
        [StringLength(50), MinLength(1)]
        [Display(Name = "GName")]
        public string GName { get; set; }

        /// <summary>
        /// 设备列表
        /// </summary>
        [Display(Name = "Devices")]
        public List<string> Devices { get; set; }
    }

    public class Input_DelDeviceProg
    {
        /// <summary>
        /// 设备列表
        /// </summary>
        [Display(Name = "ProgramGroupCodes")]
        public List<string> ProgramGroupCodes { get; set; }
    }

    /// <summary>
    /// 设备组信息输入
    /// </summary>
    public class Input_GroupInfoNew : Input_GroupInfo
    {

        [Display(Name = "IsSync")]
        public bool IsSync { get; set; }
    }

    /// <summary>
    /// 设备组列表
    /// </summary>
    public class Output_DeviceGroupList
    {
        /// <summary>
        /// ID
        /// </summary>
        [Display(Name = "ID")]
        public int ID { get; set; }


        /// <summary>
        /// 设备组名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "GName")]
        public string GName { get; set; }

        /// <summary>
        /// 屏幕信息（分辨率-横竖屏）
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ScreenInfo")]
        public string ScreenInfo { get; set; }

        /// <summary>
        /// 包含设备数量
        /// </summary>
        [Display(Name = "DeviceCount")]
        public int DeviceCount { get; set; }
    }

    /// <summary>
    /// 设备组详细信息
    /// </summary>
    public class Output_DeviceGroupInfo
    {
        /// <summary>
        /// 设备组名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "GName")]
        public string GName { get; set; }

        /// <summary>
        /// 屏幕属性
        /// </summary>
        public ScreenInfo ScreenInfo { get; set; }

        /// <summary>
        /// 节目组ID
        /// </summary>
        public string ProgramGrounpCode { get; set; }

        /// <summary>
        /// 节目组名称
        /// </summary>
        public string ProgramGrounpName { get; set; }

        /// <summary>
        /// 设备列表
        /// </summary>
        public List<Output_DeviceShow> DevicesInfo { get; set; }




    }

    /// <summary>
    /// 列表中展示的设备信息
    /// </summary>
    public class Output_DeviceShow
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }


        /// <summary>
        /// IP地址
        /// </summary>
        [StringLength(50)]
        //[RegularExpression(@"/^(([1-9]|([1-9]\d)|(1\d\d)|(2([0-4]\d|5[0-5])))\.)(([1-9]|([1-9]\d)|(1\d\d)|(2([0-4]\d|5[0-5])))\.){2}([1-9]|([1-9]\d)|(1\d\d)|(2([0-4]\d|5[0-5])))$/")]
        [Display(Name = "IP")]
        public string IP { get; set; }

        /// <summary>
        /// 楼栋
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Building")]
        public string Building { get; set; }


        /// <summary>
        /// 楼层
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Floor")]
        public string Floor { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Position")]
        public string Position { get; set; }

    }


    public class Output_DG_Local
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [Display(Name = "Code")]
        public string Code { get; set; }


        /// <summary>
        /// 设备组名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "GName")]
        public string GName { get; set; }

        /// <summary>
        /// 屏幕信息（分辨率-横竖屏）
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ScreenInfo")]
        public string ScreenInfo { get; set; }

        /// <summary>
        /// 屏幕ID
        /// </summary>
        [Display(Name = "ScreenInfoCode")]
        public string ScreenInfoCode { get; set; }


        /// <summary>
        /// 节目组ID
        /// </summary>
        [Display(Name = "ProgramGrounpCode")]
        public string ProgramGrounpCode { get; set; }

        /// <summary>
        /// 节目组名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ProgramGrounpName")]
        public string ProgramGrounpName { get; set; }

        /// <summary>
        /// 包含设备数量
        /// </summary>
        [Display(Name = "DeviceCount")]
        public int DeviceCount { get; set; }

    }

    public class Output_DeviceGroup
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [Display(Name = "Code")]
        public string Code { get; set; }


        /// <summary>
        /// 设备组名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "GName")]
        public string GName { get; set; }

        /// <summary>
        /// 1-正常 2-同步
        /// </summary>
        [Display(Name = "Type")]
        public int Type { get; set; }


        /// <summary>
        /// 包含设备数量
        /// </summary>
        [Display(Name = "DeviceCount")]
        public int DeviceCount { get; set; }
    }
}
