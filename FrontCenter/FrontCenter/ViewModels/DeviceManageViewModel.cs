using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class DeviceManageViewModel
    {
        public class Input_GetProgramList_Local
        {

            [StringLength(50)]
            [Display(Name = "Code")]
            public string Code { get; set; }
        }

        public class Outpput_ProgList
        {
            /// <summary>
            /// 账户ID
            /// </summary>
            [Required]
            [Key]
            [Display(Name = "ID")]
            public int ID { get; set; }

            /// <summary>
            /// 节目链接地址
            /// </summary>
            [StringLength(2000)]
            [Display(Name = "IMG")]
            public string IMG { get; set; }




            /// <summary>
            /// 切换模式
            /// </summary>
            [StringLength(50)]
            [Display(Name = "Effect")]
            public string Effect { get; set; }

            /// <summary>
            /// 切换时间
            /// </summary>
            [Display(Name = "Time")]
            public int Time { get; set; }

            /// <summary>
            /// 屏幕适应
            /// </summary>
            [StringLength(50)]
            [Display(Name = "ScreenMatch")]
            public string ScreenMatch { get; set; }

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


        public class Output_ProgramList
        {
            /// <summary>
            /// 账户ID
            /// </summary>
            [Required]
            [Key]
            [Display(Name = "ID")]
            public int ID { get; set; }

            /// <summary>
            /// 节目链接地址
            /// </summary>
            [StringLength(2000)]
            [Display(Name = "ProgSrc")]
            public string ProgSrc { get; set; }




            /// <summary>
            /// 切换模式
            /// </summary>
            [StringLength(50)]
            [Display(Name = "SwitchMode")]
            public string SwitchMode { get; set; }

            /// <summary>
            /// 切换时间
            /// </summary>
            [Display(Name = "SwitchTime")]
            public int SwitchTime { get; set; }

            /// <summary>
            /// 屏幕适应
            /// </summary>
            [StringLength(50)]
            [Display(Name = "ScreenMatch")]
            public string ScreenMatch { get; set; }

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

            /// <summary>
            /// 排序
            /// </summary>
            [Display(Name = "Order")]
            public int Order { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [DataType(DataType.DateTime)]
            [Display(Name = "AddTime")]
            public DateTime AddTime { get; set; }
        }
    }
}
