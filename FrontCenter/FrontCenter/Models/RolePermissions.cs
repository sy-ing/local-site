using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class RolePermissions : Base
    {

        /// <summary>
        /// 角色编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "RoleCode")]
        public string RoleCode { get; set; }


        /// <summary>
        ///权限编码
        /// </summary>
        [StringLength(50)]
        [Display(Name = "PermissionCode")]
        public string PermissionCode { get; set; }
    }
}
