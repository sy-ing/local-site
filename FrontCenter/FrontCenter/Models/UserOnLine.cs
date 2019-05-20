using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class UserOnLine
    {

        public string UserName { get; set; }

        public string MallCode { get; set; }

        public string UserCode { get; set; }

        /// <summary>
        /// 系统模块  Manage   Mall
        /// </summary>
        public String SystemModule { get; set; }

        public int ID { get; set; }
    }
}
