using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class UserViewModel
    {
    }



    public class Input_UserRegister
    {
        public RegisterUser Parameter { get; set; }
    }

    /// <summary>
    /// 注册用户
    /// </summary>
    public class RegisterUser
    {

        /// <summary>
        /// 账户名称
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "账户名称长度应该为1-100个字符", MinimumLength = 1)]
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "用户密码至少需要6个字符", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// 密码确认
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "二次输入的密码不一致")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "NickName")]
        public string NickName { get; set; }



        /// <summary>
        /// 手机
        /// </summary>
        [Required]
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        [Display(Name = "RoleCode")]
        public string RoleCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "Remark")]
        public String Remark { get; set; }

    }


    public class Input_ChangePwd
    {
        /// <summary>
        /// 账户ID
        /// </summary>
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// 旧密码
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "OldPassword")]
        public string OldPassword { get; set; }


        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "用户密码至少需要6个字符", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// 密码确认
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "二次输入的密码不一致")]
        public string ConfirmPassword { get; set; }


    }
    public class Input_ChangePassWord
    {
        /// <summary>
        /// 账户ID
        /// </summary>
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }


        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "用户密码至少需要6个字符", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// 密码确认
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "二次输入的密码不一致")]
        public string ConfirmPassword { get; set; }


    }

    public class Input_UserEditInfo
    {

        /// <summary>
        /// 昵称
        /// </summary>

        [StringLength(1000)]
        [Display(Name = "NickName")]
        public string NickName { get; set; }

        /// <summary>
        /// 手机
        /// </summary>

        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

    }

    public class Input_UserEdit
    {
        /// <summary>
        /// 账户名
        /// </summary>
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>

        [StringLength(1000)]
        [Display(Name = "NickName")]
        public string NickName { get; set; }

        /// <summary>
        /// 手机
        /// </summary>

        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        [Display(Name = "RoleCode")]
        public string RoleCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "Remark")]
        public String Remark { get; set; }
    }


    /// <summary>
    /// 获取用户列表时应提供的数据
    /// </summary>
    public class Input_QueryUser
    {


        /// <summary>
        /// 是否分页 0 不分页 1分页
        /// </summary>
        [Required]
        [Display(Name = "Paging")]
        public int Paging { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        [Display(Name = "PageIndex")]
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        [Display(Name = "PageSize")]
        public int PageSize { get; set; }
    }

    /// <summary>
    /// 删除
    /// </summary>
    public class Input_Del
    {
        public List<int> IDS { get; set; }
    }

    public class Input_SetWE
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name = "ID")]
        public int? ID { get; set; }

        /// <summary>
        /// 状态  
        /// </summary>
        [Display(Name = "Status")]
        public int? Status { get; set; }
    }

    public class Output_UserList
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name = "ID")]
        public int ID { get; set; }

        /// <summary>
        /// 账户名
        /// </summary>
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>

        [StringLength(255)]
        [Display(Name = "NickName")]
        public string NickName { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        [StringLength(255)]
        [Display(Name = "RoleName")]
        public string RoleName { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "AddTime")]
        public string AddTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "Status")]
        public int Status { get; set; }
    }


    public class Input_UserCheck
    {
        public string CheckFieldName { get; set; }

        public string CheckFieldValue { get; set; }

        public string UserCode { get; set; }

    }
}
