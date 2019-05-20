using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    /// <summary>
    /// 账户模型
    /// </summary>
    public class Account : Base
    {


        /// <summary>
        /// 账户名称
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "PassWord")]
        public string PassWord { get; set; }




        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(200)]
        [Display(Name = "NickName")]
        public string NickName { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        [StringLength(200)]
        [Display(Name = "AvatarSrc")]
        public string AvatarSrc { get; set; }


        /// <summary>
        /// 性别
        /// </summary>
        [StringLength(10)]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Birthday")]
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [Required]
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }


        /// <summary>
        /// 是否可用
        /// </summary>
        [Required]
        [Display(Name = "Activity")]
        public bool Activity { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        [StringLength(100)]
        [Display(Name = "LastLoginIP")]
        public string LastLoginIP { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "LastLoginTime")]
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 登录标记
        /// </summary>
        [StringLength(100)]
        [Display(Name = "LoginSession")]
        public String LoginSession { get; set; }


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

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "Remark")]
        public String Remark { get; set; }

    }
}
