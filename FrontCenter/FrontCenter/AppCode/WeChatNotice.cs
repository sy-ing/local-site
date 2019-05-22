using FrontCenter.Models;
using FrontCenter.Models.Data;
using Senparc.Weixin.MP.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    public class WeChatNotice
    {
        public static string WeChatTemplateID = Method.Configuration["ConnectionStrings:WeChatTemplateID"].ToString();
        public static string WeChatPage = Method.Configuration["ConnectionStrings:WeChatPage"].ToString();
        public static void SendWeChatNotice(string openID, string formID, string data)
        {
            string appId = "wxc7445db4fff61eb9";
            string appSecret = "4dcd295b0ec93878d52aa1d54e99aa50";
            //根据appId判断获取    
            if (!AccessTokenContainer.CheckRegistered(appId))    //检查是否已经注册    
            {
                AccessTokenContainer.Register(appId, appSecret);    //如果没有注册则进行注册    
            }
            string access_token = AccessTokenContainer.GetAccessTokenResult(appId).access_token; //AccessToken  
            string tagUrl = string.Format("https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?access_token={0}", access_token);
            StringBuilder paramsStr = new StringBuilder();
            paramsStr.Append("{");
            paramsStr.Append("\"touser\": \"" + openID + "\",");
            paramsStr.Append("\"template_id\": \"" + WeChatTemplateID + "\",");
            paramsStr.Append("\"page\": \"" + WeChatPage + "\",");
            paramsStr.Append("\"form_id\":\"" + formID + "\",");
            paramsStr.Append("\"data\":" + data);
            paramsStr.Append("}");
            string param = paramsStr.ToString();
            string result = Method.PostMoths(tagUrl, param);
        }

        public static void SendNoticeBySchedule(ScheduleOrder schedule, ContextString dbContext)
        {
            string remarks = schedule.Status == 2 ? "您的排期已申请通过，快去添加节目吧！" : "您的排期未能通过申请，请查看拒绝理由。";
            var account = dbContext.ShopAccount.Where(i => i.ShopCode == schedule.ShopCode).FirstOrDefault();
            if (account == null)
            {
                return;
            }
            var openID = account.UnionID;
            var data = "{\"keyword1\": {\"value\": \"" + schedule.PlacingNum + "\"},\"keyword2\": {\"value\": \"" + schedule.AddTime.ToString("yyyy年MM月dd日 HH:mm") + "\"},\"keyword3\": {\"value\": \"" + (schedule.Status == 2 ? "已通过" : "已拒绝") + "\"" + (schedule.Status == 2 ? "" : ",\"color\": \"#f00\"") + "} ,\"keyword4\": {\"value\": \"" + remarks + "\"}}";
            SendWeChatNotice(openID, schedule.FormID, data);
        }

        public static void SendNoticeByProgram(ProgramOrder program, ContextString dbContext)
        {
            string remarks = program.Status == 2 ? "您的图片素材已通过审核，快去发布吧！" : "您的图片素材未通过审核，请查验后再次上传。";
            var account = dbContext.ShopAccount.Where(i => i.ShopCode == program.ShopCode).FirstOrDefault();
            if (account == null)
            {
                return;
            }
            var openID = account.UnionID;
            var data = "{\"keyword1\": {\"value\": \"" + program.PlacingNum + "\"},\"keyword2\": {\"value\": \"" + program.AddTime.ToString("yyyy年MM月dd日 HH:mm") + "\"},\"keyword3\": {\"value\": \"" + (program.Status == 2 ? "已通过" : "已拒绝") + "\"" + (program.Status == 2 ? "" : ",\"color\": \"#f00\"") + "} ,\"keyword4\": {\"value\": \"" + remarks + "\"}}";
            SendWeChatNotice(openID, program.FormID, data);
        }
    }
}
