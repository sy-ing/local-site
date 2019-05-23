using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class BuildingViewModel
    {
    }

    public class Input_BuildingAdd
    {

        /// <summary>
        /// 楼栋名称
        /// </summary>
        [Display(Name = "BuildingName")]
        public string BuildingName { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }

    public class Input_BuildingEdit : Input_BuildingAdd
    {
        /// <summary>
        /// 楼栋编码
        /// </summary>
        [Display(Name = "BuildingCode")]
        public string BuildingCode { get; set; }
    }


    public class Input_BuildingDel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public List<string> Code { get; set; }
    }

    public class Input_GetBuildingList : Pagination
    {
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

    public class Input_GetBuildingListByUnionID
    {
        /// <summary>
        /// 商户编码
        /// </summary>
        [Display(Name = "UnionID")]
        public string UnionID { get; set; }
    }

    public class Input_GetBuildingInfo
    {
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

        /// <summary>
        /// 楼栋编码
        /// </summary>
        [Display(Name = "BuildingCode")]
        public string BuildingCode { get; set; }

    }
}
