using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrontCenter.AppCode;
using FrontCenter.Models;
using FrontCenter.Models.Data;
using FrontCenter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontCenter.Controllers.system
{
    public class UserController : Controller
    {
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUser model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //检测用户登录情况
            UserOnLine user = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (user == null || string.IsNullOrEmpty(user.UserCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }


            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (RegisterUser)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



            if (string.IsNullOrEmpty(model.RoleID))
            {
                _Result.Code = "510";
                _Result.Msg = "Erro:角色ID不可为空";
                _Result.Data = "";
                return Json(_Result);
            }



            if (!Method.IsNumeric(model.RoleID))
            {
                _Result.Code = "510";
                _Result.Msg = "Erro:包含非法的角色ID" + model.RoleID;
                _Result.Data = "";
                return Json(_Result);
            }

            var _RoleID = Convert.ToInt32(model.RoleID);

            //判断ID是否都为有效角色

            var q = await dbContext.Roles.Where(i => i.ID == _RoleID).AsNoTracking().CountAsync();
            if (q <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "Erro:没有ID为：" + _RoleID + "的角色";
                _Result.Data = "";
                return Json(_Result);
            }



            var _AvatarSrc = @"\images\DefaultAvatar.png";

            if (string.IsNullOrEmpty(model.AccountName)
                || string.IsNullOrEmpty(model.Password)
                || string.IsNullOrEmpty(model.Phone)
                || string.IsNullOrEmpty(model.Email)
                || model.Password != model.ConfirmPassword)
            {
                _Result.Code = "510";
                _Result.Msg = "输入信息不正确";
                _Result.Data = "";
                return Json(_Result);
            }

            if (model.Password.Length < 6)
            {
                _Result.Code = "510";
                _Result.Msg = "密码长度不因少于6位";
                _Result.Data = "";
                return Json(_Result);
            }

            //判断系统中是否存在用户
            if (Method.FindAllByName(dbContext, model.AccountName))
            {
                _Result.Code = "1";
                _Result.Msg = "用户已存在";
                _Result.Data = "";
                return Json(_Result);
            }

            var phonenum = await dbContext.Account.Where(i => i.Phone == model.Phone && i.Activity).CountAsync();

            if (phonenum > 0)
            {
                _Result.Code = "1";
                _Result.Msg = "手机号码已被使用";
                _Result.Data = "";
                return Json(_Result);
            }

            var emailnum = await dbContext.Account.Where(i => i.Email == model.Email && i.Activity).CountAsync();

            if (emailnum > 0)
            {
                _Result.Code = "1";
                _Result.Msg = "邮箱已被使用";
                _Result.Data = "";
                return Json(_Result);
            }

            //创建用户
            var _User = new Account()
            {
                AccountName = model.AccountName,
                PassWord = Method.StringToPBKDF2Hash(model.Password),
                NickName = model.NickName,
                Phone = model.Phone,
                Email = model.Email,
                AvatarSrc = _AvatarSrc,
                Activity = true,
                AddTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                Code = Guid.NewGuid().ToString(),
                MallCode = user.MallCode,
                SystemModule = "Mall"

            };

            int _AccountID = Method.CreateAccount(dbContext, _User).Result;

            var role = await dbContext.Roles.Where(i => i.ID == _RoleID).FirstOrDefaultAsync();

            //添加账户 角色关系
            if (_AccountID > 0)
            {

                try
                {
                    dbContext.UserRoles.Add(new UserRoles { UserCode = _User.Code, RoleCode = role.Code });
                    await dbContext.SaveChangesAsync();
                    _Result.Code = "200";
                    _Result.Msg = "创建用户成功";
                    _Result.Data = "";
                    var ip = Method.GetUserIp(this.HttpContext);
                    dbContext.SysLog.Add(new SysLog { AccountName = user.UserName, ModuleName = "用户模块", LogMsg = user.UserName + "创建了用户名为：" + model.AccountName + "的用户,访问信息：" + inputStr, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "创建", IP = ip, MallCode = user.MallCode, SystemModule = "Mall" });
                    dbContext.SaveChanges();

                }
                catch (Exception e)
                {
                    _Result.Code = "500";
                    _Result.Msg = "Erro:关联用户-角色关系失败 " + e.ToString();
                    _Result.Data = "";

                }

            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "创建用户失败";
                _Result.Data = "";
            }
            return Json(_Result);
        }


        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete(int? ID, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //检测用户登录情况
            UserOnLine user = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (user == null || string.IsNullOrEmpty(user.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }


            //Stream stream = HttpContext.Request.Body;
            //byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            //stream.Read(buffer, 0, buffer.Length);
            //string inputStr = Encoding.UTF8.GetString(buffer);

            //if (string.IsNullOrEmpty(inputStr))
            //{
            //    _Result.Code = "510";
            //    _Result.Msg = "请输入一个用户ID";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}
            //JsonModel model = new JsonModel();
            //model = (JsonModel)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            //ID = model.ID;

            if (ID == null)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个用户ID";
                _Result.Data = "";
                return Json(_Result);
            }

            Account _User = Method.GetUserByID(dbContext, (int)ID).Result;

            if (_User == null)
            {
                _Result.Code = "1";
                _Result.Msg = "系统中无此用户";
                _Result.Data = "";
                return Json(_Result);
            }

            if (_User.ID == 1)
            {
                _Result.Code = "510";
                _Result.Msg = "超管不可删除";
                _Result.Data = "";
                return Json(_Result);
            }

            //删除用户

            _User.Activity = false;

            dbContext.Account.Update(_User);
            await dbContext.SaveChangesAsync();


            _Result.Code = "200";
            _Result.Msg = "删除用户成功";
            _Result.Data = "";

            var ip = Method.GetUserIp(this.HttpContext);
            dbContext.SysLog.Add(new SysLog { AccountName = user.UserName, ModuleName = "用户模块", LogMsg = user.UserName + "删除了账户为：" + _User.AccountName + "的用户", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "删除", IP = ip, MallCode = user.MallCode, SystemModule = "Mall" });
            dbContext.SaveChanges();
            return Json(_Result);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Del(Input_Del model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //检测用户登录情况
            UserOnLine user = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (user == null || string.IsNullOrEmpty(user.UserCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_Del)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



            //if (string.IsNullOrEmpty(inputStr))
            //{
            //    _Result.Code = "510";
            //    _Result.Msg = "请输入一个用户ID";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}
            //JsonModel model = new JsonModel();
            //model = (JsonModel)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            //ID = model.ID;

            if (model.IDS == null || model.IDS.Count() <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个用户ID";
                _Result.Data = "";
                return Json(_Result);
            }
            var _username = string.Empty;
            foreach (var ID in model.IDS)
            {
                Account _User = Method.GetUserByID(dbContext, (int)ID).Result;

                if (_User == null)
                {
                    _Result.Code = "1";
                    _Result.Msg = "系统中无此用户";
                    _Result.Data = "";
                    return Json(_Result);
                }
                if (!_User.Activity)
                {
                    _Result.Code = "2";
                    _Result.Msg = "此用户已废弃";
                    _Result.Data = "";
                    return Json(_Result);
                }

                if (_User.ID == 1)
                {
                    _Result.Code = "510";
                    _Result.Msg = "超管不可删除";
                    _Result.Data = "";
                    return Json(_Result);
                }
                //删除用户

                _User.Activity = false;

                _username += _User.AccountName + ",";
                dbContext.Account.Update(_User);

                dbContext.UserRoles.RemoveRange(dbContext.UserRoles.Where(i => i.UserCode == _User.Code));

            }



            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "删除用户成功";
                _Result.Data = "";
            }
            else
            {
                _Result.Code = "200";
                _Result.Msg = "删除用户成功";
                _Result.Data = "";
            }

            var ip = Method.GetUserIp(this.HttpContext);
            dbContext.SysLog.Add(new SysLog { AccountName = user.UserName, ModuleName = "用户模块", LogMsg = user.UserName + "删除了账户为：" + _username.TrimEnd(',') + "的用户", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "删除", IP = ip, MallCode = user.MallCode, SystemModule = "Mall" });
            dbContext.SaveChanges();
            return Json(_Result);
        }


        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Input_UserEdit model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            //检测用户登录情况
            UserOnLine user = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (user == null || string.IsNullOrEmpty(user.UserCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }
            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_UserEdit)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            if (string.IsNullOrEmpty(model.AccountName)
                || string.IsNullOrEmpty(model.Phone)
                || string.IsNullOrEmpty(model.Email)
                || string.IsNullOrEmpty(model.NickName)
                || string.IsNullOrEmpty(model.RoleID))
            {
                _Result.Code = "510";
                _Result.Msg = "输入信息不正确";
                _Result.Data = "";
                return Json(_Result);
            }

            var _user = await dbContext.Account.Where(i => i.AccountName == model.AccountName && i.Activity).FirstOrDefaultAsync();

            if (_user == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的用户名";
                _Result.Data = "";
                return Json(_Result);
            }
            var phonenum = await dbContext.Account.Where(i => i.AccountName != model.AccountName && i.Phone == model.Phone && i.Activity).CountAsync();

            if (phonenum > 0)
            {
                _Result.Code = "510";
                _Result.Msg = "手机号已被使用";
                _Result.Data = "";
                return Json(_Result);
            }

            var emailnum = await dbContext.Account.Where(i => i.AccountName != model.AccountName && i.Email == model.Email && i.Activity).CountAsync();

            if (emailnum > 0)
            {
                _Result.Code = "510";
                _Result.Msg = "邮箱已被使用";
                _Result.Data = "";
                return Json(_Result);
            }

            if (string.IsNullOrEmpty(model.RoleID))
            {
                _Result.Code = "510";
                _Result.Msg = "Erro:角色ID不可为空";
                _Result.Data = "";
                return Json(_Result);
            }



            if (!Method.IsNumeric(model.RoleID))
            {
                _Result.Code = "510";
                _Result.Msg = "Erro:包含非法的角色ID" + model.RoleID;
                _Result.Data = "";
                return Json(_Result);
            }

            var _RoleID = Convert.ToInt32(model.RoleID);

            //判断ID是否都为有效角色

            var role = await dbContext.Roles.Where(i => i.ID == _RoleID).AsNoTracking().FirstOrDefaultAsync();
            if (role == null)
            {
                _Result.Code = "510";
                _Result.Msg = "Erro:没有ID为：" + _RoleID + "的角色";
                _Result.Data = "";
                return Json(_Result);
            }

            _user.NickName = model.NickName;
            _user.Phone = model.Phone;
            _user.Email = model.Email;

            dbContext.Account.Update(_user);

            dbContext.UserRoles.RemoveRange(dbContext.UserRoles.Where(i => i.UserCode == _user.Code));

            dbContext.UserRoles.Add(new UserRoles { RoleCode = role.Code, UserCode = _user.Code });


            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "修改成功";
                _Result.Data = "";
                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = user.UserName, ModuleName = "用户模块", LogMsg = user.UserName + "修改了账户为：" + _user.AccountName + "的用户信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip, MallCode = user.MallCode, SystemModule = "Mall" });
                dbContext.SaveChanges();
            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "修改失败";
                _Result.Data = "";
            }


            return Json(_Result);
        }



        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditInfo(Input_UserEditInfo model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //检测用户登录情况
            UserOnLine user = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (user == null || string.IsNullOrEmpty(user.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }


            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_UserEditInfo)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());



            if (string.IsNullOrEmpty(model.Phone)
                || string.IsNullOrEmpty(model.Email)
                || string.IsNullOrEmpty(model.NickName)
              )
            {
                _Result.Code = "510";
                _Result.Msg = "输入信息不正确";
                _Result.Data = "";
                return Json(_Result);
            }

            var _user = await dbContext.Account.Where(i => i.Code == user.UserCode).FirstOrDefaultAsync();

            if (_user == null)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的用户";
                _Result.Data = "";
                return Json(_Result);
            }

            var phonenum = await dbContext.Account.Where(i => i.Code != user.UserCode && i.Phone == model.Phone && i.Activity).CountAsync();

            if (phonenum > 0)
            {
                _Result.Code = "510";
                _Result.Msg = "手机号已被使用";
                _Result.Data = "";
                return Json(_Result);
            }

            var emailnum = await dbContext.Account.Where(i => i.Code != user.UserCode && i.Email == model.Email && i.Activity).CountAsync();

            if (emailnum > 0)
            {
                _Result.Code = "510";
                _Result.Msg = "邮箱已被使用";
                _Result.Data = "";
                return Json(_Result);
            }

            _user.NickName = model.NickName;
            _user.Phone = model.Phone;
            _user.Email = model.Email;

            dbContext.Account.Update(_user);

            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "修改成功";
                _Result.Data = "";
                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = user.UserName, ModuleName = "用户模块", LogMsg = user.UserName + "修改了账户为：" + _user.AccountName + "的用户信息", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip, MallCode = user.MallCode, SystemModule = "Mall" });
                dbContext.SaveChanges();
            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "修改失败";
                _Result.Data = "";
            }


            return Json(_Result);
        }


        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangePwd(Input_ChangePwd model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();
            //检测用户登录情况
            UserOnLine userOnLine = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (userOnLine == null || string.IsNullOrEmpty(userOnLine.UserCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_ChangePwd)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


            //判断输入条件


            if (model.Password.Length < 6)
            {
                _Result.Code = "510";
                _Result.Msg = "用户密码应为6-20位字符";
                _Result.Data = "";
                return Json(_Result);
            }
            if (model.Password != model.ConfirmPassword)
            {
                _Result.Code = "510";
                _Result.Msg = "二次输入新密码不一致";
                _Result.Data = "";
                return Json(_Result);
            }

            //加密用户密码
            string _PWD = Method.StringToPBKDF2Hash(model.OldPassword);


            var user = await dbContext.Account.Where(i => i.AccountName == model.AccountName && i.PassWord == _PWD).FirstOrDefaultAsync();

            if (user != null)
            {
                user.PassWord = Method.StringToPBKDF2Hash(model.Password);
                dbContext.Account.Update(user);
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    _Result.Code = "200";
                    _Result.Msg = "修改密码成功";
                    _Result.Data = "";

                    var ip = Method.GetUserIp(this.HttpContext);
                    dbContext.SysLog.Add(new SysLog { AccountName = userOnLine.UserName, ModuleName = "用户模块", LogMsg = userOnLine.UserName + "修改了用户：" + model.AccountName + "的登录密码", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip, MallCode = userOnLine.UserCode, SystemModule = "Mall" });
                    dbContext.SaveChanges();
                }
                else
                {
                    _Result.Code = "2";
                    _Result.Msg = "修改密码失败";
                    _Result.Data = "";
                }
            }
            else
            {
                _Result.Code = "510";
                _Result.Msg = "原有密码输入错误";
                _Result.Data = "";

            }
            return Json(_Result);

        }


        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassWord(Input_ChangePassWord model, [FromServices] ContextString dbContext)
        {

            QianMuResult _Result = new QianMuResult();



            //检测用户登录情况
            UserOnLine userOnLine = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (userOnLine == null || string.IsNullOrEmpty(userOnLine.UserCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_ChangePassWord)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            //判断输入条件
            if (string.IsNullOrEmpty(model.AccountName))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个用户名";
                _Result.Data = "";
                return Json(_Result);
            }

            if (model.Password.Length < 6)
            {
                _Result.Code = "510";
                _Result.Msg = "用户密码应为6-20位字符";
                _Result.Data = "";
                return Json(_Result);
            }
            if (model.Password != model.ConfirmPassword)
            {
                _Result.Code = "510";
                _Result.Msg = "二次输入新密码不一致";
                _Result.Data = "";
                return Json(_Result);
            }




            var user = await dbContext.Account.Where(i => i.Activity && i.AccountName == model.AccountName).FirstOrDefaultAsync();

            if (user != null)
            {
                user.PassWord = Method.StringToPBKDF2Hash(model.Password);
                dbContext.Account.Update(user);
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    _Result.Code = "200";
                    _Result.Msg = "修改密码成功";
                    _Result.Data = "";

                    var ip = Method.GetUserIp(this.HttpContext);
                    dbContext.SysLog.Add(new SysLog { AccountName = userOnLine.UserName, ModuleName = "用户模块", LogMsg = userOnLine.UserName + "修改了用户：" + model.AccountName + "的登录密码", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip, MallCode = userOnLine.MallCode, SystemModule = "Mall" });
                    dbContext.SaveChanges();
                }
                else
                {
                    _Result.Code = "2";
                    _Result.Msg = "修改密码失败";
                    _Result.Data = "";
                }
            }
            else
            {
                _Result.Code = "510";
                _Result.Msg = "无效的用户名";
                _Result.Data = "";

            }
            return Json(_Result);
        }


        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetUserList(Input_QueryUser model, [FromServices] ContextString dbContext)
        {

            QianMuResult _Result = new QianMuResult();

            //检测用户登录情况
            UserOnLine userOnLine = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (userOnLine == null || string.IsNullOrEmpty(userOnLine.UserCode) || string.IsNullOrEmpty(userOnLine.MallCode))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }


            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_QueryUser)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            var userlist = await dbContext.Output_UserList.FromSql(@"select a.ID,a.AccountName,a.NickName,c.Name as RoleName, CONVERT(varchar(100), a.AddTime, 20) as AddTime,
                                                                       case ISNULL(d.Code, '') when '' then 0 else 1 end as [Status]
                                                                       from Account a left
                                                                       join UserRoles b  on a.ID = b.UserID
                                                                       left join Roles c on b.RoleID = c.ID
                                                                       left join UserWarningEmail d  on a.ID = d.UserID where a.MallCode=@mallcode and a.Activity = 1", new SqlParameter("@mallcode", userOnLine.MallCode)).ToListAsync();

            if (model.Paging == 1)
            {
                userlist = userlist.Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            }

            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = userlist;
            return Json(_Result);
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>

        public async Task<IActionResult> GetUserInfo(int? ID, [FromServices] ContextString dbContext)
        {

            QianMuResult _Result = new QianMuResult();
            if (ID == null)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个ID";
                _Result.Data = "";
                return Json(_Result);
            }

            var count = await dbContext.Account.Where(i => i.Activity == true && i.ID == ID).CountAsync();
            if (count <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的用户ID";
                _Result.Data = "";
                return Json(_Result);
            }

            var user = await dbContext.Account.Where(i => i.Activity == true && i.ID == ID).Join(dbContext.UserRoles, ac => ac.Code, ur => ur.UserCode, (ac, ur) => new
            {
                ac.ID,
                ac.AccountName,
                ac.NickName,
                ur.RoleCode,
                ac.AddTime,
                ac.Phone,
                ac.Email,
                ac.Code
            }).Join(dbContext.Roles, ac => ac.RoleCode, ro => ro.Code, (ac, ro) => new
            {

                ac.ID,
                ac.AccountName,
                ac.NickName,
                ac.RoleCode,
                RoleName = ro.Name,
                ac.Phone,
                ac.Email,
                ac.AddTime,
                ac.Code
            }).FirstOrDefaultAsync();



            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = user;

            return Json(_Result);
        }

        [HttpPost]
        public async Task<IActionResult> SetWarningUser(Input_SetWE model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();

            //检测用户登录情况
            UserOnLine userOnLine = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (userOnLine == null || string.IsNullOrEmpty(userOnLine.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_SetWE)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());
            if (model.ID == null)
            {
                _Result.Code = "510";
                _Result.Msg = "请输入一个ID";
                _Result.Data = "";
                return Json(_Result);
            }

            var count = await dbContext.Account.Where(i => i.Activity == true && i.ID == model.ID).CountAsync();
            if (count <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "无效的用户ID";
                _Result.Data = "";
                return Json(_Result);
            }

            var user = await dbContext.Account.Where(i => i.Activity == true && i.ID == model.ID).FirstOrDefaultAsync();
            if (model.Status != 0)
            {
                dbContext.UserWarningEmail.RemoveRange(dbContext.UserWarningEmail.Where(i => i.UserCode == user.Code));
                dbContext.UserWarningEmail.Add(new UserWarningEmail { AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), UserCode = user.Code,UpdateTime = DateTime.Now });
            }
            else
            {

                dbContext.UserWarningEmail.RemoveRange(dbContext.UserWarningEmail.Where(i => i.UserCode == user.Code));
            }
            if (await dbContext.SaveChangesAsync() > 0)
            {
                _Result.Code = "200";
                _Result.Msg = "设置成功";
                _Result.Data = "";

                var ip = Method.GetUserIp(this.HttpContext);
                dbContext.SysLog.Add(new SysLog { AccountName = userOnLine.UserName, ModuleName = "用户模块", LogMsg = userOnLine.UserName + "设置用户：" + user.AccountName + "为报警用户", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "修改", IP = ip, MallCode = userOnLine.MallCode, SystemModule = "Mall" });
                dbContext.SaveChanges();
            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "设置失败";
                _Result.Data = "";
            }





            return Json(_Result);
        }

        ///// <summary>
        ///// 获取用户应用
        ///// </summary>
        ///// <param name="ID"></param>
        ///// <param name="dbContext"></param>
        ///// <returns></returns>

        //public async Task<IActionResult> GetUserApp(int? ID, [FromServices] ContextString dbContext)
        //{

        //    QianMuResult _Result = new QianMuResult();
        //    if (ID == null)
        //    {
        //        _Result.Code = "510";
        //        _Result.Msg = "请输入一个ID";
        //        _Result.Data = "";
        //        return Json(_Result);
        //    }

        //    var count = await dbContext.Account.Where(i => i.Activity == true && i.ID == ID).CountAsync();
        //    if (count <= 0)
        //    {
        //        _Result.Code = "510";
        //        _Result.Msg = "无效的用户ID";
        //        _Result.Data = "";
        //        return Json(_Result);
        //    }

        //    var apps = await dbContext.UserApp.Where(i => i.UserID == ID).Join(dbContext.Application, ua => ua.AppCode, ap => ap.AppID, (ua, ap) => new
        //    {

        //        ap.AddTime,
        //        ap.AppClass,
        //        ap.AppID,
        //        ap.AppSecClass,
        //        ap.Description,
        //        ap.Developer,
        //        ap.DevSupport,
        //        ap.ID,
        //        ap.IconFileID,
        //        ap.ID,
        //        ap.IsDel,
        //        ap.Name,
        //        ap.NameEn,
        //        ap.PreviewFiles,
        //        ap.ScreenInfoID,
        //        ap.Version
        //    }).Join(dbContext.AssetFiles, ap => ap.IconFileID, af => af.ID, (ap, af) => new
        //    {
        //        Name = ap.Name,
        //        ID = ap.ID,
        //        AppClass = ap.AppClass,
        //        AppSecClass = ap.AppSecClass,
        //        Description = ap.Description,
        //        IconFilePath = Method.ServerAddr + af.FilePath.ToString(),
        //        ScreenInfoID = ap.ScreenInfoID,
        //        Version = ap.Version,
        //        AppID = ap.AppID

        //    }).Join(dbContext.AppClass, ap => ap.AppClass, ac => ac.ID, (ap, ac) => new {

        //        ap.Name,
        //        ap.ID,
        //        AppClass = ac.ClassName,
        //        ap.AppSecClass,
        //        ap.Description,
        //        ap.IconFilePath,
        //        ap.ScreenInfoID,
        //        ap.Version,
        //        ap.AppID

        //    }).Join(dbContext.AppClass, ap => ap.AppSecClass, ac => ac.ID, (ap, ac) => new {

        //        ap.Name,
        //        ap.ID,
        //        ap.AppClass,
        //        AppSecClass = ac.ClassName,
        //        ap.Description,
        //        ap.IconFilePath,
        //        ap.ScreenInfoID,
        //        ap.Version,
        //        ap.AppID

        //    }).AsNoTracking().ToListAsync(); ;


        //    ArrayList arrayList = new ArrayList();
        //    foreach (var app in apps)
        //    {
        //        string href = string.Empty;
        //        var isLogOut = 0;
        //        var sitecount = await dbContext.AppSite.Where(i => i.AppCode == app.AppID).AsNoTracking().CountAsync();
        //        if (sitecount > 0)
        //        {
        //            var appsite = await dbContext.AppSite.Where(i => i.AppCode == app.AppID).AsNoTracking().FirstOrDefaultAsync();
        //            href = appsite.Href;

        //            //检测用户登录情况
        //            string username = Method.GetLoginUserName(dbContext, this.HttpContext);
        //            if (!string.IsNullOrEmpty(username))
        //            {
        //                isLogOut = 1;
        //                var _User = dbContext.Accounts.Where(i => i.AccountName == username).FirstOrDefault();
        //                string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        //                string salt = "QianMu";
        //                string key = salt + dtime;

        //                string _EncryptionKey = Method.StringToPBKDF2Hash(key);

        //                _EncryptionKey = System.Net.WebUtility.UrlEncode(_EncryptionKey);
        //                href = href + "?account=" + username + "&CheckTime=" + dtime + "&Token=" + _EncryptionKey + "&Pwd=" + _User.PassWord;
        //            }
        //            else
        //            {
        //                href = Method.ServerAddr;
        //            }

        //        }
        //        arrayList.Add(new
        //        {
        //            app.Name,
        //            app.ID,
        //            app.AppClass,
        //            app.AppSecClass,
        //            app.Description,
        //            app.IconFilePath,
        //            app.ScreenInfoID,
        //            app.Version,
        //            Href = href,
        //            IsLotOut = isLogOut
        //        });
        //    }


        //    _Result.Code = "200";
        //    _Result.Msg = "获取成功";
        //    _Result.Data = arrayList;

        //    return Json(_Result);
        //}


        ///// <summary>
        ///// 获取用户应用
        ///// </summary>
        ///// <param name="ID"></param>
        ///// <param name="dbContext"></param>
        ///// <returns></returns>

        public async Task<IActionResult> GetUserAppNew([FromServices] ContextString dbContext)
        {

            QianMuResult _Result = new QianMuResult();

            //var count = await dbContext.Account.Where(i => i.Activity == true && i.Code == UserCode).CountAsync();
            //if (string.IsNullOrEmpty(UserCode) || count <= 0)
            //{
            //    _Result.Code = "510";
            //    _Result.Msg = "无效的用户编码";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}
            //检测用户登录情况
            var uol = Method.GetLoginUserName(dbContext, this.HttpContext);
            if (string.IsNullOrEmpty(uol.UserName))
            {
                _Result.Code = "401";
                _Result.Msg = "请登陆后再进行操作";
                _Result.Data = "";
                return Json(_Result);
            }
            var apps = await dbContext.UserAppNew.Where(i => i.UserCode == uol.UserCode).Join(dbContext.ApplicationNew.Where(i => i.MallCode == uol.MallCode && !i.IsDel), ua => ua.AppCode, ap => ap.Code, (ua, ap) => new
            {

                ap.AddTime,
                ap.AppClass,
                ap.AppID,
                ap.AppSecClass,
                ap.Description,
                ap.Developer,
                ap.DevSupport,
                ap.FileCode,
                ap.IconFileCode,
                ap.ID,
                ap.IsDel,
                ap.Name,
                ap.NameEn,
                ap.PreviewFiles,
                ap.ScreenInfoCode,
                ap.Version,
                ap.Code,
                ap.PlatformType
            }).Join(dbContext.AssetFiles, ap => ap.IconFileCode, af => af.Code, (ap, af) => new
            {
                Name = ap.Name,
                ID = ap.ID,
                AppClass = ap.AppClass,
                AppSecClass = ap.AppSecClass,
                Description = ap.Description,
                IconFilePath = Method.OSSServer + af.FilePath.ToString(),
                ap.ScreenInfoCode,
                Version = ap.Version,
                AppID = ap.AppID,
                ap.Code,
                ap.PlatformType

            }).Join(dbContext.AppClassNew, ap => ap.AppClass, ac => ac.Code, (ap, ac) => new
            {

                ap.Name,
                ap.ID,
                AppClass = ac.ClassName,
                ap.AppSecClass,
                ap.Description,
                ap.IconFilePath,
                ap.ScreenInfoCode,
                ap.Version,
                ap.AppID,
                ap.Code,
                ap.PlatformType

            }).Join(dbContext.AppClassNew, ap => ap.AppSecClass, ac => ac.Code, (ap, ac) => new
            {

                ap.Name,
                ap.ID,
                ap.AppClass,
                AppSecClass = ac.ClassName,
                ap.Description,
                ap.IconFilePath,
                ap.ScreenInfoCode,
                ap.Version,
                ap.AppID,
                ap.Code,
                ap.PlatformType

            }).Join(dbContext.ScreenInfo.Where(i => i.MallCode == uol.MallCode), ap => ap.ScreenInfoCode, s => s.Code, (ap, s) => new
            {
                ap.Name,
                ap.ID,
                ap.AppClass,
                ap.AppSecClass,
                ap.Description,
                ap.IconFilePath,
                ap.ScreenInfoCode,
                ap.Version,
                ap.AppID,
                ap.Code,
                s.SName,
                ap.PlatformType

            }).AsNoTracking().ToListAsync(); ;


            ArrayList arrayList = new ArrayList();
            foreach (var app in apps)
            {
                string href = string.Empty;
                var isLogOut = 0;
                var sitecount = await dbContext.AppSite.Where(i => i.AppCode == app.Code).AsNoTracking().CountAsync();
                if (sitecount > 0)
                {
                    var appsite = await dbContext.AppSite.Where(i => i.AppCode == app.Code).AsNoTracking().FirstOrDefaultAsync();

                    if (!string.IsNullOrEmpty(appsite.Href))
                    {
                        href = appsite.Href;

                        //检测用户登录情况
                        //string username = Method.GetLoginUserName(dbContext, this.HttpContext);
                        //if (!string.IsNullOrEmpty(username))
                        //{
                        isLogOut = 1;
                        var _User = dbContext.Account.Where(i => i.Code == uol.UserCode).FirstOrDefault();
                        string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        string salt = "QianMu";
                        string key = salt + dtime;

                        string _EncryptionKey = Method.StringToPBKDF2Hash(key);

                        _EncryptionKey = System.Net.WebUtility.UrlEncode(_EncryptionKey);
                        href = href + "?account=" + uol.UserName + "&CheckTime=" + dtime + "&Token=" + _EncryptionKey + "&Pwd=" + _User.PassWord;
                        //}
                        //else
                        //{
                        //    href = Method.ServerAddr;
                        //}
                    }

                }
                arrayList.Add(new
                {
                    app.Name,
                    app.ID,
                    app.AppClass,
                    app.AppSecClass,
                    app.Description,
                    app.IconFilePath,
                    app.Code,
                    app.SName,
                    app.Version,
                    Href = href,
                    app.PlatformType,
                    IsLotOut = isLogOut
                });
            }


            _Result.Code = "200";
            _Result.Msg = "获取成功";
            _Result.Data = arrayList;

            return Json(_Result);
        }

    }
}