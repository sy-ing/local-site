using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FrontCenter.AppCode;
using FrontCenter.Models;
using FrontCenter.Models.Data;
using FrontCenter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontCenter.Controllers.system
{
    public class RoleController : Controller
    {
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(Input_AddRole model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_AddRole)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            //检测用户登录情况
            UserOnLine userOnLine = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (userOnLine == null || string.IsNullOrEmpty(userOnLine.UserCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                _Result.Code = "510";
                _Result.Msg = "角色名称不可为空";
                _Result.Data = "";
                return Json(_Result);
            }

            Regex regExp = new Regex("[ \\[ \\] \\^ \\-_*×――(^)$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]");
            if (regExp.IsMatch(model.Name))
            {
                _Result.Code = "510";
                _Result.Msg = "角色名称不能有特殊字符";
                _Result.Data = "";
                return Json(_Result);
            }

            var count = await dbContext.Roles.Where(i => i.Name == model.Name.Trim()).AsNoTracking().CountAsync();
            if (count > 0)
            {
                _Result.Code = "510";
                _Result.Msg = "角色已存在，不可重复添加";
                _Result.Data = "";
                return Json(_Result);
            }

            if (regExp.IsMatch(model.Intro))
            {
                _Result.Code = "510";
                _Result.Msg = "角色描述不能有特殊字符";
                _Result.Data = "";
                return Json(_Result);
            }

            //创建角色
            Roles role = new Roles();
            role.AddTime = DateTime.Now;
            role.Code = Guid.NewGuid().ToString();
            role.Intro = model.Intro;
            role.Name = model.Name;
            role.Description = model.Name;

            dbContext.Roles.Add(role);

            List<RolePermissions> list = new List<RolePermissions>();

            //如果有设备控制权限 则加上设备查看权限
            var _DeviceControl = await dbContext.Permission.Where(i => i.Description == "DeviceControl").FirstOrDefaultAsync();
            var _DevListMgr = await dbContext.Permission.Where(i => i.Description == "DevListMgr").FirstOrDefaultAsync();
            if (model.PermissionCode.Contains(_DeviceControl.Code))
            {
                if (!model.PermissionCode.Contains(_DevListMgr.Code))
                {
                    model.PermissionCode.Add(_DevListMgr.Code);
                }
            }

            foreach (var p in model.PermissionCode)
            {
                var percount = await dbContext.Permission.Where(i => i.Code == p).CountAsync();
                if (percount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的权限编码：" + p;
                    _Result.Data = "";
                    return Json(_Result);
                }


                var per = await dbContext.Permission.Where(i => i.Code == p).FirstOrDefaultAsync();

                var parents = await dbContext.Permission.Where(i => i.Code == per.ParentCode).FirstOrDefaultAsync();




                list.Add(new RolePermissions { AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), PermissionCode = p, RoleCode = role.Code });
                if (list.Where(i => i.PermissionCode == parents.Code).Count() <= 0)
                {
                    list.Add(new RolePermissions { AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), PermissionCode = parents.Code, RoleCode = role.Code });

                }

                //  dbContext.RolePermissions.Add(new RolePermissions { AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), PermissionCode = p, RoleCode = role.Code });
            }

            list = list.Distinct().ToList();
            dbContext.RolePermissions.AddRange(list);


            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "添加成功";
                _Result.Data = "";



                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = userOnLine.UserName, ModuleName = "用户模块", LogMsg = userOnLine.UserName + "添加角色：" + model.Name, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "创建", IP = ip, MallCode = userOnLine.MallCode, SystemModule = "Mall" });
                dbContext.SaveChanges();

            }
            else
            {
                _Result.Code = "1";
                _Result.Msg = "添加失败";
                _Result.Data = "";
            }

            return Json(_Result);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Del(Input_DelRole model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_DelRole)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            //检测用户登录情况
            UserOnLine userOnLine = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (userOnLine == null || string.IsNullOrEmpty(userOnLine.UserCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }


            var _names = String.Empty;

            foreach (var c in model.Code)
            {
                var rcount = await dbContext.Roles.Where(i => i.Code == c).CountAsync();
                if (rcount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的角色编码：" + c;
                    _Result.Data = "";
                    return Json(_Result);
                }

                var role = await dbContext.Roles.Where(i => i.Code == c).FirstOrDefaultAsync();
                var urcount = await dbContext.UserRoles.Where(i => i.RoleCode == role.Code).CountAsync();
                if (urcount > 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "角色：" + role.Name + "正在被使用不可删除";
                    _Result.Data = "";
                    return Json(_Result);
                }
                if (role.Name == "超级管理员")
                {
                    _Result.Code = "510";
                    _Result.Msg = "超级管理员不可删除";
                    _Result.Data = "";
                    return Json(_Result);
                }
                dbContext.Roles.Remove(role);
                dbContext.RolePermissions.RemoveRange(dbContext.RolePermissions.Where(i => i.RoleCode == role.Code));
                _names += role.Name + ",";


            }

            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "删除成功";
                _Result.Data = "";



                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = userOnLine.UserName, ModuleName = "用户模块", LogMsg = userOnLine.UserName + "删除角色：" + _names.TrimEnd(','), AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "删除", IP = ip, MallCode = userOnLine.MallCode, SystemModule = "Mall" });
                dbContext.SaveChanges();

            }
            else
            {
                _Result.Code = "1";
                _Result.Msg = "删除失败";
                _Result.Data = "";
            }

            return Json(_Result);
        }


        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Input_EditRole model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_EditRole)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            //检测用户登录情况
            UserOnLine userOnLine = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (userOnLine == null || string.IsNullOrEmpty(userOnLine.UserCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                _Result.Code = "510";
                _Result.Msg = "角色名称不可为空";
                _Result.Data = "";
                return Json(_Result);
            }

            Regex regExp = new Regex("[ \\[ \\] \\^ \\-_*×――(^)$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]");
            if (regExp.IsMatch(model.Name))
            {
                _Result.Code = "510";
                _Result.Msg = "角色名称不能有特殊字符";
                _Result.Data = "";
                return Json(_Result);
            }

            var count = await dbContext.Roles.Where(i => i.Code == model.Code).AsNoTracking().CountAsync();
            if (count <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "角色不存在";
                _Result.Data = "";
                return Json(_Result);
            }
            //更新角色

            if (regExp.IsMatch(model.Intro))
            {
                _Result.Code = "510";
                _Result.Msg = "角色描述不能有特殊字符";
                _Result.Data = "";
                return Json(_Result);
            }

            var role = await dbContext.Roles.Where(i => i.Code == model.Code).FirstOrDefaultAsync();

            if (role.Name == "超级管理员")
            {
                _Result.Code = "510";
                _Result.Msg = "超级管理员不可修改";
                _Result.Data = "";
                return Json(_Result);
            }


            role.Description = model.Name;
            role.Name = model.Name;
            role.Intro = model.Intro;

            dbContext.Roles.UpdateRange(role);

            dbContext.RolePermissions.RemoveRange(dbContext.RolePermissions.Where(i => i.RoleCode == model.Code));


            //如果有设备控制权限 则加上设备查看权限
            var _DeviceControl = await dbContext.Permission.Where(i => i.Description == "DeviceControl").FirstOrDefaultAsync();
            var _DevListMgr = await dbContext.Permission.Where(i => i.Description == "DevListMgr").FirstOrDefaultAsync();
            if (model.PermissionCode.Contains(_DeviceControl.Code))
            {
                if (!model.PermissionCode.Contains(_DevListMgr.Code))
                {
                    model.PermissionCode.Add(_DevListMgr.Code);
                }
            }

            List<RolePermissions> list = new List<RolePermissions>();
            foreach (var p in model.PermissionCode)
            {
                var percount = await dbContext.Permission.Where(i => i.Code == p).CountAsync();
                if (percount <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "无效的权限编码：" + p;
                    _Result.Data = "";
                    return Json(_Result);
                }

                var per = await dbContext.Permission.Where(i => i.Code == p).FirstOrDefaultAsync();

                var parents = await dbContext.Permission.Where(i => i.Code == per.ParentCode).FirstOrDefaultAsync();


                list.Add(new RolePermissions { AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), PermissionCode = p, RoleCode = role.Code });
                if (list.Where(i => i.PermissionCode == parents.Code).Count() <= 0)
                {
                    list.Add(new RolePermissions { AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), PermissionCode = parents.Code, RoleCode = role.Code });

                }

                // dbContext.RolePermissions.Add(new RolePermissions { AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), PermissionCode = p, RoleCode = role.Code });
            }

            list = list.Distinct().ToList();
            dbContext.RolePermissions.AddRange(list);

            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "修改成功";
                _Result.Data = "";



                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = userOnLine.UserName, ModuleName = "用户模块", LogMsg = userOnLine.UserName + "修改角色：" + model.Name, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip, MallCode = userOnLine.MallCode, SystemModule = "Mall" });
                dbContext.SaveChanges();

            }
            else
            {
                _Result.Code = "1";
                _Result.Msg = "修改失败";
                _Result.Data = "";
            }

            return Json(_Result);
        }


        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetInfo(string code, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            try
            {
                var count = await dbContext.Roles.Where(i => i.Code == code).AsNoTracking().CountAsync();
                if (count <= 0)
                {
                    _Result.Code = "510";
                    _Result.Msg = "角色不存在";
                    _Result.Data = "";
                    return Json(_Result);
                }
                //获取角色

                var role = await dbContext.Roles.Where(i => i.Code == code).FirstOrDefaultAsync();

                //获取权限

                var roleper = await dbContext.RolePermissions.Where(i => i.RoleCode == code).Join(dbContext.Permission.Where(i => !string.IsNullOrEmpty(i.ParentCode)), rp => rp.PermissionCode, pe => pe.Code, (rp, pe) => new {
                    pe.Code,
                    pe.Name
                }).ToListAsync();

    

                _Result.Code = "200";
                _Result.Msg = "获取成功";
                _Result.Data = new { RoleID = role.ID, role.Name, role.Intro, role.Description, role.Code, role.AddTime, Permission = roleper };

            }
            catch (Exception e)
            {

                _Result.Code = "500";
                _Result.Msg = "获取失败" + e.ToString();
                _Result.Data = "";
            }

            return Json(_Result);
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetList([FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            var q = await dbContext.Roles.AsNoTracking().ToArrayAsync();

            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = q;



            return Json(_Result);
        }

    }
}