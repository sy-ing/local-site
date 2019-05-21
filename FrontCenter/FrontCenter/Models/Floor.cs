using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class Floor : Base
    {


        /// <summary>
        /// 楼栋ID
        /// </summary>
        [Display(Name = "BuildingCode")]
        [StringLength(50)]
        public string BuildingCode { get; set; }

        /// <summary>
        /// 楼层名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 楼层名称(英文)
        /// </summary>
        [StringLength(255)]
        [Display(Name = "NameEn")]
        public string NameEn { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "Order")]
        public int Order { get; set; }

        /// <summary>
        /// 楼层地图
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Map")]
        public string Map { get; set; }

        /// <summary>
        /// 是否被删除
        /// </summary>
        [Display(Name = "IsDel")]
        public bool IsDel { get; set; }

        /// <summary>
        /// 地图信息
        /// </summary>
        [Display(Name = "MapInfo")]
        public string MapInfo { get; set; }

    }
}
