using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class Input_StoreNewsAdd
    {

        public string News { get; set; }

        public DateTime ValidityPeriod { get; set; }

        public List<string> Imgs { get; set; }

        public string UnionID { get; set; }
    }


    public class Input_StoreNewsEdit : Input_StoreNewsAdd
    {
        public string NewsCode { get; set; }
    }


    public class Input_GetNewsList : Pagination
    {
        public string UnionID { get; set; }

    }

    public class Input_GetNewsInfo
    {
        public string UnionID { get; set; }

        public string NewsCode { get; set; }

    }
    public class Input_GetAuditNewsList : Pagination
    {
        /// <summary>
        /// 订单号、店铺名称、店铺号查询
        /// </summary>
        public string SearchName { get; set; }

        /// <summary>
        /// 状态查询  1、未审核  2通过  3 拒绝  4终止  5排期内 6未开始  7已结束
        /// </summary>
        public int State { get; set; }

        public string BeginTime { get; set; }

        public string EndTime { get; set; }

        public string MallCode { get; set; }

    }

    public class Input_AuditState
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 状态查询  1、未审核  2通过  3 拒绝  4终止  5排期内 6未开始  7已结束
        /// </summary>
        public int State { get; set; }

        public string Reason { get; set; }

        public string MgrCode { get; set; }
    }


    public class Input_HasProg
    {
        public string UnionID { get; set; }
    }
}
