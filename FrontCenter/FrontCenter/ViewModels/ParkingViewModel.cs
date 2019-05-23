using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class ParkingViewModel
    {
    }


    /// <summary>
    /// 停车位
    /// </summary>
    public class Input_ParkingSpace
    {

        /// <summary>
        /// 停车场ID
        /// </summary>
        [Display(Name = "ParkCode")]
        public string ParkCode { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Num")]
        public string Num { get; set; }

        /// <summary>
        /// 横坐标
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Xaxis")]
        public string Xaxis { get; set; }

        /// <summary>
        /// 纵坐标
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Yaxis")]
        public string Yaxis { get; set; }

        /// <summary>
        /// 横坐标
        /// </summary>
        [StringLength(255)]
        [Display(Name = "NavXaxis")]
        public string NavXaxis { get; set; }

        /// <summary>
        /// 纵坐标
        /// </summary>
        [StringLength(255)]
        [Display(Name = "NavYaxis")]
        public string NavYaxis { get; set; }


        [Display(Name = "UserName")]
        public string UserName { get; set; }

    }


    public class Input_PLEdit
    {
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        [Display(Name = "FloorCodes")]
        public List<string> FloorCodes { get; set; }

        [Display(Name = "UserName")]
        public string UserName { get; set; }
    }


    public class Input_PLL
    {
        [Display(Name = "MallCode")]
        public string MallCode { get; set; }

        [Display(Name = "UserName")]
        public string UserName { get; set; }
    }

    public class Input_GetPSL
    {

        /// <summary>
        /// 停车场ID
        /// </summary>
        [Display(Name = "ParkCode")]
        public string ParkCode { get; set; }

        /// <summary>
        /// 是否分页 0 不分页 1分页
        /// </summary>
        [DefaultValue(0)]
        [Display(Name = "Paging")]
        public int Paging { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        [Display(Name = "PageIndex")]
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        [Display(Name = "PageSize")]
        public int PageSize { get; set; }
    }
}
