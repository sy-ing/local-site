using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class DeviceIOTViewModel
    {
    }

    public class Send_PubMsg
    {
        public string MsgTemplate { get; set; }
    }

    public class Post_AddDevice
    {
        /// <summary>
        /// 设备名（设备编码）
        /// </summary>
        public string deviceName { get; set; }
        /// <summary>
        /// 模板ID
        /// </summary>
        public string schemaId { get; set; }
        /// <summary>
        /// 设备描述
        /// </summary>
        public string description { get; set; }
    }

    public class Post_GetShadowList
    {

        /// <summary>
        /// 表示取第几页，默认1
        /// </summary>
        public int qpageNo { get; set; }
        /// <summary>
        /// 每页返回的数目，默认10
        /// </summary>
        public int qpageSize { get; set; }
        /// <summary>
        /// name，createTime，lastActiveTime，默认name
        /// </summary>
        public string orderBy { get; set; }
        /// <summary>
        /// 按升序或降序返回结果，desc / asc，默认asc
        /// </summary>
        public string order { get; set; }
        /// <summary>
        /// 查询属性名，默认全量
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 查询属性名对应的值，默认全量
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 收藏，true / false /all，默认all
        /// </summary>
        public string favourite { get; set; }

    }

    public class Post_DelDevice
    {
        public List<string> deviceList { get; set; }
    }


}
