using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 用户角色对应关系
    /// </summary>
    public class UserRoles:Base
    {
  

        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        [Display(Name = "UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [Required]
        [Display(Name = "RoleCode")]
        public string RoleCode { get; set; }
    }
}
