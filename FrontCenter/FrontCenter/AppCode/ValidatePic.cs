using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    public class ValidatePic
    {
        public static int SuspiciousVal = string.IsNullOrEmpty(Method.Configuration["ConnectionStrings:SuspiciousVal"]) ? 0 : Convert.ToInt32(Method.Configuration["ConnectionStrings:SuspiciousVal"]);
        public static int PicAutoPassVal = string.IsNullOrEmpty(Method.Configuration["ConnectionStrings:PicAutoPassVal"]) ? 0 : Convert.ToInt32(Method.Configuration["ConnectionStrings:PicAutoPassVal"]);
        private static string ValidatePicUrl = Method.Configuration["ConnectionStrings:ValidatePicUrl"];
        private static string Host = Method.Configuration["ConnectionStrings:Host"];
        private static string Path = Method.Configuration["ConnectionStrings:Path"];



        public static bool GreenImg(string url)
        {
            var result = false;

            String _url = Host + Path;
            //鉴黄

            String bodys = "base64=base64&imgUrl=" + Url(url) + "&type=1";
            int _sex = GetPicValue(_url, bodys);

            if (_sex == -1)
            {
                return result;
            }
            //鉴暴
            bodys = "base64=base64&imgUrl=" + Url(url) + "&type=2";
            int _terro = GetPicValue(_url, bodys);

            if (_terro == -1)
            {
                return result;
            }

            if (_sex < ValidatePic.SuspiciousVal && _terro < ValidatePic.SuspiciousVal)
            {
                result = true;
            }

            return result;
        }

        public static int GetPicValue(string url, string bodys)
        {
            int result = -1;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
            String querys = "";
            if (0 < querys.Length)
            {
                url = url + "?" + querys;
            }

            if (Host.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = "POST";
            httpRequest.Headers.Add("Authorization", "APPCODE b959e2775668484f8e47cee1edffa0cc");
            //根据API的要求，定义相对应的Content-Type
            httpRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            if (0 < bodys.Length)
            {
                byte[] data = Encoding.UTF8.GetBytes(bodys);
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }

            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
            string StrDate = "";
            string strValue = "";
            while ((StrDate = reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }

            var _r = (ValidatePicModel)Newtonsoft.Json.JsonConvert.DeserializeObject(strValue, (new ValidatePicModel()).GetType());
            if (_r.showapi_res_body.ret_code == 0)
            {
                result = _r.showapi_res_body.rate;
                if (_r.showapi_res_body.code == "needManualReview")
                {
                    result = 100;
                }
            }
            return result;
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        private static string Url(string picUrl)
        {
            picUrl = picUrl.Replace("\\", "/");
            picUrl = picUrl.Replace(@"\", "/");
            return picUrl;
        }
    }

    public class ValidatePicModel
    {
        public int showapi_res_code { get; set; }
        public string showapi_res_error { get; set; }
        public ValidatePicResultModel showapi_res_body { get; set; }
    }

    public class ValidatePicResultModel
    {
        public int ret_code { get; set; }
        public int rate { get; set; }
        public string code { get; set; }

        // public string remark { get; set; }
    }
}
