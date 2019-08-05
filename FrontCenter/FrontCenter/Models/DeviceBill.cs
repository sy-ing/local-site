using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class DeviceBill : Base
    {
        [StringLength(50)]
        public string MallCode { get; set; }

        [StringLength(50)]
        public string DeviceCode { get; set; }

        [StringLength(50)]
        public string Bill { get; set; }

        [StringLength(50)]
        public string ProperBill { get; set; }

        public DateTime EffecDate { get; set; }
    }
}
