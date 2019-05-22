using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class ProgramFile : Base
    {
        /// <summary>
        /// 店铺编码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ShopCode")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 审核状态  1-待审核  2-审核通过 3审核拒绝  4-下架
        /// </summary>
        [Display(Name = "Status")]
        public int Status { get; set; }


        /// <summary>
        /// 原因
        /// </summary>
        [Display(Name = "Reason")]
        [StringLength(2000)]
        public string Reason { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [Display(Name = "PlacingNum")]
        [StringLength(50)]
        public string PlacingNum { get; set; }



        /// <summary>
        /// 操作者编码
        /// </summary>
        [Display(Name = "MgrCode")]
        [StringLength(50)]
        public string MgrCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "Info")]
        [StringLength(2000)]
        public string Info { get; set; }

        /// <summary>
        /// 节目类型
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ProgType")]
        public string ProgType { get; set; }

        /// <summary>
        /// 节目文件
        /// </summary>
        [Display(Name = "ProgFile")]
        [StringLength(50)]
        public string ProgFile { get; set; }

        /// <summary>
        /// 目标设备组
        /// </summary>
        [Display(Name = "DevGroup")]
        [StringLength(50)]
        public string DevGroup { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "LaunchTime")]
        public DateTime LaunchTime { get; set; }

        /// <summary>
        /// 下线时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "ExpiryDate")]
        public DateTime ExpiryDate { get; set; }


    }
}
