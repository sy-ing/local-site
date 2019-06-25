using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrontCenter.AppCode;
using FrontCenter.Models;
using FrontCenter.Models.Data;
using Microsoft.AspNetCore.Mvc;

namespace FrontCenter.Controllers
{
    public class MallController : Controller
    {
        [HttpPost]
        public IActionResult GetMallByRegKey([FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            _Result.Code = "200";
            _Result.Msg = "";
            _Result.Data =Method.CusID;
            return Json(_Result);
        }
    }
}