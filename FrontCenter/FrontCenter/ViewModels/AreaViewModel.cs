using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class AreaViewModel
    {
    }

    public class Input_AreaAdd
    {

        /// <summary>
        /// 区域名称
        /// </summary>
        [Display(Name = "AreaName")]
        public string AreaName { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }
    }

    public class Input_AreaEdit : Input_AreaAdd
    {
        /// <summary>
        /// 区域编码
        /// </summary>
        [Display(Name = "AreaCode")]
        public string AreaCode { get; set; }
    }


    public class Input_AreaDel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public List<string> Code { get; set; }
    }

    public class Input_GetAreaList : Pagination
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

    public class Input_GetAreaInfo
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
        /// 区域编码
        /// </summary>
        [Display(Name = "AreaCode")]
        public string AreaCode { get; set; }

    }
}
