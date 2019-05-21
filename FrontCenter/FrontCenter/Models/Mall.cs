using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class Mall : Base
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ContractNo")]
        public string ContractNo { get; set; }

        /// <summary>
        /// 省份编号
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ProvinceID")]
        public string ProvinceID { get; set; }

        /// <summary>
        /// 城市编号
        /// </summary>
        [StringLength(255)]
        [Display(Name = "CityID")]
        public string CityID { get; set; }


        /// <summary>
        /// 地区编号
        /// </summary>
        [StringLength(255)]
        [Display(Name = "AreaID")]
        public string AreaID { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Address")]
        public string Address { get; set; }


        /// <summary>
        /// 设备数量
        /// </summary>
        [Display(Name = "DeviceNum")]
        public int DeviceNum { get; set; }

        /// <summary>
        /// 建筑面积
        /// </summary>
        [Display(Name = "ConstructionArea")]
        public int ConstructionArea { get; set; }

        /// <summary>
        /// 联系人（姓名）
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Contacts")]
        public string Contacts { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [StringLength(255)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "ContactPhone")]
        public string ContactPhone { get; set; }

        /// <summary>
        /// 联系邮箱
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ContactEmail")]
        [EmailAddress]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 营业时间
        /// </summary>
        [Display(Name = "OpenTime")]
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// 歇业时间
        /// </summary>
        [Display(Name = "CloseTime")]
        public DateTime CloseTime { get; set; }

        /// <summary>
        /// 注册码
        /// </summary>
        [StringLength(255)]
        [Display(Name = "RegKey")]
        public string RegKey { get; set; }

        /// <summary>
        /// 服务器Mac地址
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ServerMac")]
        public string ServerMac { get; set; }

        /// <summary>
        /// 到期时间
        /// </summary>
        [Display(Name = "ExpTime")]
        public DateTime ExpTime { get; set; }


        [Display(Name = "RegTime")]
        public DateTime RegTime { get; set; }
    }
}
