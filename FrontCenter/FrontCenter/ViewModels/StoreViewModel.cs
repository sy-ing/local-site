using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class StoreViewModel
    {
    }

    public class Input_SUSearch : Pagination
    {
        public string SearchName { get; set; }

        public string ProvinceID { get; set; }

        public string CityID { get; set; }



        public string MallCode { get; set; }
    }

    public class Input_SUUnbinding
    {
        public string Code { get; set; }
    }
}
