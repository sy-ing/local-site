using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class SysLog : Base
    {


        /// <summary>
        /// 账户名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }


        /// <summary>
        /// 操作模块模块
        /// </summary>
        [StringLength(255)]
        [Display(Name = "ModuleName")]
        public string ModuleName { get; set; }


        /// <summary>
        /// 操作类型
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Type")]
        public string Type { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>

        [Display(Name = "LogMsg")]
        [StringLength(2000)]
        public string LogMsg { get; set; }


        /// <summary>
        /// 系统区域地址
        /// </summary>
        [StringLength(255)]
        [Display(Name = "SysArea")]
        public string SysArea { get; set; }


        /// <summary>
        /// IP地址
        /// </summary>
        [StringLength(255)]
        [Display(Name = "IP")]
        public string IP { get; set; }


        /// <summary>
        /// 系统模块  Manage   Mall
        /// </summary>
        [StringLength(100)]
        [Display(Name = "SystemModule")]
        public String SystemModule { get; set; }

        /// <summary>
        /// 商场编码
        /// </summary>
        [StringLength(100)]
        [Display(Name = "MallCode")]
        public String MallCode { get; set; }

    }
}
