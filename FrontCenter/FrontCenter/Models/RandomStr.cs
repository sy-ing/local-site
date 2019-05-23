using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models
{
    public class RandomStr : Base
    {
        [Display(Name = "Parent")]
        [StringLength(255)]
        public string Str { get; set; }
    }
}
