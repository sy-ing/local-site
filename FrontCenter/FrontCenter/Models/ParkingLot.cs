using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 停车场
    /// </summary>
    public class ParkingLot : Base
    {


        /// <summary>
        /// 楼层ID
        /// </summary>
        [Display(Name = "FloorCode")]
        [StringLength(50)]
        public string FloorCode { get; set; }

        /// <summary>
        /// 被删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }

    }
}
