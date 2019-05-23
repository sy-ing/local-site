using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 停车位
    /// </summary>
    public class ParkingSpace : Base
    {


        /// <summary>
        /// 停车场编码
        /// </summary>
        [Display(Name = "ParkCode")]
        [StringLength(50)]
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


        /// <summary>
        /// 被删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }


    }
}
