using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{  /// <summary>
   /// 修改角色名称输入
   /// </summary>
    public class Input_RoleEdit
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Required]
        [Display(Name = "RoleID")]
        public int RoleID { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }

    public class Input_RolePerEdit
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Required]
        [Display(Name = "RoleID")]
        public int RoleID { get; set; }

        /// <summary>
        /// 权限列表
        /// </summary>
        [Display(Name = "PermissionIDs")]
        public string PermissionIDs { get; set; }

    }


    public class Input_AddRole
    {

        /// <summary>
        /// 角色名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }


        /// <summary>
        /// 角色描述
        /// </summary>
        [Display(Name = "Intro")]
        public string Intro { get; set; }

        /// <summary>
        /// 权限编码列表
        /// </summary>
        [Display(Name = "PermissionCode")]
        public List<string> PermissionCode { get; set; }

    }

    public class Input_DelRole
    {
        /// <summary>
        /// 编码列表
        /// </summary>
        [Display(Name = "Code")]
        public List<string> Code { get; set; }
    }

    public class Input_EditRole : Input_AddRole
    {
        /// <summary>
        /// 编码
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; }
    }
}
