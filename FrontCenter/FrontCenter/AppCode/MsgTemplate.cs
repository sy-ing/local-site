using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    public class MsgTemplate
    {
        /// <summary>
        /// 发送者ID
        /// </summary>
        public string SenderID { get; set; }
        /// <summary>
        /// 接受者ID
        /// </summary>
        public string ReceiverID { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public object Content { get; set; }
    }
}
