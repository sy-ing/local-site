using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class DictionaryViewModel
    {
    }
    public class Input_GetDeviceOptionsNew
    {
        public string MallCode { get; set; }
    }
    public class Input_Screensaver
    {
        public int? Time { get; set; }

        public int? ScreenType { get; set; }
    }

    /// <summary>
    /// 设备相关选项
    /// </summary>
    public class Output_DeviceOptionsNew
    {
        /// <summary>
        /// 楼栋选项
        /// </summary>
        public List<Output_Building_Local> Buildings { get; set; }
        /// <summary>
        /// 楼层选项
        /// </summary>
        //  public object Floors { get; set; }

        /// <summary>
        /// 屏幕属性选项
        /// </summary>
        public object ScreenInfos { get; set; }
    }
    /// <summary>
    /// 输出楼栋信息
    /// </summary>
    public class Output_Building_Local
    {
        public string BuildingName { get; set; }



        public string Code { get; set; }

        public object Floors { get; set; }
    }
}
