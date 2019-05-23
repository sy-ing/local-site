using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class ConstructionViewModel
    {
    }

    /// <summary>
    /// 楼层编辑
    /// </summary>
    public class Input_FloorEdit
    {
        public string Code { get; set; }

        public string FileGUID { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

    }

    public class Input_GetFloorInfo_Local
    {
        public string Code { get; set; }
    }


    public class Input_GetAreaList_Local
    {
        public string MallCode { get; set; }
    }


    /// <summary>
    /// 建筑信息
    /// </summary>
    public class Input_ConstructionInfo
    {
        /// <summary>
        /// 楼栋信息
        /// </summary>
        public Input_Building[] Buildings { get; set; }
    }

    /// <summary>
    /// 输入楼栋
    /// </summary>
    public class Input_Building
    {
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 名称(英文)
        /// </summary>
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "NameEn")]
        public string NameEn { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Order { get; set; }

        /// <summary>
        /// 楼层信息
        /// </summary>
        public Input_FloorAdd[] Floors { get; set; }
    }


    public class Input_FloorAdd
    {


        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 名称(英文)
        /// </summary>
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "NameEn")]
        public string NameEn { get; set; }

        /// <summary>
        /// 地图文件
        /// </summary>
        public long Map { get; set; }

        /// <summary>
        /// 建筑ID
        /// </summary>
        public int? BuildingID { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Order { get; set; }
    }

    /// <summary>
    /// 输出楼栋信息
    /// </summary>
    public class Output_Building
    {
        public string BuildingName { get; set; }

        public int ID { get; set; }

        public object Floors { get; set; }
    }
}
