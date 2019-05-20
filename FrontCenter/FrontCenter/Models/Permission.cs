using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class Permission:Base
    {

        ///// <summary>
        ///// 权限ID
        ///// </summary>
        //[Required]
        //[Key]
        //[Display(Name = "PermissionID")]
        //public int PermissionID { get; set; }



        /// <summary>
        /// 权限名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 权限名称英文
        /// </summary>
        [Required]
        [StringLength(255)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// 父权限编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "ParentCode")]
        public string ParentCode { get; set; }

    }
}
