using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class JsonModel
    {
        [Display(Name = "ID")]
        public int? ID { get; set; }

        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; }

        [Display(Name = "FileName")]
        public string FileName { get; set; }
    }
}
