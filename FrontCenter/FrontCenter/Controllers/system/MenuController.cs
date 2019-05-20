using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrontCenter.AppCode;
using FrontCenter.Models;
using FrontCenter.Models.Data;
using FrontCenter.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace FrontCenter.Controllers.system
{
    public class MenuController : Controller
    {
        private ArrayList WebChatMenu = new ArrayList() { "AuditManage", "ScheduleAudit", "MaterialAudit", "ShopAudit", "StoreManage", "SAMgr", "StatisticsManage", "HotMap" };
        [EnableCors("CorsSample")]
        public async Task<IActionResult> GetMenuInfo([FromServices] ContextString dbContext)

        {
            QianMuResult _Result = new QianMuResult();
            try
            {

                var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
                List<MenuViewModel> menus;



                if (uol == null || string.IsNullOrEmpty(uol.UserCode))
                {
                    _Result.Code = "401";
                    _Result.Msg = "请先登录";
                    _Result.Data = "";
                    return Json(_Result);
                }
                ArrayList notExixtsTextEn = new ArrayList();
                bool isOpenChat = false;
                var fun = await dbContext.FuncInfo.Where(i => i.Name == "品牌发布端").FirstOrDefaultAsync();
                if (fun != null)
                {
                    var mall = await dbContext.MallFunc.Where(i => i.MallCode == uol.MallCode && i.FuncCode == fun.Code).FirstOrDefaultAsync();
                    if (mall != null)
                    {
                        isOpenChat = true;
                    }
                }
                if (!isOpenChat)
                {
                    notExixtsTextEn = WebChatMenu;
                }
                if (uol.SystemModule == "Manage")
                {
                    menus = await GetUserMenu(dbContext, 0, notExixtsTextEn);
                }
                else
                {
                    menus = await GetUserMenu(dbContext, uol.ID, notExixtsTextEn);
                }



                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = menus;
                return Json(_Result);
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);

            }

        }

        [EnableCors("CorsSample")]
        [HttpPost]
        public async Task<IActionResult> GetMenuInfo(Input_MenuAccount model, [FromServices] ContextString dbContext)

        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_MenuAccount)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (uol == null || string.IsNullOrEmpty(uol.UserCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请先登录";
                _Result.Data = "";
                return Json(_Result);
            }
            try
            {
                List<MenuViewModel> menus;


                ArrayList notExixtsTextEn = new ArrayList();
                bool isOpenChat = false;
                var fun = await dbContext.FuncInfo.Where(i => i.Name == "品牌发布端").FirstOrDefaultAsync();
                if (fun != null)
                {
                    var mall = await dbContext.MallFunc.Where(i => i.MallCode == uol.MallCode && i.FuncCode == fun.Code).FirstOrDefaultAsync();
                    if (mall != null)
                    {
                        isOpenChat = true;
                    }
                }
                if (!isOpenChat)
                {
                    notExixtsTextEn = WebChatMenu;
                }
                if (uol.SystemModule != "Mall")
                {
                    menus = await GetUserMenu(dbContext, 0, notExixtsTextEn);
                }
                else
                {
                    menus = await GetUserMenu(dbContext, uol.ID, notExixtsTextEn);
                }


                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = menus;

                return Json(_Result);
            }
            catch (Exception e)
            {
                _Result.Code = "500";
                _Result.Msg = "Erro: " + e.ToString();
                _Result.Data = "";
                return Json(_Result);

            }

        }

        private async Task<List<MenuViewModel>> GetUserMenu(ContextString dbContext, int? ID, ArrayList TextEN)
        {
            string sql = "";
            if (ID == null || ID == 0)
            {
                sql = "select a.[ID],a.[AddTime] ,a.[Icon],a.[Order],a.[ParentID],a.[PermissionID],a.[TextCH] ,a.[TextEN],a.[Href] from MallSite_Menu a where a.[Enable] = 1";
            }
            else
            {
                sql = @"select a.[ID],a.[AddTime] ,a.[Icon],a.[Order],a.[ParentID],a.[PermissionID],a.[TextCH] ,a.[TextEN],a.[Href] from MallSite_Menu a 
 left join MallSite_Permission b on a.PermissionID = b.PermissionID
 left join MallSite_RolePermissions c on c.PermissionCode = b.Code
 left join MallSite_Roles d on c.RoleCode = d.Code
where d.RoleID in (select[RoleID] from MallSite_UserRoles where UserID = " + ID + ")  and a.[Enable] = 1";
            }
            if (TextEN != null && TextEN.Count > 0)
            {
                sql += "and a.[TextEN] not in (";
                foreach (var item in TextEN)
                {
                    sql += "'" + item + "',";
                }
                sql += "'')";
            }

            return await dbContext.MenuViewModel.FromSql(sql).AsNoTracking().OrderBy(o => o.ParentID).ThenBy(t => t.Order).ToListAsync();

        }
    }
}